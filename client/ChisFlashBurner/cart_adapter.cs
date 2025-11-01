using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;
using System.Threading;

namespace ChisFlashBurner
{
    public partial class Form1
    {
        // 计算crc16
        public UInt16 ModbusCRC16(byte[] bytes)
        {
            UInt16 crc = 0xFFFF;

            foreach (byte b in bytes)
            {
                crc = (ushort)(crc ^ b);

                for (int i = 0; i < 8; i++)
                {
                    UInt16 temp = (ushort)(crc & 1);
                    crc = (ushort)(crc >> 1);

                    if (temp == 1)
                        crc = (ushort)(crc ^ 0xa001);

                    crc = (ushort)(crc & 0xFFFF);
                }
            }

            return crc;
        }

        void sendPackage(byte[] bytes)
        {
            int size = 2 + bytes.Length + 2;

            byte[] buf = new byte[size];

            // 包大小 2字节
            Array.Copy(
                BitConverter.GetBytes((UInt16)size), 0,
                buf, 0,
                2
            );
            // 数据
            Array.Copy(
                bytes, 0,
                buf, 2,
                bytes.Length
            );
            //// crc 2字节
            //UInt16 crc = ModbusCRC16(buf.Take(size - 2).ToArray());
            //Array.Copy(
            //    BitConverter.GetBytes(crc), 0,
            //    buf, size - 2,
            //    2
            //);

            try
            {
                port.Write(buf, 0, size);
                showSpeed(bytes.Length);
            }
            catch { return; }
        }

        bool getRespon(ref byte[] respon)
        {
            int expectedCount = respon.Length + 2;

            byte[] buf = new byte[expectedCount];

            int receivedCount = 0;
            while (receivedCount < expectedCount)
            {
                int bytesToRead = port.BytesToRead;

                if (bytesToRead > 0)
                {
                    if (bytesToRead + receivedCount > expectedCount)
                        bytesToRead = expectedCount - receivedCount;

                    try
                    {
                        port.Read(buf, receivedCount, bytesToRead);
                    }
                    catch { return false; }

                    receivedCount += bytesToRead;
                }
            }
            //port.DiscardInBuffer();

            //UInt16 packCrc = BitConverter.ToUInt16(buf, 0);
            //UInt16 crc = ModbusCRC16(buf.Skip(2).Take(expectedCount - 2).ToArray());

            //if (crc == packCrc)
            if (true)
            {
                Array.Copy(buf, 2, respon, 0, respon.Length);
                showSpeed(respon.Length);
                return true;
            }
            else
            {
                return false;
            }
        }

        bool getRespon()
        {
            while (port.BytesToRead == 0) ;

            byte[] ack = new byte[1];

            try
            {
                port.Read(ack, 0, 1);
                port.DiscardInBuffer();
            }
            catch { return false; }

            if (ack[0] == 0xaa)
                return true;
            else
                return false;
        }

        bool rom_readID(ref byte[] id)
        {
            // sent read id cmd
            sendPackage(new byte[] { 0xf0 });

            //get respon
            byte[] respon = new byte[8];
            bool ack = getRespon(ref respon);

            if (ack)
            {
                Array.Copy(respon, 0, id, 0, 8);

                return true;
            }
            else
            {
                return false;
            }
        }

        bool rom_eraseChip()
        {
            // sent erase chip cmd
            sendPackage(new byte[] { 0xf1 });

            //get respon
            bool ack = getRespon();
            return ack;
        }

        bool rom_write(UInt32 wordAddr, byte[] bytes)
        {
            byte[] baseAddress = BitConverter.GetBytes(wordAddr);
            int sentLen = bytes.Length;

            byte[] pack = new byte[1 + 4 + sentLen];

            pack[0] = 0xf5;                         // rom program cmd
            Array.Copy(baseAddress, 0, pack, 1, 4); // base address in word
            Array.Copy(bytes, 0, pack, 5, sentLen); // data

            //发送
            sendPackage(pack);

            // 等待响应
            bool ack = getRespon();

            return ack;
        }

        bool rom_program(UInt32 addr, byte[] bytes, UInt16 bufferWriteBytes)
        {
            byte[] baseAddress = BitConverter.GetBytes(addr);
            byte[] buffWr = BitConverter.GetBytes(bufferWriteBytes);

            int sentLen = bytes.Length;

            byte[] pack = new byte[1 + 4 + 2 + sentLen];

            pack[0] = 0xf4;                         // rom program cmd
            Array.Copy(baseAddress, 0, pack, 1, 4); // base address
            Array.Copy(buffWr, 0, pack, 5, 2);      // bufferWriteBytes
            Array.Copy(bytes, 0, pack, 7, sentLen); // data

            //发送
            sendPackage(pack);

            // 等待响应
            bool ack = getRespon();

            return ack;
        }

        bool rom_read(UInt32 addr, ref byte[] bytes)
        {
            // rom Read cmd
            byte[] baseAddress = BitConverter.GetBytes(addr);
            byte[] readlen = BitConverter.GetBytes((UInt16)(bytes.Length));

            byte[] pack = new byte[1 + 4 + 2];

            pack[0] = 0xf6;
            Array.Copy(baseAddress, 0, pack, 1, 4);
            Array.Copy(readlen, 0, pack, 5, 2);

            //发送
            sendPackage(pack);

            // 等待响应
            bool valid = getRespon(ref bytes);

            return valid;
        }

        bool ram_write(UInt32 addr, byte[] bytes)
        {
            byte[] baseAddress = BitConverter.GetBytes(addr);
            int sentLen = bytes.Length;

            byte[] pack = new byte[1 + 4 + sentLen];

            pack[0] = 0xf7; // ram Write cmd
            Array.Copy(baseAddress, 0, pack, 1, 4); // base address
            Array.Copy(bytes, 0, pack, 5, sentLen); // data

            //发送
            sendPackage(pack);

            // 等待响应
            bool valid = getRespon();

            return valid;
        }

        bool ram_read(UInt32 addr, ref byte[] bytes)
        {

            byte[] baseAddress = BitConverter.GetBytes(addr);
            byte[] readlen = BitConverter.GetBytes((UInt16)(bytes.Length));

            byte[] pack = new byte[1 + 4 + 2];
            pack[0] = 0xf8; // ram Read cmd
            Array.Copy(baseAddress, 0, pack, 1, 4);
            Array.Copy(readlen, 0, pack, 5, 2);

            // 发送
            sendPackage(pack);

            // 等待响应
            bool valid = getRespon(ref bytes);

            return valid;
        }

        bool ram_flashProgram(UInt32 addr, byte[] bytes)
        {
            byte[] baseAddress = BitConverter.GetBytes(addr);
            int sentLen = bytes.Length;

            byte[] pack = new byte[1 + 4 + sentLen];

            pack[0] = 0xf9; // ram program flash cmd
            Array.Copy(baseAddress, 0, pack, 1, 4); // base address
            Array.Copy(bytes, 0, pack, 5, sentLen); // data

            //发送
            sendPackage(pack);

            // 等待响应
            bool valid = getRespon();

            return valid;
        }


        bool ram_write_forFram(UInt32 addr, byte[] bytes, byte latency)
        {
            byte[] baseAddress = BitConverter.GetBytes(addr);
            int sentLen = bytes.Length;

            byte[] pack = new byte[1 + 4 + 1 + sentLen];

            pack[0] = 0xe7;
            Array.Copy(baseAddress, 0, pack, 1, 4);
            pack[5] = latency;
            Array.Copy(bytes, 0, pack, 6, sentLen);

            //发送
            sendPackage(pack);

            // 等待响应
            bool valid = getRespon();
            return valid;
        }

        bool ram_read_forFram(UInt32 addr, ref byte[] bytes, byte latency)
        {
            byte[] baseAddress = BitConverter.GetBytes(addr);
            byte[] readlen = BitConverter.GetBytes((UInt16)(bytes.Length));

            byte[] pack = new byte[1 + 4 + 1 + 2];

            pack[0] = 0xe8;
            Array.Copy(baseAddress, 0, pack, 1, 4);
            Array.Copy(readlen, 0, pack, 5, 2);
            pack[7] = latency;

            // 发送
            sendPackage(pack);
            // 等待响应
            bool valid = getRespon(ref bytes);
            return valid;
        }


        ////////////////////////////////////////////////////////////
        /// 下面是gbc的功能

        bool gbcCart_write(UInt32 addr, byte[] bytes)
        {
            byte[] baseAddress = BitConverter.GetBytes(addr);
            int sentLen = bytes.Length;

            byte[] pack = new byte[1 + 4 + sentLen];

            pack[0] = 0xfa;                         // gbc write cmd
            Array.Copy(baseAddress, 0, pack, 1, 4); // base address
            Array.Copy(bytes, 0, pack, 5, sentLen); // data

            //发送
            sendPackage(pack);

            // 等待响应
            bool ack = getRespon();

            return ack;
        }


        bool gbcCart_read(UInt32 addr, ref byte[] bytes)
        {
            byte[] baseAddress = BitConverter.GetBytes(addr);
            byte[] readlen = BitConverter.GetBytes((UInt16)(bytes.Length));

            byte[] pack = new byte[1 + 4 + 2];
            pack[0] = 0xfb;                           // gbc write cmd
            Array.Copy(baseAddress, 0, pack, 1, 4);   // base address
            Array.Copy(readlen, 0, pack, 5, 2); // read length

            // 发送
            sendPackage(pack);

            // 等待响应
            bool valid = getRespon(ref bytes);

            return valid;
        }

        bool gbcCart_romProgram(UInt32 addr, byte[] bytes, UInt16 bufferWriteBytes)
        {
            byte[] baseAddress = BitConverter.GetBytes(addr);
            byte[] buffWr = BitConverter.GetBytes(bufferWriteBytes);
            int sentLen = bytes.Length;

            byte[] pack = new byte[1 + 4 + 2 + sentLen];

            pack[0] = 0xfc;                         // gbc rom program cmd
            Array.Copy(baseAddress, 0, pack, 1, 4); // base address
            Array.Copy(buffWr, 0, pack, 5, 2);      // bufferWriteBytes
            Array.Copy(bytes, 0, pack, 7, sentLen); // data

            //发送
            sendPackage(pack);

            // 等待响应
            bool ack = getRespon();

            return ack;
        }

        bool gbcCart_write_forFram(UInt32 addr, byte[] bytes, byte latency)
        {
            byte[] baseAddress = BitConverter.GetBytes(addr);
            int sentLen = bytes.Length;

            byte[] pack = new byte[1 + 4 + 1 + sentLen];

            pack[0] = 0xea;
            Array.Copy(baseAddress, 0, pack, 1, 4);
            pack[5] = latency;
            Array.Copy(bytes, 0, pack, 6, sentLen);

            //发送
            sendPackage(pack);

            // 等待响应
            bool valid = getRespon();
            return valid;
        }

        bool gbcCart_read_forFram(uint addr, ref byte[] bytes, byte latency)
        {
            byte[] baseAddress = BitConverter.GetBytes(addr);
            byte[] readlen = BitConverter.GetBytes((UInt16)(bytes.Length));

            byte[] pack = new byte[1 + 4 + 1 + 2];

            pack[0] = 0xeb;
            Array.Copy(baseAddress, 0, pack, 1, 4);
            Array.Copy(readlen, 0, pack, 5, 2);
            pack[7] = latency;

            // 发送
            sendPackage(pack);

            // 等待响应
            bool valid = getRespon(ref bytes);
            return valid;
        }

        /////////////////////////////////////////////
        /// 碳酸丐专属

        void cart_power(bool en, bool _5v)
        {
            byte[] pack = new byte[2];

            pack[0] = 0xa0;

            if (!en)
                pack[1] = 0; // 断电
            else if (_5v)
                pack[1] = 2; // 5v
            else
                pack[1] = 1; // 3.3v

            sendPackage(pack);

            Thread.Sleep(10);
            port.RtsEnable = true;
            port.DtrEnable = true;
            port.RtsEnable = false;
            port.DtrEnable = false;
            Thread.Sleep(10);
        }

        void cart_phiDiv(int div)
        {
            byte[] pack = new byte[2];

            pack[0] = 0xa1;
            pack[1] = (byte)(div & 0x7f);

            sendPackage(pack);

            Thread.Sleep(10);
            port.RtsEnable = true;
            port.DtrEnable = true;
            port.RtsEnable = false;
            port.DtrEnable = false;
        }
    }
}
