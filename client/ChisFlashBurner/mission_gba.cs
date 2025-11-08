using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ChisFlashBurner
{
    public partial class Form1
    {

        byte[] nintendonLogo_gba = new byte[]  {
            0x24, 0xFF, 0xAE, 0x51, 0x69, 0x9A, 0xA2, 0x21, 0x3D, 0x84, 0x82, 0x0A, 0x84, 0xE4, 0x09, 0xAD,
            0x11, 0x24, 0x8B, 0x98, 0xC0, 0x81, 0x7F, 0x21, 0xA3, 0x52, 0xBE, 0x19, 0x93, 0x09, 0xCE, 0x20,
            0x10, 0x46, 0x4A, 0x4A, 0xF8, 0x27, 0x31, 0xEC, 0x58, 0xC7, 0xE8, 0x33, 0x82, 0xE3, 0xCE, 0xBF,
            0x85, 0xF4, 0xDF, 0x94, 0xCE, 0x4B, 0x09, 0xC1, 0x94, 0x56, 0x8A, 0xC0, 0x13, 0x72, 0xA7, 0xFC,
            0x9F, 0x84, 0x4D, 0x73, 0xA3, 0xCA, 0x9A, 0x61, 0x58, 0x97, 0xA3, 0x27, 0xFC, 0x03, 0x98, 0x76,
            0x23, 0x1D, 0xC7, 0x61, 0x03, 0x04, 0xAE, 0x56, 0xBF, 0x38, 0x84, 0x00, 0x40, 0xA7, 0x0E, 0xFD,
            0xFF, 0x52, 0xFE, 0x03, 0x6F, 0x95, 0x30, 0xF1, 0x97, 0xFB, 0xC0, 0x85, 0x60, 0xD6, 0x80, 0x25,
            0xA9, 0x63, 0xBE, 0x03, 0x01, 0x4E, 0x38, 0xE2, 0xF9, 0xA2, 0x34, 0xFF, 0xBB, 0x3E, 0x03, 0x44,
            0x78, 0x00, 0x90, 0xCB, 0x88, 0x11, 0x3A, 0x94, 0x65, 0xC0, 0x7C, 0x63, 0x87, 0xF0, 0x3C, 0xAF,
            0xD6, 0x25, 0xE4, 0x8B, 0x38, 0x0A, 0xAC, 0x72, 0x21, 0xD4, 0xF8, 0x07
        };

        void gba_sramSwitchBank(int bank)
        {
            bank = bank == 0 ? 0 : 1;

            rom_write(0x800000, BitConverter.GetBytes((UInt16)bank));
        }

        void gba_romSwitchBank(int bank, bool isBankIn4m = false)
        {
            if (isBankIn4m)
            {
                int h = ((bank / 8) & 0x0f) << 4;
                int l = 0x40 | ((bank % 8) << 3);

                ram_write(2, new byte[] { (byte)(h) });
                ram_write(3, new byte[] { (byte)(l) });
            }
            else
            {
                int h = (bank & 0x0f) << 4;

                ram_write(2, new byte[] { (byte)h });
                ram_write(3, new byte[] { 0x40 });
            }
        }

        void gba_romSwitchBank_byAddr(int addr)
        {
            int bank = addr / (32 * 1024 * 1024);
            gba_romSwitchBank(bank);
        }


        void gba_flashSwitchBank(int bank)
        {
            bank = bank == 0 ? 0 : 1;

            ram_write(0x5555, new byte[] { 0xaa });
            ram_write(0x2aaa, new byte[] { 0x55 });
            ram_write(0x5555, new byte[] { 0xb0 }); // FLASH_COMMAND_SWITCH_BANK
            ram_write(0x0000, new byte[] { (byte)bank });
        }

        void gba_romGetSize(out int sectorCount, out int sectorSize, out int bufferWriteBytes, out int deviceSize)
        {
            byte[] cfi = new byte[20];
            int temp, temp1;

            // CFI Query
            rom_write(0x55, BitConverter.GetBytes((UInt16)0x98));
            rom_read(0x27 << 1, ref cfi);
            // reset
            rom_write(0x00, BitConverter.GetBytes((UInt16)0xf0));

            temp = BitConverter.ToUInt16(cfi, 0); // 27h
            deviceSize = (int)Math.Pow(2, temp);

            temp = BitConverter.ToUInt16(cfi, 6); // 2a
            if (temp == 0)
                bufferWriteBytes = 0;
            else
                bufferWriteBytes = (int)Math.Pow(2, temp);

            temp = BitConverter.ToUInt16(cfi, 12);  // 2d
            temp1 = BitConverter.ToUInt16(cfi, 14); // 2e
            sectorCount = (((temp1 & 0xff) << 8) | (temp & 0xff)) + 1;

            temp = BitConverter.ToUInt16(cfi, 16); // 2f
            temp1 = BitConverter.ToUInt16(cfi, 18); // 30
            sectorSize = (((temp1 & 0xff) << 8) | (temp & 0xff)) * 256;
        }


        int gba_romGetEraseTime()
        {
            int deviceSize, sectorCount, sectorSize, bufferWriteBytes;
            gba_romGetSize(out sectorCount, out sectorSize, out bufferWriteBytes, out deviceSize);

            byte[] cfi = new byte[20];
            int temp;

            // CFI Query
            rom_write(0x55, BitConverter.GetBytes((UInt16)0x98));
            rom_read(0x1f << 1, ref cfi);
            // reset
            rom_write(0x00, BitConverter.GetBytes((UInt16)0xf0));

            //Typical timeout per individual block erase = 2^n ms
            temp = BitConverter.ToUInt16(cfi, 4);
            int timeout_block = (int)Math.Pow(2, temp);

            //Typical timeout for full chip erase = 2^nms
            temp = BitConverter.ToUInt16(cfi, 6);
            int timeout_chip = (int)Math.Pow(2, temp);

            //// Maximum timeout for chip erase = 2n times typical
            //temp = BitConverter.ToUInt16(cfi, 14);
            //int timeout_chipMax = (int)Math.Pow(2, temp);

            //return timeout_chip * timeout_chipMax;
            //return (int)(timeout_chip * 1.5);
            //return timeout_chip;
            if (timeout_chip == 1)
                return timeout_block * sectorCount;
            else
                return timeout_chip;
        }

        void gba_romEraseSector(int addrFrom, int addrTo, int sectorSize, bool isMultiCard = false)
        {
            addrFrom /= 2;
            addrTo /= 2;
            sectorSize /= 2;

            int sectorMask = sectorSize - 1;
            addrTo &= ~sectorMask;

            printLog(string.Format("擦除 0x{0:x8} - 0x{1:x8}", addrFrom, addrTo));

            int currentBank = -1;

            for (int sa = addrTo; sa >= addrFrom; sa -= sectorSize)
            {
                printLog(string.Format("    0x{0:x8}", sa));

                if (isMultiCard)
                {
                    int bank = sa / (32 / 2 * 1024 * 1024);
                    if (bank != currentBank)
                    {
                        printLog(string.Format("切换至Bank {0}", bank));
                        gba_romSwitchBank(bank);
                        currentBank = bank;
                    }
                }

                // Sector Erase
                rom_write(0x555, BitConverter.GetBytes((UInt16)0xaa));
                rom_write(0x2aa, BitConverter.GetBytes((UInt16)0x55));
                rom_write(0x555, BitConverter.GetBytes((UInt16)0x80));
                rom_write(0x555, BitConverter.GetBytes((UInt16)0xaa));
                rom_write(0x2aa, BitConverter.GetBytes((UInt16)0x55));
                rom_write((uint)sa, BitConverter.GetBytes((UInt16)0x30));

                showProgress((sa - addrFrom) / sectorSize + 1, (addrTo - addrFrom) / sectorSize + 1);

                byte[] temp = new byte[2];
                do
                {
                    rom_read((uint)(sa << 1), ref temp);
                    Thread.Sleep(20);
                } while (BitConverter.ToUInt16(temp, 0) != 0xffff);
            }
        }

        bool gba_isMultiCard()
        {
            int deviceSize, sectorCount, sectorSize, bufferWriteBytes;
            gba_romGetSize(out sectorCount, out sectorSize, out bufferWriteBytes, out deviceSize);

            int i = comboBox_gbaMultiCartSelect.SelectedIndex;

            if (i != 0 || deviceSize > 32 * 1024 * 1024)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        int gba_multiCardBaseAddr()
        {
            int i = comboBox_gbaMultiCartSelect.SelectedIndex;

            if (i >= 2)
            {
                return (8 + 4 * (i - 2)) * 1024 * 1024;
            }
            else
            {
                return 0;
            }
        }


        //////////////////////////////////////
        //////////////////////////////////////
        //////////////////////////////////////

        void mission_readRomID()
        {
            // id
            byte[] id = new byte[8];
            rom_readID(ref id);

            printLog(BitConverter.ToString(id).Replace("-", " "));

            if (id.SequenceEqual(new byte[] { 0x01, 0x00, 0x7e, 0x22, 0x22, 0x22, 0x01, 0x22 }))
                printLog("S29GL256");
            else if (id.SequenceEqual(new byte[] { 0x89, 0x00, 0x7e, 0x22, 0x22, 0x22, 0x01, 0x22 }))
                printLog("JS28F256");
            else if (id.SequenceEqual(new byte[] { 0x01, 0x00, 0x7e, 0x22, 0x28, 0x22, 0x01, 0x22 }))
                printLog("S29GL01");
            else if (id.SequenceEqual(new byte[] { 0x01, 0x00, 0x7e, 0x22, 0x48, 0x22, 0x01, 0x22 }))
                printLog("S70GL02");
            else if (id.SequenceEqual(new byte[] { 0xC2, 0x00, 0x7E, 0x22, 0x22, 0x22, 0x01, 0x22 }))
                printLog("MX29GL256");
            else if (id.SequenceEqual(new byte[] { 0x20, 0x00, 0x7E, 0x22, 0x22, 0x22, 0x01, 0x22 }))
                printLog("M29W256G");
            else
                printLog("ID暂未收录，可能无法写入");

            // cfi
            int deviceSize, sectorCount, sectorSize, bufferWriteBytes;
            gba_romGetSize(out sectorCount, out sectorSize, out bufferWriteBytes, out deviceSize);

            printLog(string.Format("容量:{0:d} 扇区数量:{1:d} 扇区大小:{2:d} BuffWr:{3:d}", deviceSize, sectorCount, sectorSize, bufferWriteBytes));

            // 合卡信息
            if (deviceSize > 32 * 1024 * 1024)
            {
                int bankCount = deviceSize / (4 * 1024 * 1024);

                byte[] header = new byte[200];
                byte[] logo = new byte[156];

                bool found = false;
                for (int i = 0; i < bankCount; i++)
                {
                    gba_romSwitchBank(i, isBankIn4m: true);
                    rom_read(0, ref header);

                    Array.Copy(header, 4, logo, 0, 156);

                    if (logo.SequenceEqual(nintendonLogo_gba))
                    {
                        if (!found)
                        {
                            found = true;
                            printLog("以下位置存在游戏");
                        }

                        string gameName = Encoding.UTF8.GetString(header, 160, 12).TrimEnd('\0');


                        printLog(string.Format("[{0:s} M]: {1:s}",
                            (i * 4).ToString().PadLeft(3, ' '),
                            gameName
                        ));
                    }
                }
                if (found) printLog("----------------------------------------");

                gba_romSwitchBank(0);
            }

            port.Close();
            enableButton();
        }

        void mission_eraseChip()
        {

            // 02g的胶水双核要擦两次
            byte[] id = new byte[8];
            rom_readID(ref id);

            bool isS70GL02 = id.SequenceEqual(new byte[] { 0x01, 0x00, 0x7e, 0x22, 0x48, 0x22, 0x01, 0x22 });
            if (isS70GL02)
                gba_romSwitchBank(0);

            // 获取擦除时间
            int eraseTimeMs = gba_romGetEraseTime();

            // 开擦
            rom_eraseChip();

            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();

            // 检查结果
            while (true)
            {
                showProgress(
                    (int)stopwatch.ElapsedMilliseconds,
                    eraseTimeMs
                );

                byte[] respon = new byte[2];
                rom_read(0x000000, ref respon);

                printLog(string.Format("..... {0:x4}", BitConverter.ToUInt16(respon, 0)));
                if (BitConverter.ToUInt16(respon, 0) == 0xffff)
                    break;
                else
                    Thread.Sleep(1000);
            }

            if (isS70GL02)
            {
                gba_romSwitchBank(4);
                printLog("02G再擦1次");

                // 擦
                rom_eraseChip();
                // 检查结果
                while (true)
                {
                    showProgress(
                        (int)stopwatch.ElapsedMilliseconds,
                        eraseTimeMs
                    );

                    byte[] respon = new byte[2];
                    rom_read(0x000000, ref respon);

                    printLog(string.Format("..... {0:x4}", BitConverter.ToUInt16(respon, 0)));
                    if (BitConverter.ToUInt16(respon, 0) == 0xffff)
                        break;
                    else
                        Thread.Sleep(1000);
                }
            }

            stopwatch.Stop();
            printLog("擦除完毕");
            printLog(string.Format("擦除耗时 {0:f3} s", stopwatch.ElapsedMilliseconds / 1000.0f));

            port.Close();
            enableButton();
        }

        void mission_programRom()
        {
            // 打开文件
            string romFilePath = textBox_romPath.Text;

            //FileStream file;
            try
            {
                file = new FileStream(romFilePath, FileMode.Open, FileAccess.Read);
            }
            catch
            {
                printLog("文件被占用");
                port.Close();
                enableButton();
                return;
            }

            int fileLength = (int)file.Length;
            int romBufSize = fileLength;

            if (fileLength % 2 != 0)
            {
                romBufSize += 1; // 补齐为偶数
            }

            byte[] rom = new byte[romBufSize];
            file.Read(rom, 0, fileLength);
            file.Close();

            if (fileLength % 2 != 0)
            {
                rom[romBufSize - 1] = 0;
            }

            // 获取rom flash buffer大小
            int deviceSize, sectorCount, sectorSize, bufferWriteBytes;
            gba_romGetSize(out sectorCount, out sectorSize, out bufferWriteBytes, out deviceSize);

            bool isMultiCard = gba_isMultiCard();


            // 获取工作区间
            int addrBegin, addrEnd;
            addrBegin = gba_multiCardBaseAddr();
            addrEnd = addrBegin + romBufSize - 1;


            // 检查rom容量
            if ((addrEnd + 1) > deviceSize)
            {
                printLog(string.Format("Flash 空间不足 需要{0:d} 剩余{1:d}", romBufSize, deviceSize - addrBegin));
                port.Close();
                enableButton();
                return;
            }

            // 自动擦除flash
            if (isMultiCard)
            {
                int bank = addrBegin / (32 * 1024 * 1024);
                gba_romSwitchBank(bank);
            }
            byte[] temp = new byte[512];
            rom_read((UInt32)addrBegin, ref temp);

            if (!isBlank(temp))
            {
                Stopwatch swErase = new Stopwatch();
                swErase.Start();

                gba_romEraseSector(addrBegin, addrEnd, sectorSize, isMultiCard);

                swErase.Stop();
                printLog(string.Format("擦除耗时 {0:f3} s", swErase.ElapsedMilliseconds / 1000.0f));
            }

            printLog("开始写入");

            // 开始发送
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();

            int currentBank = -1;
            int writtenCount = 0;

            while (writtenCount < romBufSize)
            {
                // 分包
                int sentLen = romBufSize - writtenCount;
                sentLen = sentLen > 4096 ? 4096 : sentLen;

                byte[] sendPack = new byte[sentLen];
                Array.Copy(rom, writtenCount, sendPack, 0, sentLen);

                int romAddress = addrBegin + writtenCount;

                // 切bank
                if (isMultiCard)
                {
                    int bank = romAddress / (32 * 1024 * 1024);
                    if (bank != currentBank)
                    {
                        printLog(string.Format("切换至Bank {0}", bank));
                        gba_romSwitchBank(bank);
                        currentBank = bank;
                    }
                }

                // 写入
                rom_program((UInt32)romAddress, sendPack, (UInt16)bufferWriteBytes);
                //rom_program((UInt32)romAddress, sendPack, 0);

                writtenCount += sentLen;
                showProgress(writtenCount, romBufSize);
            }
            stopwatch.Stop();

            port.Close();
            enableButton();

            printScore(romBufSize, stopwatch.ElapsedMilliseconds);
        }

        void mission_dumpRom()
        {
            // 打开文件
            string romFilePath = textBox_romPath.Text;
            //FileStream file;
            try
            {
                file = new FileStream(romFilePath, FileMode.Create, FileAccess.Write);
            }
            catch
            {
                printLog("文件被占用");
                port.Close();
                enableButton();
                return;
            }

            int fileLength = (int)(double.Parse(comboBox_romSize.Text) * 1024 * 1024);
            if (fileLength % 2 != 0)
                fileLength += 1;

            byte[] rom = new byte[fileLength];

            // 获取rom flash buffer大小
            int deviceSize, sectorCount, sectorSize, bufferWriteBytes;
            gba_romGetSize(out sectorCount, out sectorSize, out bufferWriteBytes, out deviceSize);

            bool isMultiCard = gba_isMultiCard();

            // 获取工作区间
            int addrBegin, addrEnd;
            addrBegin = gba_multiCardBaseAddr();
            addrEnd = addrBegin + fileLength - 1;

            if (addrEnd > deviceSize - 1)
            {
                fileLength = deviceSize - addrBegin;
                if (fileLength <= 0)
                {
                    printLog("该地址无数据");
                    file.Close();
                    port.Close();
                    enableButton();
                    return;
                }
            }

            // 开始读取
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();

            int currentBank = -1;
            int readCount = 0;

            while (readCount < fileLength)
            {
                // 分包
                int readLen = fileLength - readCount;
                readLen = readLen > 4096 ? 4096 : readLen;

                int romAddress = addrBegin + readCount;

                // 切bank
                if (isMultiCard)
                {
                    int bank = romAddress / (32 * 1024 * 1024);
                    if (bank != currentBank)
                    {
                        printLog(string.Format("切换至Bank {0}", bank));
                        gba_romSwitchBank(bank);
                        currentBank = bank;
                    }
                }

                // 读取
                byte[] respon = new byte[readLen];
                rom_read((UInt32)romAddress, ref respon);


                // 保存
                Array.Copy(respon, 0, rom, readCount, readLen);
                readCount += readLen;
                showProgress(readCount, fileLength);

            }

            stopwatch.Stop();

            file.Write(rom, 0, fileLength);
            file.Close();

            port.Close();
            enableButton();

            printScore(fileLength, stopwatch.ElapsedMilliseconds);
        }

        void mission_verifyRom()
        {
            // 打开文件
            string romFilePath = textBox_romPath.Text;

            //FileStream file;
            try
            {
                file = new FileStream(romFilePath, FileMode.Open, FileAccess.Read);
            }
            catch
            {
                printLog("文件被占用");
                port.Close();
                enableButton();
                return;
            }

            int fileLength = (int)file.Length;
            int romBufSize = fileLength;

            if (fileLength % 2 != 0)
            {
                romBufSize += 1; // 补齐为偶数
            }

            byte[] rom = new byte[romBufSize];
            file.Read(rom, 0, fileLength);
            file.Close();

            if (fileLength % 2 != 0)
            {
                rom[romBufSize - 1] = 0; // 补齐为偶数
            }

            // 获取rom flash buffer大小
            int deviceSize, sectorCount, sectorSize, bufferWriteBytes;
            gba_romGetSize(out sectorCount, out sectorSize, out bufferWriteBytes, out deviceSize);

            bool isMultiCard = gba_isMultiCard();

            // 获取工作区间
            int addrBegin, addrEnd;
            addrBegin = gba_multiCardBaseAddr();
            addrEnd = addrBegin + romBufSize - 1;

            // 合卡位置判断
            if (addrEnd > deviceSize - 1)
            {
                romBufSize = deviceSize - addrBegin;
                if (romBufSize <= 0)
                {
                    printLog("该地址无数据");
                    port.Close();
                    enableButton();
                    return;
                }
            }

            // 开始校验
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();

            int currentBank = -1;
            int readCount = 0;

            while (readCount < romBufSize)
            {
                // 分包
                int readLen = romBufSize - readCount;
                readLen = readLen > 4096 ? 4096 : readLen;
                //readLen = readLen > 32768 ? 32768 : readLen;

                int romAddress = addrBegin + readCount;

                // 切bank
                if (isMultiCard)
                {
                    int bank = romAddress / (32 * 1024 * 1024);
                    if (bank != currentBank)
                    {
                        printLog(string.Format("切换至Bank {0}", bank));
                        gba_romSwitchBank(bank);
                        currentBank = bank;
                    }
                }

                // 读取
                byte[] respon = new byte[readLen];
                rom_read((UInt32)romAddress, ref respon);

                for (int i = 0; i < readLen; i++)
                {
                    if (rom[readCount + i] != respon[i])
                    {
                        printLog(string.Format(
                            "0x{0:x8}校验失败，{1:x2} -> {2:x2}",
                            readCount + i,
                            rom[readCount + i],
                            respon[i]
                        ));
                    }
                }
                readCount += readLen;
                showProgress(readCount, romBufSize);
            }
            stopwatch.Stop();

            port.Close();
            enableButton();

            printScore(romBufSize, stopwatch.ElapsedMilliseconds);
        }

        void mission_wrtieSram()
        {
            // 打开文件
            string savFilePath = textBox_savePath.Text;

            //FileStream file;
            try
            {
                file = new FileStream(savFilePath, FileMode.Open, FileAccess.Read);
            }
            catch
            {
                printLog("文件被占用");
                port.Close();
                enableButton();
                return;
            }

            int fileLength = (int)file.Length;

            byte[] sav = new byte[fileLength];
            file.Read(sav, 0, fileLength);
            file.Close();

            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();

            // 如果是flash就擦除
            if (comboBox_ramType.Text == "FLASH")
            {
                // 擦除flash
                printLog("擦除flash");
                ram_write(0x5555, new byte[] { 0xaa });
                ram_write(0x2aaa, new byte[] { 0x55 });
                ram_write(0x5555, new byte[] { 0x80 });
                ram_write(0x5555, new byte[] { 0xaa });
                ram_write(0x2aaa, new byte[] { 0x55 });
                ram_write(0x5555, new byte[] { 0x10 }); // Chip-Erase
                //ram_write(0x7654, new byte[] { 0x30 }); // sector-Erase
                Thread.Sleep(200);

                // 等待擦除完成
                while (true)
                {
                    byte[] respon = new byte[1];

                    ram_read(0x0000, ref respon);
                    //ram_read(0x7654, ref respon);

                    printLog(string.Format("..... {0:x2}", respon[0]));
                    if (respon[0] == 0xff)
                    {
                        printLog("擦除完毕");
                        showProgress(0, fileLength);
                        break;
                    }
                    else
                    {
                        Thread.Sleep(200);
                    }

                }

                //port.Close();
                //enableButton();
                //return;
            }

            // 开始写入
            int writtenCount = 0;
            while (writtenCount < fileLength)
            {
                // 切bank
                if (writtenCount == 0x00000)
                {
                    printLog("切换至Bank 0");
                    if (comboBox_ramType.Text == "FLASH")
                        gba_flashSwitchBank(0);
                    else
                        gba_sramSwitchBank(0);
                }
                if (writtenCount == 0x10000)
                {
                    printLog("切换至Bank 1");
                    if (comboBox_ramType.Text == "FLASH")
                        gba_flashSwitchBank(1);
                    else
                        gba_sramSwitchBank(1);
                }

                UInt32 baseAddr = (UInt32)(writtenCount & 0xffff);

                // 分包
                int sentLen = fileLength - writtenCount;
                sentLen = sentLen > 4096 ? 4096 : sentLen;

                byte[] sendPack = new byte[sentLen];
                Array.Copy(sav, writtenCount, sendPack, 0, sentLen);

                // 发送
                if (comboBox_ramType.Text == "FLASH")
                    ram_flashProgram(baseAddr, sendPack);
                else if (comboBox_ramType.Text == "FRAM")
                    ram_write_forFram(baseAddr, sendPack, 25);
                else // SRAM
                    ram_write(baseAddr, sendPack);


                writtenCount += sentLen;
                showProgress(writtenCount, fileLength);

            }
            stopwatch.Stop();

            port.Close();
            enableButton();

            printScore(fileLength, stopwatch.ElapsedMilliseconds);
        }

        void mission_dumpRam()
        {
            string romFilePath = textBox_savePath.Text; // 打开文件
            FileStream file;
            try
            {
                file = new FileStream(romFilePath, FileMode.Create, FileAccess.Write);
            }
            catch
            {
                printLog("文件被占用");
                port.Close();
                enableButton();
                return;
            }

            int fileLength = (int)(double.Parse(comboBox_saveSize.Text) * 1024.0);
            byte[] sav = new byte[fileLength];

            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();

            int readCount = 0;
            while (readCount < fileLength)
            {
                // 切bank
                if (readCount == 0x00000)
                {
                    printLog("切换至Bank 0");
                    if (comboBox_ramType.Text == "FLASH")
                        gba_flashSwitchBank(0);
                    else
                        gba_sramSwitchBank(0);
                }
                if (readCount == 0x10000)
                {
                    printLog("切换至Bank 1");
                    if (comboBox_ramType.Text == "FLASH")
                        gba_flashSwitchBank(1);
                    else
                        gba_sramSwitchBank(1);
                }

                UInt32 baseAddr = (UInt32)(readCount & 0xffff);

                // 分包
                int readLen = fileLength - readCount;
                readLen = readLen > 4096 ? 4096 : readLen;

                // 读取
                byte[] respon = new byte[readLen];

                if (comboBox_ramType.Text == "FRAM")
                    ram_read_forFram((UInt32)baseAddr, ref respon, 25);
                else
                    ram_read((UInt32)baseAddr, ref respon);

                Array.Copy(respon, 0, sav, readCount, readLen);
                readCount += readLen;
                showProgress(readCount, fileLength);
            }
            stopwatch.Stop();

            // 保存

            file.Write(sav, 0, fileLength);
            file.Close();

            port.Close();
            enableButton();

            printScore(fileLength, stopwatch.ElapsedMilliseconds);
        }

        void mission_verifyRam()
        {
            // 打开文件
            string savFilePath = textBox_savePath.Text;

            //FileStream file;
            try
            {
                file = new FileStream(savFilePath, FileMode.Open, FileAccess.Read);
            }
            catch
            {
                printLog("文件被占用");
                port.Close();
                enableButton();
                return;
            }

            int fileLength = (int)file.Length;

            byte[] sav = new byte[fileLength];
            file.Read(sav, 0, fileLength);
            file.Close();

            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();

            int readCount = 0;
            while (readCount < fileLength)
            {
                // 切bank
                if (readCount == 0x00000)
                {
                    printLog("切换至Bank 0");
                    if (comboBox_ramType.Text == "FLASH")
                        gba_flashSwitchBank(0);
                    else
                        gba_sramSwitchBank(0);
                }
                if (readCount == 0x10000)
                {
                    printLog("切换至Bank 1");
                    if (comboBox_ramType.Text == "FLASH")
                        gba_flashSwitchBank(1);
                    else
                        gba_sramSwitchBank(1);
                }

                UInt32 baseAddr = (UInt32)(readCount & 0xffff);

                // 分包
                int readLen = fileLength - readCount;
                readLen = readLen > 4096 ? 4096 : readLen;

                // 读取
                byte[] respon = new byte[readLen];

                if (comboBox_ramType.Text == "FRAM")
                    ram_read_forFram((UInt32)baseAddr, ref respon, 25);
                else
                    ram_read((UInt32)baseAddr, ref respon);

                for (int i = 0; i < readLen; i++)
                {
                    if (sav[readCount + i] != respon[i])
                    {
                        printLog(string.Format(
                            "0x{0:x8}校验失败，{1:x2} -> {2:x2}",
                            readCount + i,
                            sav[readCount + i],
                            respon[i]
                        ));
                    }
                }
                readCount += readLen;
                showProgress(readCount, fileLength);


            }

            stopwatch.Stop();

            printLog("校验结束");
            port.Close();
            enableButton();

            printScore(fileLength, stopwatch.ElapsedMilliseconds);
        }



        ////////////////////////////////////////////////////////////
        /// 免电
        /// 

        bool gba_searchBatteryless(UInt32 baseAddress, out int offset, out int size)
        {
            offset = 0; // batteryless SRAM数据的位置
            size = 0;


            // 获取rom flash buffer大小
            int deviceSize, sectorCount, sectorSize, bufferWriteBytes;
            gba_romGetSize(out sectorCount, out sectorSize, out bufferWriteBytes, out deviceSize);
            bool isMultiCard = gba_isMultiCard();




            if (isMultiCard)
                gba_romSwitchBank_byAddr((int)baseAddress);
            byte[] boot = new byte[4];
            rom_read(baseAddress, ref boot);

            UInt32 boot_vector = BitConverter.ToUInt32(boot, 0);
            boot_vector = ((boot_vector & 0x00FFFFFF) + 2) << 2;

            byte[] targetBuf = Encoding.ASCII.GetBytes("<3 from Maniac");
            byte[] searchBuf = new byte[0x2000];


            if (isMultiCard)
                gba_romSwitchBank_byAddr((int)(baseAddress + boot_vector));
            byte[] temp = new byte[4096]; // 分开读，mcu内存没这么大
            rom_read(baseAddress + boot_vector, ref temp);
            Array.Copy(temp, 0, searchBuf, 0, 4096);
            rom_read(baseAddress + boot_vector + 4096, ref temp);
            Array.Copy(temp, 0, searchBuf, 4096, 4096);

            bool found = false;
            for (int i = 0; i < searchBuf.Length; i++)
            {
                found = true;
                for (int ii = 0; ii < targetBuf.Length; ii++)
                {
                    if (searchBuf[i + ii] != targetBuf[ii])
                    {
                        found = false;
                        break;
                    }
                }
                if (found)
                {
                    int payload_size = BitConverter.ToUInt16(searchBuf, i + 0x0e);
                    if (payload_size == 0)
                        payload_size = 0x414;

                    offset = (int)(baseAddress + boot_vector + i + 0x10); // <3 from Maniac后面是payload的大小和数据

                    int payload_start = offset - payload_size;

                    if (isMultiCard)
                        gba_romSwitchBank_byAddr(payload_start);
                    rom_read((UInt32)payload_start, ref temp);

                    size = (int)BitConverter.ToUInt32(temp, 8);

                    return true;
                }
            }

            return false;
        }


        void mission_writeSave_batteryless()
        {
            // 打开文件
            string savFilePath = textBox_savePath.Text;

            //FileStream file;
            try
            {
                file = new FileStream(savFilePath, FileMode.Open, FileAccess.Read);
            }
            catch
            {
                printLog("文件被占用");
                port.Close();
                enableButton();
                return;
            }

            int fileLength = (int)file.Length;

            byte[] sav = new byte[fileLength];
            file.Read(sav, 0, fileLength);
            file.Close();


            // 获取rom flash buffer大小
            int deviceSize, sectorCount, sectorSize, bufferWriteBytes;
            gba_romGetSize(out sectorCount, out sectorSize, out bufferWriteBytes, out deviceSize);

            bool isMultiCard = gba_isMultiCard();
            int baseAddress = gba_multiCardBaseAddr();

            if (isMultiCard)
                gba_romSwitchBank_byAddr(baseAddress);



            // 找呀找呀找存档
            int sav_offset, sav_size;
            bool found = gba_searchBatteryless((UInt32)baseAddress, out sav_offset, out sav_size);

            if (!found)
            {
                printLog("找不到免电存档");
                port.Close();
                enableButton();
                return;
            }

            if (sav_size > fileLength)
                sav_size = fileLength;
            printLog(string.Format("Offset:{0:x8} Size:{1:d}", sav_offset, sav_size));



            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();

            // 开始写入

            printLog(string.Format("擦除存档 {0:x8} - {1:x8}", sav_offset, sav_offset + sav_size));
            gba_romEraseSector(sav_offset, sav_offset + sav_size, sectorSize, isMultiCard);

            int writtenCount = 0;
            while (writtenCount < sav_size)
            {

                // 分包
                int sentLen = sav_size - writtenCount;
                sentLen = sentLen > 4096 ? 4096 : sentLen;

                int addr = baseAddress + sav_offset + writtenCount;

                if (isMultiCard)
                    gba_romSwitchBank_byAddr(addr);

                byte[] sendPack = new byte[sentLen];
                Array.Copy(sav, writtenCount, sendPack, 0, sentLen);

                // 发送
                rom_program((UInt32)addr, sendPack, (UInt16)bufferWriteBytes);

                writtenCount += sentLen;
                showProgress(writtenCount, sav_size);

            }
            stopwatch.Stop();


            port.Close();
            enableButton();
            printScore(sav_size, stopwatch.ElapsedMilliseconds);
        }



        void mission_dumpSave_batteryless()
        {
            // 打开文件
            string romFilePath = textBox_savePath.Text;
            //FileStream file;
            try
            {
                file = new FileStream(romFilePath, FileMode.Create, FileAccess.Write);
            }
            catch
            {
                printLog("文件被占用");
                port.Close();
                enableButton();
                return;
            }


            // 获取rom flash buffer大小
            int deviceSize, sectorCount, sectorSize, bufferWriteBytes;
            gba_romGetSize(out sectorCount, out sectorSize, out bufferWriteBytes, out deviceSize);

            bool isMultiCard = gba_isMultiCard();
            int baseAddress = gba_multiCardBaseAddr();

            if (isMultiCard)
                gba_romSwitchBank_byAddr(baseAddress);



            // 找呀找呀找存档
            int sav_offset, sav_size;
            bool found = gba_searchBatteryless((UInt32)baseAddress, out sav_offset, out sav_size);

            if (!found)
            {
                printLog("找不到免电存档");
                file.Close();
                port.Close();
                enableButton();
                return;
            }
            printLog(string.Format("Offset:{0:x8} Size:{1:d}", sav_offset, sav_size));


            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();


            // 导出存档
            byte[] sav = new byte[sav_size];

            int readCount = 0;
            while (readCount < sav_size)
            {
                // 分包
                int readLen = sav_size - readCount;
                readLen = readLen > 4096 ? 4096 : readLen;

                int addr = baseAddress + sav_offset + readCount;

                if (isMultiCard)
                    gba_romSwitchBank_byAddr(addr);

                // 读取
                byte[] respon = new byte[readLen];

                rom_read((UInt32)addr, ref respon);

                Array.Copy(respon, 0, sav, readCount, readLen);
                readCount += readLen;
                showProgress(readCount, sav_size);
            }
            stopwatch.Stop();


            // 保存
            file.Write(sav, 0, sav_size);
            file.Close();


            port.Close();
            enableButton();
            printScore(sav_size, stopwatch.ElapsedMilliseconds);
        }


        void mission_verifySave_batteryless()
        {
            // 打开文件
            string savFilePath = textBox_savePath.Text;

            //FileStream file;
            try
            {
                file = new FileStream(savFilePath, FileMode.Open, FileAccess.Read);
            }
            catch
            {
                printLog("文件被占用");
                port.Close();
                enableButton();
                return;
            }

            int fileLength = (int)file.Length;

            byte[] sav = new byte[fileLength];
            file.Read(sav, 0, fileLength);
            file.Close();


            // 获取rom flash buffer大小
            int deviceSize, sectorCount, sectorSize, bufferWriteBytes;
            gba_romGetSize(out sectorCount, out sectorSize, out bufferWriteBytes, out deviceSize);

            bool isMultiCard = gba_isMultiCard();
            int baseAddress = gba_multiCardBaseAddr();

            if (isMultiCard)
                gba_romSwitchBank_byAddr(baseAddress);

            // 找呀找呀找存档
            int sav_offset, sav_size;
            bool found = gba_searchBatteryless((UInt32)baseAddress, out sav_offset, out sav_size);

            if (!found)
            {
                printLog("找不到免电存档");
                file.Close();
                port.Close();
                enableButton();
                return;
            }

            if (sav_size > fileLength)
                sav_size = fileLength;
            printLog(string.Format("Offset:{0:x8} Size:{1:d}", sav_offset, sav_size));



            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();


            // 导出存档

            int readCount = 0;
            while (readCount < sav_size)
            {
                // 分包
                int readLen = sav_size - readCount;
                readLen = readLen > 4096 ? 4096 : readLen;

                int addr = baseAddress + sav_offset + readCount;

                // 读取
                byte[] respon = new byte[readLen];

                if (isMultiCard)
                    gba_romSwitchBank_byAddr(addr);
                rom_read((UInt32)addr, ref respon);


                // 对比
                for (int i = 0; i < readLen; i++)
                {
                    if (sav[readCount + i] != respon[i])
                    {
                        printLog(string.Format(
                            "0x{0:x8}校验失败，{1:x2} -> {2:x2}",
                            readCount + i,
                            sav[readCount + i],
                            respon[i]
                        ));
                    }
                }

                readCount += readLen;
                showProgress(readCount, sav_size);
            }
            stopwatch.Stop();


            port.Close();
            enableButton();
            printScore(sav_size, stopwatch.ElapsedMilliseconds);

        }
    }
}
