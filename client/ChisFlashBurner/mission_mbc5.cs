using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Rebar;

namespace ChisFlashBurner
{
    public partial class Form1
    {
        /**
         * |             | ROM                             | RAM                      |                                                                                                                                                             |
         * | 菜单        | 0 0000 BBBB BBxx xxxx xxxx xxxx | 0000 0BBx xxxx xxxx xxxx | 1m 32k                                                                                                                                                      |
         * | 游戏1       | 0 0001 BBBB BBxx xxxx xxxx xxxx | 0000 0BBx xxxx xxxx xxxx | 1m 32k                                                                                                                                                      |
         * | 游戏2       | 0 001B BBBB BBxx xxxx xxxx xxxx | 0000 1BBx xxxx xxxx xxxx | 2m 32k                                                                                                                                                      |
         * | 游戏3       | 0 010B BBBB BBxx xxxx xxxx xxxx | 0001 0BBx xxxx xxxx xxxx | 2m 32k                                                                                                                                                      |
         * | 游戏4       | 0 011B BBBB BBxx xxxx xxxx xxxx | 0001 1BBx xxxx xxxx xxxx | 2m 32k                                                                                                                                                      |
         * | max         |                                 |                          |                                                                                                                                                             |
         * | 游戏5       | 0 100B BBBB BBxx xxxx xxxx xxxx | 0010 0BBx xxxx xxxx xxxx | 2m 32k                                                                                                                                                      |
         * | 游戏6       | 0 101B BBBB BBxx xxxx xxxx xxxx | 0010 1BBx xxxx xxxx xxxx | 2m 32k                                                                                                                                                      |
         * | 游戏7       | 0 110B BBBB BBxx xxxx xxxx xxxx | 0011 0BBx xxxx xxxx xxxx | 2m 32k                                                                                                                                                      |
         * | 游戏8       | 0 111B BBBB BBxx xxxx xxxx xxxx | 0011 1BBx xxxx xxxx xxxx | 2m 32k                                                                                                                                                      |
         * | 游戏9       | 1 000B BBBB BBxx xxxx xxxx xxxx | 0100 0BBx xxxx xxxx xxxx | 2m 32k                                                                                                                                                      |
         * | 游戏10      | 1 001B BBBB BBxx xxxx xxxx xxxx | 0100 1BBx xxxx xxxx xxxx | 2m 32k                                                                                                                                                      |
         * | 游戏11      | 1 010B BBBB BBxx xxxx xxxx xxxx | 0101 0BBx xxxx xxxx xxxx | 2m 32k                                                                                                                                                      |
         * | 游戏12      | 1 011B BBBB BBxx xxxx xxxx xxxx | 0101 1BBx xxxx xxxx xxxx | 2m 32k                                                                                                                                                      |
         * | 游戏13      | 1 100B BBBB BBxx xxxx xxxx xxxx | 0110 0BBx xxxx xxxx xxxx | 2m 32k                                                                                                                                                      |
         * | 游戏14      | 1 101B BBBB BBxx xxxx xxxx xxxx | 0110 1BBx xxxx xxxx xxxx | 2m 32k                                                                                                                                                      |
         * | 游戏15      | 1 110B BBBB BBxx xxxx xxxx xxxx | 0111 0BBx xxxx xxxx xxxx | 2m 32k                                                                                                                                                      |
         * | 游戏16      | 1 111B BBBB BBxx xxxx xxxx xxxx | 0111 1BBx xxxx xxxx xxxx | 2m 32k  
        */
        UInt32[,] mbc5_multiCardAddrRange_rom = new UInt32[,]
        {
            //   from         to
            { 0, 0 },                   // 整卡
            { 0x00000000, 0x000fffff }, // 菜单
            { 0x00100000, 0x001fffff }, // 游戏1
            { 0x00200000, 0x003fffff }, // 游戏2
            { 0x00400000, 0x005fffff }, // 游戏3
            { 0x00600000, 0x007fffff }, // 游戏4
            { 0x00800000, 0x009fffff }, // 游戏5
            { 0x00a00000, 0x00bfffff }, // 游戏6
            { 0x00c00000, 0x00dfffff }, // 游戏7
            { 0x00e00000, 0x00ffffff }, // 游戏8
            { 0x01000000, 0x011fffff }, // 游戏9
            { 0x01200000, 0x013fffff }, // 游戏10
            { 0x01400000, 0x015fffff }, // 游戏11
            { 0x01600000, 0x017fffff }, // 游戏12
            { 0x01800000, 0x019fffff }, // 游戏13
            { 0x01a00000, 0x01bfffff }, // 游戏14
            { 0x01c00000, 0x01dfffff }, // 游戏15
            { 0x01e00000, 0x01ffffff }  // 游戏16
        };

        UInt32[,] mbc5_multiCardAddrRange_ram = new UInt32[,]
        {
            { 0,0 },             // 整卡
            { 0x00000, 0x07fff },// 菜单
            { 0x00000, 0x07fff}, // 游戏1
            { 0x08000, 0x0ffff}, // 游戏2
            { 0x10000, 0x17fff}, // 游戏3
            { 0x18000, 0x1ffff}, // 游戏4
            { 0x20000, 0x27fff}, // 游戏5
            { 0x28000, 0x2ffff}, // 游戏6
            { 0x30000, 0x37fff}, // 游戏7
            { 0x38000, 0x3ffff}, // 游戏8
            { 0x40000, 0x47fff}, // 游戏9
            { 0x48000, 0x4ffff}, // 游戏10
            { 0x50000, 0x57fff}, // 游戏11
            { 0x58000, 0x5ffff}, // 游戏12
            { 0x60000, 0x67fff}, // 游戏13
            { 0x68000, 0x6ffff}, // 游戏14
            { 0x70000, 0x77fff}, // 游戏15
            { 0x78000, 0x7ffff}  // 游戏16
        };

        void mbc5_romSwitchBank(int bank)
        {
            if (bank < 0)
                return;

            byte b0 = (byte)((bank) & 0xff);
            byte b1 = (byte)((bank >> 8) & 0xff);

            gbcCart_write(0x2000, new byte[] { b0 }); // rom addr [21:14]
            gbcCart_write(0x3000, new byte[] { b1 }); // rom addr [22]
        }

        void mbc5_ramSwitchBank(int bank)
        {
            if (bank < 0)
                return;

            byte b = (byte)((bank) & 0xff);

            gbcCart_write(0x4000, new byte[] { b }); // ram addr [16:13]
        }

        void mbc5_romEraseSector(int addrFrom, int addrTo, int sectorSize)
        {
            int sectorMask = sectorSize - 1;
            addrTo &= ~sectorMask;

            printLog(string.Format("擦除 0x{0:x8} - 0x{1:x8}", addrFrom, addrTo));

            for (int sa = addrTo; sa >= addrFrom; sa -= sectorSize)
            {
                printLog(string.Format("    0x{0:x8}", sa));

                int bank = sa >> 14;
                mbc5_romSwitchBank(bank);

                UInt32 _sa;
                if (bank == 0)
                    _sa = (UInt32)(0x0000 + (sa & 0x3fff));
                else
                    _sa = (UInt32)(0x4000 + (sa & 0x3fff));

                gbcCart_write(0xaaa, new byte[] { 0xaa });
                gbcCart_write(0x555, new byte[] { 0x55 });
                gbcCart_write(0xaaa, new byte[] { 0x80 });
                gbcCart_write(0xaaa, new byte[] { 0xaa });
                gbcCart_write(0x555, new byte[] { 0x55 });
                gbcCart_write(_sa, new byte[] { 0x30 });   // Sector Erase

                showProgress((sa - addrFrom) / sectorSize + 1, (addrTo - addrFrom) / sectorSize + 1);

                byte[] temp = new byte[1];
                do
                {
                    gbcCart_read(_sa, ref temp);
                    Thread.Sleep(20);
                } while (temp[0] != 0xff);
            }
        }

        byte[] mbc5_romGetID()
        {
            byte[] id = new byte[4];

            // enter autoselect
            gbcCart_write(0xaaa, new byte[] { 0xaa });
            gbcCart_write(0x555, new byte[] { 0x55 });
            gbcCart_write(0xaaa, new byte[] { 0x90 });
            // read id
            gbcCart_read(0, ref id);
            // reset
            gbcCart_write(0x00, new byte[] { 0xf0 });

            return id;
        }

        void mbc5_romGetSize(out int secotrCount, out int sectorSize, out int bufferWriteBytes, out int deviceSize)
        {
            byte[] cfi = new byte[1];
            int temp;

            // CFI Query
            gbcCart_write(0xaa, new byte[] { 0x98 });

            gbcCart_read(0x4e, ref cfi);
            deviceSize = (int)Math.Pow(2, cfi[0]);

            gbcCart_read(0x56, ref cfi);
            temp = cfi[0] << 8;
            gbcCart_read(0x54, ref cfi);
            temp |= cfi[0];
            if (temp == 0)
                bufferWriteBytes = 0;
            else
                bufferWriteBytes = (int)Math.Pow(2, cfi[0]);


            gbcCart_read(0x5c, ref cfi);
            temp = cfi[0] << 8;
            gbcCart_read(0x5a, ref cfi);
            temp |= cfi[0];
            secotrCount = temp + 1;

            gbcCart_read(0x60, ref cfi);
            temp = cfi[0] << 8;
            gbcCart_read(0x5e, ref cfi);
            temp |= cfi[0];
            sectorSize = temp * 256;

            //gbcCart_read(0x64, ref cfi);
            //temp = cfi[0] << 8;
            //gbcCart_read(0x62, ref cfi);
            //temp |= cfi[0];
            //secotrCount = temp + 1;

            //gbcCart_read(0x68, ref cfi);
            //temp = cfi[0] << 8;
            //gbcCart_read(0x66, ref cfi);
            //temp |= cfi[0];
            //sectorSize = temp * 256;

            // reset
            gbcCart_write(0x00, new byte[] { 0xf0 });
        }

        ////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////

        void mission_readRomID_mbc5()
        {
            byte[] id = mbc5_romGetID();

            printLog(BitConverter.ToString(id).Replace("-", " "));

            if (id.SequenceEqual(new byte[] { 0xc2, 0xc2, 0xcb, 0xcb }))
                printLog("MX29LV640EB");
            else if (id.SequenceEqual(new byte[] { 0xc2, 0xc2, 0xc9, 0xc9 }))
                printLog("MX29LV640ET");
            else if (id.SequenceEqual(new byte[] { 0x01, 0x01, 0x7e, 0x7e }))
                printLog("S29GL256N");
            else
                printLog("ID暂未收录，可能无法写入");

            int deviceSize, secotrCount, sectorSize, bufferWriteBytes;
            mbc5_romGetSize(out secotrCount, out sectorSize, out bufferWriteBytes, out deviceSize);

            printLog(string.Format("容量:{0:d} 扇区数量:{1:d} 扇区大小:{2:d} BuffWr:{3:d}", deviceSize, secotrCount, sectorSize, bufferWriteBytes));

            port.Close();
            enableButton();
        }

        void mission_eraseChip_mbc5()
        {
            gbcCart_write(0xaaa, new byte[] { 0xaa });
            gbcCart_write(0x555, new byte[] { 0x55 });
            gbcCart_write(0xaaa, new byte[] { 0x80 });
            gbcCart_write(0xaaa, new byte[] { 0xaa });
            gbcCart_write(0x555, new byte[] { 0x55 });
            gbcCart_write(0xaaa, new byte[] { 0x10 }); // Chip Erase

            byte[] temp = new byte[1];
            do
            {
                Thread.Sleep(1000);
                gbcCart_read(0, ref temp);
                printLog(string.Format("..... {0:x2}", temp[0]));
            } while (temp[0] != 0xff);

            printLog("擦除完毕");

            port.Close();
            enableButton();
        }

        void mission_programRom_mbc5()
        {
            // 打开文件
            string romFilePath = textBox_romPath.Text;

            FileStream file;
            try
            {
                file = new FileStream(romFilePath, FileMode.Open, FileAccess.Read);
            }
            catch (System.IO.IOException)
            {
                printLog("文件被占用");
                port.Close();
                enableButton();
                return;
            }

            int fileLength = (int)file.Length;

            byte[] rom = new byte[fileLength];
            file.Read(rom, 0, fileLength);
            file.Close();

            // 获取rom flash buffer大小
            int deviceSize, secotrCount, sectorSize, bufferWriteBytes;
            mbc5_romGetSize(out secotrCount, out sectorSize, out bufferWriteBytes, out deviceSize);

            // 获取工作区间
            int addrBegin, addrEnd;
            int gameSelect = comboBox_mbc5MultiCartSelect.SelectedIndex;
            if (gameSelect != 0)
            {
                addrBegin = (int)mbc5_multiCardAddrRange_rom[gameSelect, 0];
                addrEnd = (int)mbc5_multiCardAddrRange_rom[gameSelect, 1];
            }
            else
            {
                addrBegin = 0;
                addrEnd = deviceSize - 1;
            }

            // 检查rom容量
            if (fileLength > (addrEnd - addrBegin + 1) || (addrEnd + 1) > deviceSize)
            {
                printLog(string.Format("Flash 空间不足 需要{0:d} 剩余{1:d}", fileLength, addrEnd - addrBegin + 1));
                port.Close();
                enableButton();
                return;
            }
            else
            {
                addrEnd = addrBegin + fileLength - 1;
            }


            // 自动擦除flash
            int bank = addrBegin >> 14;
            int addr;

            mbc5_romSwitchBank(bank);

            if (bank == 0)
                addr = 0x0000 + (addrBegin & 0x3fff);
            else
                addr = 0x4000 + (addrBegin & 0x3fff);

            byte[] temp = new byte[512];
            gbcCart_read((UInt32)addr, ref temp);

            if (!isBlank(temp))
            {
                Stopwatch swErase = new Stopwatch();
                swErase.Start();

                mbc5_romEraseSector(addrBegin, addrEnd, sectorSize);

                swErase.Stop();
                printLog(string.Format("擦除耗时 {0:f3} s", swErase.ElapsedMilliseconds / 1000.0f));
            }


            printLog("开始写入");

            // 开始发送
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();

            int currentBank = -123;
            int writtenCount = 0;

            while (writtenCount < fileLength)
            {
                // 分包
                int sentLen = fileLength - writtenCount;
                sentLen = sentLen > 4096 ? 4096 : sentLen;

                byte[] sendPack = new byte[sentLen];
                Array.Copy(rom, writtenCount, sendPack, 0, sentLen);

                int romAddress = addrBegin + writtenCount;
                int cartAddress;

                // 切bank
                bank = romAddress >> 14;

                if (bank != currentBank)
                {
                    currentBank = bank;
                    mbc5_romSwitchBank(bank);
                    //printLog(string.Format("    切换至 bank{0:d}", bank));
                }

                if (bank == 0)
                    cartAddress = 0x0000 + (romAddress & 0x3fff);
                else
                    cartAddress = 0x4000 + (romAddress & 0x3fff);

                // 写入
                gbcCart_romProgram((UInt32)cartAddress, sendPack, (UInt16)bufferWriteBytes);

                writtenCount += sentLen;
                showProgress(writtenCount, fileLength);
            }
            stopwatch.Stop();

            port.Close();
            enableButton();

            printScore(fileLength, stopwatch.ElapsedMilliseconds);
        }

        void mission_dumpRom_mbc5()
        {
            string romFilePath = textBox_romPath.Text; // 打开文件

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

            int fileLength = (int)(double.Parse(comboBox_romSize_mbc5.Text) * 1024 * 1024);
            byte[] rom = new byte[fileLength];


            // 获取rom flash buffer大小
            int deviceSize, secotrCount, sectorSize, bufferWriteBytes;
            mbc5_romGetSize(out secotrCount, out sectorSize, out bufferWriteBytes, out deviceSize);


            // 获取工作区间
            int addrBegin, addrEnd;
            int gameSelect = comboBox_mbc5MultiCartSelect.SelectedIndex;
            if (gameSelect != 0)
            {
                addrBegin = (int)mbc5_multiCardAddrRange_rom[gameSelect, 0];
                addrEnd = (int)mbc5_multiCardAddrRange_rom[gameSelect, 1];
            }
            else
            {
                addrBegin = 0;
                addrEnd = fileLength - 1;
            }

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

            int currentBank = -123;
            int readCount = 0;

            while (readCount < fileLength)
            {
                // 分包
                int readLen = fileLength - readCount;
                readLen = readLen > 4096 ? 4096 : readLen;

                int romAddress = addrBegin + readCount;
                int cartAddress;

                // 切bank 
                int bank = romAddress >> 14;

                if (bank != currentBank)
                {
                    currentBank = bank;
                    mbc5_romSwitchBank(bank);
                    //printLog(string.Format("    切换至 bank{0:d}", bank));
                }

                if (bank == 0)
                    cartAddress = 0x0000 + (romAddress & 0x3fff);
                else
                    cartAddress = 0x4000 + (romAddress & 0x3fff);

                // 读取
                byte[] respon = new byte[readLen];
                gbcCart_read((UInt32)cartAddress, ref respon);

                Array.Copy(respon, 0, rom, readCount, readLen);

                readCount += readLen;
                showProgress(readCount, fileLength);
            }
            stopwatch.Stop();

            // 保存

            file.Write(rom, 0, fileLength);
            file.Close();

            port.Close();
            enableButton();

            printScore(fileLength, stopwatch.ElapsedMilliseconds);
        }

        void mission_verifyRom_mbc5()
        {
            // 打开文件
            string romFilePath = textBox_romPath.Text;

            FileStream file;
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

            byte[] rom = new byte[fileLength];
            file.Read(rom, 0, fileLength);
            file.Close();

            // 获取工作区间
            int addrBegin, addrEnd;
            int gameSelect = comboBox_mbc5MultiCartSelect.SelectedIndex;
            if (gameSelect != 0)
            {
                addrBegin = (int)mbc5_multiCardAddrRange_rom[gameSelect, 0];
                addrEnd = (int)mbc5_multiCardAddrRange_rom[gameSelect, 1];
            }
            else
            {
                addrBegin = 0;
                addrEnd = fileLength - 1;
            }

            // 开始校验
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();

            int currentBank = -123;
            int readCount = 0;

            while (readCount < fileLength)
            {
                // 分包
                int readLen = fileLength - readCount;
                readLen = readLen > 4096 ? 4096 : readLen;

                int romAddress = addrBegin + readCount;
                int cartAddress;

                // 切bank
                int bank = romAddress >> 14;

                if (bank != currentBank)
                {
                    currentBank = bank;
                    mbc5_romSwitchBank(bank);
                    //printLog(string.Format("    切换至 bank{0:d}", bank));
                }

                if (bank == 0)
                    cartAddress = 0x0000 + (romAddress & 0x3fff);
                else
                    cartAddress = 0x4000 + (romAddress & 0x3fff);

                // 读取
                byte[] respon = new byte[readLen];
                gbcCart_read((UInt32)cartAddress, ref respon);

                // 对比
                for (int i = 0; i < readLen; i++)
                {
                    if (rom[readCount + i] != respon[i])
                    {
                        printLog(string.Format(
                            "0x{0:x8}校验失败，{1:x2} -> {2:x2}",
                            romAddress + i,
                            rom[readCount + i],
                            respon[i]
                        ));
                    }
                }
                readCount += readLen;
                showProgress(readCount, fileLength);
            }
            stopwatch.Stop();

            port.Close();
            enableButton();

            printScore(fileLength, stopwatch.ElapsedMilliseconds);
        }

        void mission_wrtieRam_mbc5()
        {
            // 打开文件
            string savFilePath = textBox_savePath.Text;

            FileStream file;
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

            // 获取工作区间
            int addrBegin, addrEnd;
            int gameSelect = comboBox_mbc5MultiCartSelect.SelectedIndex;
            if (gameSelect != 0)
            {
                addrBegin = (int)mbc5_multiCardAddrRange_ram[gameSelect, 0];
                addrEnd = (int)mbc5_multiCardAddrRange_ram[gameSelect, 1];
            }
            else
            {
                addrBegin = 0;
                addrEnd = fileLength - 1;
            }

            // 开启ram访问权限
            gbcCart_write(0x0000, new byte[] { 0x0a });

            // 开始发送
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();

            int currentBank = -123;
            int writtenCount = 0;

            while (writtenCount < fileLength)
            {
                // 分包
                int sentLen = fileLength - writtenCount;
                sentLen = sentLen > 4096 ? 4096 : sentLen;

                byte[] sendPack = new byte[sentLen];
                Array.Copy(sav, writtenCount, sendPack, 0, sentLen);

                int ramAddress = addrBegin + writtenCount;
                int cartAddress;

                // 切bank
                int bank = ramAddress >> 13;

                int b = bank < 0 ? 0 : bank;
                if (b != currentBank)
                {
                    currentBank = b;
                    mbc5_ramSwitchBank(b);
                    //printLog(string.Format("    切换至 bank{0:d}", bank > 0 ? bank : 0));
                }

                cartAddress = 0xa000 + (ramAddress & 0x1fff);

                // 写入
                gbcCart_write((UInt32)cartAddress, sendPack);

                writtenCount += sentLen;
                showProgress(writtenCount, fileLength);
            }
            stopwatch.Stop();

            port.Close();
            enableButton();

            printScore(fileLength, stopwatch.ElapsedMilliseconds);
        }

        void mission_dumpRam_mbc5()
        {
            string savFilePath = textBox_savePath.Text; // 打开文件
            FileStream file;
            try
            {
                file = new FileStream(savFilePath, FileMode.Create, FileAccess.Write);
            }
            catch
            {
                printLog("文件被占用");
                port.Close();
                enableButton();
                return;
            }

            int fileLength = (int)(double.Parse(comboBox_saveSize_mbc5.Text) * 1024);
            byte[] sav = new byte[fileLength];

            // 获取工作区间
            int addrBegin, addrEnd;
            int gameSelect = comboBox_mbc5MultiCartSelect.SelectedIndex;
            if (gameSelect != 0)
            {
                addrBegin = (int)mbc5_multiCardAddrRange_ram[gameSelect, 0];
                addrEnd = (int)mbc5_multiCardAddrRange_ram[gameSelect, 1];
            }
            else
            {
                addrBegin = 0;
                addrEnd = fileLength - 1;
            }

            // 开启ram访问权限
            gbcCart_write(0x0000, new byte[] { 0x0a });

            // 开始导出
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();

            int currentBank = -123;
            int readCount = 0;

            while (readCount < fileLength)
            {
                // 分包
                int readLen = fileLength - readCount;
                readLen = readLen > 4096 ? 4096 : readLen;

                int ramAddress = addrBegin + readCount;
                int cartAddress;

                // 切bank
                int bank = ramAddress >> 13;

                int b = bank < 0 ? 0 : bank;
                if (b != currentBank)
                {
                    currentBank = b;
                    mbc5_ramSwitchBank(b);
                    //printLog(string.Format("    切换至 bank{0:d}", bank > 0 ? bank : 0));
                }

                cartAddress = 0xa000 + (ramAddress & 0x1fff);

                // 读取
                byte[] respon = new byte[readLen];
                gbcCart_read((UInt32)cartAddress, ref respon);

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

        void mission_verifyRam_mbc5()
        {
            // 打开文件
            string savFilePath = textBox_savePath.Text;

            FileStream file;
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

            // 获取工作区间
            int addrBegin, addrEnd;
            int gameSelect = comboBox_mbc5MultiCartSelect.SelectedIndex;
            if (gameSelect != 0)
            {
                addrBegin = (int)mbc5_multiCardAddrRange_ram[gameSelect, 0];
                addrEnd = (int)mbc5_multiCardAddrRange_ram[gameSelect, 1];
            }
            else
            {
                addrBegin = 0;
                addrEnd = fileLength - 1;
            }

            // 开启ram访问权限
            gbcCart_write(0x0000, new byte[] { 0x0a });

            // 开始校验
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();

            int currentBank = -123;
            int readCount = 0;

            while (readCount < fileLength)
            {
                // 分包
                int readLen = fileLength - readCount;
                readLen = readLen > 4096 ? 4096 : readLen;

                int ramAddress = addrBegin + readCount;
                int cartAddress;

                // 切bank
                int bank = ramAddress >> 13;

                int b = bank < 0 ? 0 : bank;
                if (b != currentBank)
                {
                    currentBank = b;
                    mbc5_ramSwitchBank(b);
                    //printLog(string.Format("    切换至 bank{0:d}", bank > 0 ? bank : 0));
                }

                cartAddress = 0xa000 + (ramAddress & 0x1fff);

                // 读取
                byte[] respon = new byte[readLen];
                gbcCart_read((UInt32)cartAddress, ref respon);

                // 对比
                for (int i = 0; i < readLen; i++)
                {
                    if (sav[readCount + i] != respon[i])
                    {
                        printLog(string.Format(
                            "0x{0:x8}校验失败，{1:x2} -> {2:x2}",
                            ramAddress + i,
                            sav[readCount + i],
                            respon[i]
                        ));
                    }
                }
                readCount += readLen;
                showProgress(readCount, fileLength);
            }
            stopwatch.Stop();

            port.Close();
            enableButton();

            printScore(fileLength, stopwatch.ElapsedMilliseconds);
        }

    }
}
