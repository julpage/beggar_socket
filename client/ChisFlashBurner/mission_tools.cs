using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Rebar;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.TaskbarClock;

namespace ChisFlashBurner
{
    public partial class Form1
    {
        void mission_unlockPPB_gba()
        {
            int height = this.Size.Height < 700 ? 700 : this.Size.Height;
            int width = this.Size.Width < 800 ? 800 : this.Size.Width;
            this.Size = new System.Drawing.Size(width, height);

            printLog("解锁PPB");

            // reset
            rom_write(0, BitConverter.GetBytes((UInt16)0x90));
            rom_write(0, BitConverter.GetBytes((UInt16)0x00)); // Command Set Exit
            rom_write(0, BitConverter.GetBytes((UInt16)0xf0)); // Reset/ASO Exit

            // 获取nor信息
            int deviceSize, sectorCount, sectorSize, bufferWriteBytes;
            gba_romGetSize(out sectorCount, out sectorSize, out bufferWriteBytes, out deviceSize);
            bool isMultiCard = deviceSize > (32 * 1024 * 1024);

            printLog(string.Format("容量:{0:d} 扇区数量:{1:d} 扇区大小:{2:d} BuffWr:{3:d}", deviceSize, sectorCount, sectorSize, bufferWriteBytes));

            if (deviceSize > 512 * 1024 * 1024)
            {
                printLog("NOR flash识别异常");
                port.Close();
                enableButton();
                return;
            }


            // Global Non-Volatile Sector Protection Freeze Command Set Definitions
            rom_write(0x000555, BitConverter.GetBytes((UInt16)0xaa));
            rom_write(0x0002aa, BitConverter.GetBytes((UInt16)0x55));
            rom_write(0x000555, BitConverter.GetBytes((UInt16)0x50));

            byte[] lockBit = new byte[2];
            rom_read(0, ref lockBit);

            // reset
            rom_write(0, BitConverter.GetBytes((UInt16)0x90));
            rom_write(0, BitConverter.GetBytes((UInt16)0x00)); // Command Set Exit
            rom_write(0, BitConverter.GetBytes((UInt16)0xf0)); // Reset/ASO Exit

            printLog(string.Format("PPB Lock Status: 0x{0:x4}", BitConverter.ToUInt16(lockBit, 0)));
            if (lockBit[0] != 1)
            {
                printLog("无法解锁");
                port.Close();
                enableButton();
                return;
            }


            // 检查ppb

            int currentBank = -1;
            UInt16 ppb;
            string str = "";
            bool needToUnlock = false;

            for (int i = 0; i < sectorCount; i++)
            {
                // 切bank
                if (isMultiCard)
                {
                    int bank = i * sectorSize / (32 * 1024 * 1024);
                    if (bank != currentBank)
                    {
                        gba_romSwitchBank(bank);
                        currentBank = bank;
                    }
                }

                // Non-Volatile Sector Protection Command Set Definitions
                rom_write(0x000555, BitConverter.GetBytes((UInt16)0xaa));
                rom_write(0x0002aa, BitConverter.GetBytes((UInt16)0x55));
                rom_write(0x000555, BitConverter.GetBytes((UInt16)0xc0));
                rom_read((UInt32)(i * sectorSize), ref lockBit);
                // reset
                rom_write(0, BitConverter.GetBytes((UInt16)0x90));
                rom_write(0, BitConverter.GetBytes((UInt16)0x00)); // Command Set Exit
                rom_write(0, BitConverter.GetBytes((UInt16)0xf0)); // Reset/ASO Exit

                ppb = BitConverter.ToUInt16(lockBit, 0);

                if (ppb != 1)
                    needToUnlock = true;

                if (i != 0 && (i % 16 == 0))
                {
                    printLog(str);
                    str = "";
                }

                str += string.Format("{0:x4}  ", ppb);
            }
            printLog(str);

            if (!needToUnlock)
            {
                DialogResult result = MessageBox.Show(
                    this,
                    "无需解锁，再解1次？", "  ",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question,
                    MessageBoxDefaultButton.Button2
                );

                if (result == DialogResult.No)
                {
                    printLog("已取消");
                    port.Close();
                    enableButton();
                    return;
                }
            }

            // All PPB Erase
            printLog("---- All PPB Erase ----");
            // Non-Volatile Sector Protection Command Set Definitions
            rom_write(0x000555, BitConverter.GetBytes((UInt16)0xaa));
            rom_write(0x0002aa, BitConverter.GetBytes((UInt16)0x55));
            rom_write(0x000555, BitConverter.GetBytes((UInt16)0xc0));
            rom_write(0, BitConverter.GetBytes((UInt16)0x80));
            rom_write(0, BitConverter.GetBytes((UInt16)0x30)); // All PPB Erase
            Thread.Sleep(2000);
            rom_write(0, BitConverter.GetBytes((UInt16)0x90));
            rom_write(0, BitConverter.GetBytes((UInt16)0x00)); // Command Set Exit
            rom_write(0, BitConverter.GetBytes((UInt16)0xf0)); // Reset/ASO Exit


            // 再次打印ppb

            str = "";
            for (int i = 0; i < sectorCount; i++)
            {
                // 切bank
                if (isMultiCard)
                {
                    int bank = i * sectorSize / (32 * 1024 * 1024);
                    if (bank != currentBank)
                    {
                        gba_romSwitchBank(bank);
                        currentBank = bank;
                    }
                }

                // Non-Volatile Sector Protection Command Set Definitions
                rom_write(0x000555, BitConverter.GetBytes((UInt16)0xaa));
                rom_write(0x0002aa, BitConverter.GetBytes((UInt16)0x55));
                rom_write(0x000555, BitConverter.GetBytes((UInt16)0xc0));
                rom_read((UInt32)(i * sectorSize), ref lockBit);
                // reset
                rom_write(0, BitConverter.GetBytes((UInt16)0x90));
                rom_write(0, BitConverter.GetBytes((UInt16)0x00)); // Command Set Exit
                rom_write(0, BitConverter.GetBytes((UInt16)0xf0)); // Reset/ASO Exit

                ppb = BitConverter.ToUInt16(lockBit, 0);

                if (i != 0 && (i % 16 == 0))
                {
                    printLog(str);
                    str = "";
                }

                str += string.Format("{0:x4}  ", ppb);
            }
            printLog(str);


            printLog("-----------------------");
            port.Close();
            enableButton();
        }


        void mission_unlockPPB_mbc5()
        {
            int height = this.Size.Height < 700 ? 700 : this.Size.Height;
            int width = this.Size.Width < 800 ? 800 : this.Size.Width;
            this.Size = new System.Drawing.Size(width, height);

            printLog("解锁PPB");
            mbc_romSwitchBank(mbcTypeSelected(), 0);

            // reset
            gbcCart_write(0, new byte[] { 0x90 });
            gbcCart_write(0, new byte[] { 0x00 }); // Command Set Exit
            gbcCart_write(0, new byte[] { 0xf0 }); // Reset/ASO Exit

            // 获取nor信息
            int deviceSize, sectorCount, sectorSize, bufferWriteBytes;
            mbc5_romGetSize(out sectorCount, out sectorSize, out bufferWriteBytes, out deviceSize);

            printLog(string.Format("容量:{0:d} 扇区数量:{1:d} 扇区大小:{2:d} BuffWr:{3:d}", deviceSize, sectorCount, sectorSize, bufferWriteBytes));


            // Global Non-Volatile Sector Protection Freeze Command Set Definitions
            gbcCart_write(0xaaa, new byte[] { 0xaa });
            gbcCart_write(0x555, new byte[] { 0x55 });
            gbcCart_write(0xaaa, new byte[] { 0x50 });

            byte[] lockBit = new byte[1];
            gbcCart_read(0, ref lockBit);

            // reset
            gbcCart_write(0, new byte[] { 0x90 });
            gbcCart_write(0, new byte[] { 0x00 }); // Command Set Exit
            gbcCart_write(0, new byte[] { 0xf0 }); // Reset/ASO Exit

            printLog(string.Format("PPB Lock Status: 0x{0:x2}", lockBit[0]));
            if (lockBit[0] != 1)
            {
                printLog("无法解锁");
                port.Close();
                enableButton();
                return;
            }


            // 检查ppb

            int currentBank = -1;
            int cartAddress;
            string str = "";
            bool needUnlock = false;

            for (int i = 0; i < sectorCount; i++)
            {
                int bank = (i * sectorSize) >> 14;
                if (bank != currentBank)
                {
                    currentBank = bank;
                    mbc_romSwitchBank(mbcTypeSelected(), bank);
                }

                if (bank == 0)
                    cartAddress = 0x0000 + ((i * sectorSize) & 0x3fff);
                else
                    cartAddress = 0x4000 + ((i * sectorSize) & 0x3fff);

                // Non-Volatile Sector Protection Command Set Definitions
                gbcCart_write(0xaaa, new byte[] { 0xaa });
                gbcCart_write(0x555, new byte[] { 0x55 });
                gbcCart_write(0xaaa, new byte[] { 0xc0 });
                gbcCart_read((UInt32)cartAddress, ref lockBit);
                // reset
                gbcCart_write(0, new byte[] { 0x90 });
                gbcCart_write(0, new byte[] { 0x00 }); // Command Set Exit
                gbcCart_write(0, new byte[] { 0xf0 }); // Reset/ASO Exit

                if (i != 0 && (i % 16 == 0))
                {
                    printLog(str);
                    str = "";
                }

                if (lockBit[0] != 1)
                    needUnlock = true;

                str += string.Format("{0:x2}  ", lockBit[0]);
            }
            printLog(str);


            if (!needUnlock)
            {
                DialogResult result = MessageBox.Show(
                    this,
                    "无需解锁，再解1次？", "  ",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question,
                    MessageBoxDefaultButton.Button2
                );

                if (result == DialogResult.No)
                {
                    printLog("已取消");
                    port.Close();
                    enableButton();
                    return;
                }
            }

            // All PPB Erase
            printLog("---- All PPB Erase ----");
            // Non-Volatile Sector Protection Command Set Definitions
            gbcCart_write(0xaaa, new byte[] { 0xaa });
            gbcCart_write(0x555, new byte[] { 0x55 });
            gbcCart_write(0xaaa, new byte[] { 0xc0 });
            gbcCart_write(0, new byte[] { 0x80 });
            gbcCart_write(0, new byte[] { 0x30 }); // All PPB Erase
            Thread.Sleep(2000);
            gbcCart_write(0, new byte[] { 0x90 });
            gbcCart_write(0, new byte[] { 0x00 }); // Command Set Exit
            gbcCart_write(0, new byte[] { 0xf0 }); // Reset/ASO Exit


            // 再次打印ppb
            str = "";
            for (int i = 0; i < sectorCount; i++)
            {
                int bank = (i * sectorSize) >> 14;
                if (bank != currentBank)
                {
                    currentBank = bank;
                    mbc_romSwitchBank(mbcTypeSelected(), bank);
                }

                if (bank == 0)
                    cartAddress = 0x0000 + ((i * sectorSize) & 0x3fff);
                else
                    cartAddress = 0x4000 + ((i * sectorSize) & 0x3fff);

                // Non-Volatile Sector Protection Command Set Definitions
                gbcCart_write(0xaaa, new byte[] { 0xaa });
                gbcCart_write(0x555, new byte[] { 0x55 });
                gbcCart_write(0xaaa, new byte[] { 0xc0 });
                gbcCart_read((UInt32)cartAddress, ref lockBit);
                // reset
                gbcCart_write(0, new byte[] { 0x90 });
                gbcCart_write(0, new byte[] { 0x00 }); // Command Set Exit
                gbcCart_write(0, new byte[] { 0xf0 }); // Reset/ASO Exit

                if (i != 0 && (i % 16 == 0))
                {
                    printLog(str);
                    str = "";
                }

                str += string.Format("{0:x2}  ", lockBit[0]);
            }
            printLog(str);

            mbc_romSwitchBank(mbcTypeSelected(), 0);
            printLog("-----------------------");
            port.Close();
            enableButton();
        }


        byte s3511_readByte()
        {
            byte value = 0;
            byte[] a = new byte[2];

            rom_write(0xc6 >> 1, BitConverter.GetBytes((UInt16)5)); // sio in

            for (int i = 0; i < 8; i++)
            {
                rom_write(0xc4 >> 1, BitConverter.GetBytes((UInt16)4)); // cs 1, sck 0
                rom_write(0xc4 >> 1, BitConverter.GetBytes((UInt16)5)); // cs 1, sck 1
                rom_read(0xc4, ref a);

                // lsb in
                value >>= 1;
                if ((a[0] & 0x02) != 0)
                    value |= 0x80;
            }

            return value;
        }

        void s3511_writeByte(int value)
        {
            rom_write(0xc6 >> 1, BitConverter.GetBytes((UInt16)7)); // sio out

            UInt16 bit;
            for (int i = 0; i < 8; i++)
            {
                if ((value & 0x01) == 0)
                    bit = 0x00;
                else
                    bit = 0x02;
                value >>= 1;

                rom_write(0xc4 >> 1, BitConverter.GetBytes((UInt16)(4 | bit))); // cs 1, sck 0
                rom_write(0xc4 >> 1, BitConverter.GetBytes((UInt16)(5 | bit))); // cs 1, sck 1
            }
        }


        void mission_setRTC_gba()
        {
            printLog("设置RTC");

            // 检测gpio功能
            byte[] read1 = new byte[6];
            byte[] read2 = new byte[6];

            rom_read(0xc4, ref read1);
            rom_write(0xc8 >> 1, BitConverter.GetBytes((UInt16)(0x01))); // enable gpio
            rom_read(0xc4, ref read2);
            rom_write(0xc8 >> 1, BitConverter.GetBytes((UInt16)(0x00))); // diable gpio
            printLog(
                string.Format("{0:s} <-> {1:s}",
                BitConverter.ToString(read1).Replace("-", " "),
                BitConverter.ToString(read2).Replace("-", " ")
            ));

            if (read1.SequenceEqual(read2))
            {
                printLog("卡带无gpio功能");
                port.Close();
                enableButton();
                return;
            }


            rom_write(0xc4 >> 1, BitConverter.GetBytes((UInt16)1)); // cs 0, sck 1
            rom_write(0xc6 >> 1, BitConverter.GetBytes((UInt16)7)); // cs sio sck output
            rom_write(0xc8 >> 1, BitConverter.GetBytes((UInt16)(0x01))); // enable gpio


            // 读取RTC状态
            s3511_writeByte(0xc6);
            byte status = s3511_readByte();
            rom_write(0xc4 >> 1, BitConverter.GetBytes((UInt16)1)); // cs 0, sck 1

            printLog(string.Format("RTC Status: 0x{0:x2}", status));



            // 断电重置
            if ((status & 0x80) != 0)
            {
                printLog("电池没电，重置");

                s3511_writeByte(0x06); // reset
                rom_write(0xc4 >> 1, BitConverter.GetBytes((UInt16)1)); // cs 0, sck 1

                s3511_writeByte(0x46); // write status
                s3511_writeByte(0x40); // 24 hour mode
                rom_write(0xc4 >> 1, BitConverter.GetBytes((UInt16)1)); // cs 0, sck 1

                s3511_writeByte(0xc6);
                status = s3511_readByte();
                rom_write(0xc4 >> 1, BitConverter.GetBytes((UInt16)1)); // cs 0, sck 1
                printLog(string.Format("RTC Status: 0x{0:x2}", status));
            }



            // 读取当前时间
            s3511_writeByte(0xa6);
            byte year = s3511_readByte();
            byte month = (byte)(s3511_readByte() & 0x1f);
            byte date = (byte)(s3511_readByte() & 0x3f);
            byte day = (byte)(s3511_readByte() & 0x07);
            byte hour = (byte)(s3511_readByte() & 0x3f);
            byte minute = (byte)(s3511_readByte() & 0x7f);
            byte second = (byte)(s3511_readByte() & 0x7f);
            rom_write(0xc4 >> 1, BitConverter.GetBytes((UInt16)1)); // cs 0, sck 1

            printLog(string.Format("卡带时间: {0:x2}-{1:x2}-{2:x2} {3:x2}:{4:x2}:{5:x2} 星期{6:x}",
                year, month, date, hour, minute, second, day));



            // 设置时间
            Form_gba_rtc f = new Form_gba_rtc();
            f.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            f.ShowDialog(this);

            if (!f.isSet)
            {
                printLog("取消设置");
                rom_write(0xc8 >> 1, BitConverter.GetBytes((UInt16)(0x00))); // diable gpio
                port.Close();
                enableButton();
                return;
            }

            year = f.year;
            month = f.month;
            date = f.date;
            day = f.day;
            hour = f.hour;
            minute = f.minute;
            second = f.second;

            printLog(string.Format("设置为: {0:x2}-{1:x2}-{2:x2} {3:x2}:{4:x2}:{5:x2} 星期{6:x}",
                year, month, date, hour, minute, second, day));

            s3511_writeByte(0x26);
            s3511_writeByte(year);
            s3511_writeByte(month);
            s3511_writeByte(date);
            s3511_writeByte(day);
            s3511_writeByte(hour);
            s3511_writeByte(minute);
            s3511_writeByte(second);
            rom_write(0xc4 >> 1, BitConverter.GetBytes((UInt16)1)); // cs 0, sck 1

            Thread.Sleep(1000);

            // 重新获取
            for (int i = 5; i > 0; i--)
            {
                rom_write(0xc8 >> 1, BitConverter.GetBytes((UInt16)(0x01))); // enable gpio
                s3511_writeByte(0xa6);
                year = s3511_readByte();
                month = (byte)(s3511_readByte() & 0x1f);
                date = (byte)(s3511_readByte() & 0x3f);
                day = (byte)(s3511_readByte() & 0x07);
                hour = (byte)(s3511_readByte() & 0x3f);
                minute = (byte)(s3511_readByte() & 0x7f);
                second = (byte)(s3511_readByte() & 0x7f);
                rom_write(0xc4 >> 1, BitConverter.GetBytes((UInt16)1)); // cs 0, sck 1
                rom_write(0xc8 >> 1, BitConverter.GetBytes((UInt16)(0x00))); // diable gpio

                printLog(string.Format("{0:d} 卡带时间: {1:x2}-{2:x2}-{3:x2} {4:x2}:{5:x2}:{6:x2} 星期{7:x}",
                    i, year, month, date, hour, minute, second, day));
                Thread.Sleep(1000);
            }


            printLog("-----------------------");
            port.Close();
            enableButton();
        }


        void mission_setRTC_mbc3()
        {
            printLog("设置RTC");



            // 读取当前时间
            byte[] a = new byte[1];
            List<byte> buffer = new List<byte>();

            gbcCart_write(0x0000, new byte[] { 0x0a }); // EnableRAM
            gbcCart_write(0x6000, new byte[] { 0x01 }); // 锁存时间
            for (int i = 0x08; i <= 0x0D; i++)
            {
                // Register  Name    Description                                           Range
                // $08       RTC S   Seconds 0-59                                         ($00-$3B)
                // $09       RTC M   Minutes 0-59                                         ($00-$3B)
                // $0A       RTC H   Hours 0-23                                           ($00-$17)
                // $0B       RTC DL  Lower 8 bits of Day Counter                          ($00-$FF)
                // $0C       RTC DH  Bit 0: Most significant bit (Bit 8) of Day Counter
                //                   Bit 6: Halt (0=Active, 1=Stop Timer)
                //                   Bit 7: Day Counter Carry Bit (1=Counter Overflow)

                gbcCart_write(0x4000, new byte[] { (byte)i });
                gbcCart_read(0xa000, ref a);
                buffer.Add(a[0]);
            }
            gbcCart_write(0x6000, new byte[] { 0x00 }); // 解锁

            byte day = (byte)((buffer[4] & 0x01) << 8 | buffer[3]);
            byte hour = buffer[2];
            byte minute = buffer[1];
            byte second = buffer[0];

            printLog(string.Format("卡带时间: {0:d}日 {1:d}:{2:d}:{3:d}", day, hour, minute, second));



            // 设置时间
            Form_mbc3_rtc f = new Form_mbc3_rtc();
            f.StartPosition = FormStartPosition.CenterParent;
            f.ShowDialog(this);

            if (!f.isSet)
            {
                printLog("取消设置");
                port.Close();
                enableButton();
                return;
            }

            byte[] b = new byte[]{
                (byte)f.second,
                (byte)f.minute,
                (byte)f.hour,
                (byte)(f.day & 0xff),
                (byte)((f.hour & 0x100) >> 8)
            };

            printLog(string.Format("设置为: {0:d}日 {1:d}:{2:d}:{3:d}", f.day, f.hour, f.minute, f.second));

            gbcCart_write(0x0000, new byte[] { 0x0a }); // EnableRAM
            gbcCart_write(0x6000, new byte[] { 0x00 });
            gbcCart_write(0x6000, new byte[] { 0x01 }); // 锁存时间
            gbcCart_write(0x4000, new byte[] { 0x0c }); // RTC DH
            gbcCart_write(0xa000, new byte[] { 0x40 }); // Bit 6: Halt

            for (int i = 0x08; i <= 0x0D; i++)
            {
                gbcCart_write(0x4000, new byte[] { (byte)i });
                var v = b.Skip(i - 8).Take(1).ToArray();
                gbcCart_write(0xa000, v);
            }

            gbcCart_write(0x6000, new byte[] { 0 });
            gbcCart_write(0x6000, new byte[] { 1 });
            gbcCart_write(0x4000, new byte[] { 0 });
            gbcCart_write(0x0000, new byte[] { 0 });
            Thread.Sleep(100);
            gbcCart_write(0x6000, new byte[] { 0 });
            Thread.Sleep(100);



            // 重新获取
            gbcCart_write(0x0000, new byte[] { 0x0a }); // EnableRAM
            for (int ii = 5; ii > 0; ii--)
            {
                buffer.Clear();
                gbcCart_write(0x6000, new byte[] { 0x01 }); // 锁存时间
                for (int i = 0x08; i <= 0x0D; i++)
                {
                    gbcCart_write(0x4000, new byte[] { (byte)i });
                    gbcCart_read(0xa000, ref a);
                    buffer.Add(a[0]);
                }
                gbcCart_write(0x6000, new byte[] { 0x00 }); // 解锁

                day = (byte)((buffer[4] & 0x01) << 8 | buffer[3]);
                hour = buffer[2];
                minute = buffer[1];
                second = buffer[0];

                printLog(string.Format("{0:d} 卡带时间: {1:d}日 {2:d}:{3:d}:{4:d}", ii, day, hour, minute, second));

                Thread.Sleep(1000);
            }

            printLog("-----------------------");
            port.Close();
            enableButton();
        }

        void mission_rumbleTest_gba()
        {
            printLog("震动测试");

            printLog("GPIO  指令");
            rom_write(0xc8 >> 1, BitConverter.GetBytes((UInt16)(0x01))); // enable gpio
            rom_write(0xc6 >> 1, BitConverter.GetBytes((UInt16)(0x08))); // gpio3 output
            rom_write(0xc4 >> 1, BitConverter.GetBytes((UInt16)(0x08))); // gpio3 1
            Thread.Sleep(500);
            rom_write(0xc4 >> 1, BitConverter.GetBytes((UInt16)(0x00))); // gpio3 0
            rom_write(0xc8 >> 1, BitConverter.GetBytes((UInt16)(0x00))); // diable gpio
            Thread.Sleep(250);

            printLog("EZODE 指令");
            for (int i = 0; i < 10; i++)
            {
                rom_write(0xff0000, BitConverter.GetBytes((UInt16)(0xd200)));
                rom_write(0x000000, BitConverter.GetBytes((UInt16)(0x1500)));
                rom_write(0x010000, BitConverter.GetBytes((UInt16)(0xd200)));
                rom_write(0x020000, BitConverter.GetBytes((UInt16)(0x1500)));
                rom_write(0xf10000, BitConverter.GetBytes((UInt16)(0xf1)));
                rom_write(0xfe0000, BitConverter.GetBytes((UInt16)(0x1500)));
                rom_write(0x000800, BitConverter.GetBytes((UInt16)(0x02)));
                rom_write(0x000800, BitConverter.GetBytes((UInt16)(0x00)));
                Thread.Sleep(50);
            }

            printLog("-------------------");
            port.Close();
            enableButton();
        }



    }
}
