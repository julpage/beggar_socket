using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ChisFlashBurner
{
    public partial class Form1
    {
        // 打印日志
        public void printLog(string s)
        {
            string timeStamp = DateTime.Now.ToString("[HH:mm:ss.ff]: ");
            string msg = timeStamp + s + "\r\n";

            textBox_log.AppendText(msg);
            textBox_log.ScrollToCaret();
        }

        public void printScore(int fileLength, long ms)
        {
            float second = ms / 1000.0f;

            printLog(
                string.Format(
                    "传输完成, 耗时: {0:f3} s, 平均速度: {1:f3} KiB/s",
                    second,
                    fileLength / second / 1024.0f
                )
            );
        }

        // 更新进度条
        public void showProgress(int value, int max)
        {
            if (progressBar_total.Maximum != max)
                progressBar_total.Maximum = max;
            if (value > max)
                value = max;

            progressBar_total.Value = value;
            label_progress.Text = string.Format("{0:d}/{1:d}", value, max);
        }

        // 打开串口
        bool openPort()
        {
            string deviceName = comboBox_com.Text;
            if (deviceName == "")
            {
                printLog("没有串口");
                return false;
            }

            string portName = serialPorts[comboBox_com.Text];

            try
            {
                port = new SerialPort();

                port.BaudRate = 9600;
                port.DataBits = 8;
                port.Parity = Parity.None;
                port.StopBits = StopBits.One;
                port.PortName = portName;
                port.Open();
                port.RtsEnable = true;
                port.DtrEnable = true;
                port.RtsEnable = false;
                port.DtrEnable = false;

                return true;
            }
            catch
            {
                printLog("串口打不开");
                port = null;
                return false;
            }
        }

        // 禁用按钮
        void disableButton(bool abortable)
        {
            btn_renewPort.Enabled = false;
            btn_readID.Enabled = false;
            btn_writeRom.Enabled = false;
            btn_readRom.Enabled = false;
            btn_verifyRom.Enabled = false;
            btn_writeSave.Enabled = false;
            btn_readSave.Enabled = false;
            btn_verifySave.Enabled = false;

            btn_writeRom_mbc5.Enabled = false;
            btn_readRom_mbc5.Enabled = false;
            btn_verifyRom_mbc5.Enabled = false;
            btn_writeSave_mbc5.Enabled = false;
            btn_readSave_mbc5.Enabled = false;
            btn_verifySave_mbc5.Enabled = false;

            btn_unlockPPB_gba.Enabled = false;
            btn_unlockPPB_mbc5.Enabled = false;
            btn_setRTC_gba.Enabled = false;
            btn_setRTC_mbc.Enabled = false;
            btn_rumbleTest_gba.Enabled = false;
            btn_cancel.Enabled = true;

            if (abortable)
            {
                btn_eraseChip.Text = "取消";
                btn_eraseChip_mbc5.Text = "取消";
            }
            else
            {
                btn_eraseChip.Enabled = false;
                btn_eraseChip_mbc5.Enabled = false;
            }

            checkBox_mbc5V.Enabled = false;
        }

        // 启用按钮
        void enableButton()
        {
            btn_renewPort.Enabled = true;
            btn_readID.Enabled = true;
            btn_writeRom.Enabled = true;
            btn_readRom.Enabled = true;
            btn_verifyRom.Enabled = true;
            btn_writeSave.Enabled = true;
            btn_readSave.Enabled = true;
            btn_verifySave.Enabled = true;

            btn_writeRom_mbc5.Enabled = true;
            btn_readRom_mbc5.Enabled = true;
            btn_verifyRom_mbc5.Enabled = true;
            btn_writeSave_mbc5.Enabled = true;
            btn_readSave_mbc5.Enabled = true;
            btn_verifySave_mbc5.Enabled = true;

            btn_unlockPPB_gba.Enabled = true;
            btn_unlockPPB_mbc5.Enabled = true;
            btn_setRTC_gba.Enabled = true;
            btn_setRTC_mbc.Enabled = true;
            btn_rumbleTest_gba.Enabled = true;
            btn_cancel.Enabled = false;

            btn_eraseChip.Enabled = true;
            btn_eraseChip.Text = "全片擦除";

            btn_eraseChip_mbc5.Enabled = true;
            btn_eraseChip_mbc5.Text = "全片擦除";

            checkBox_mbc5V.Enabled = true;
        }

        // 计算传输速度
        public int transmittedBytes = 0;
        public void showSpeed(int bs)
        {
            transmittedBytes += bs;
        }
        public void tmr_showSpeed_Tick(object sender, EventArgs e)
        {
            int b = transmittedBytes;
            transmittedBytes = 0;

            if (b == 0)
            {
                label_speed.Text = "";
                return;
            }

            string[] unitNameLst = { "B", "KiB", "MiB", "GiB" };

            double speed = b / (double)(tmr_showSpeed.Interval) * 1000;

            int p = (int)Math.Log(speed, 1024);

            label_speed.Text = string.Format("{0:f3} {1:s}/s",
                                               speed / Math.Pow(1024, p),
                                               unitNameLst[p]);
        }


        public bool isBlank(byte[] bytes)
        {
            foreach (byte b in bytes)
            {
                if (b != 0xFF)
                    return false;
            }
            return true;
        }


        int mbcTypeDetect()
        {
            byte[] id_mbc1 = { 0x01, 0x02, 0x03 };
            byte[] id_mbc2 = { 0x05, 0x06 };
            byte[] id_mbc3 = { 0x0f, 0x10, 0x11, 0x12, 0x13 };
            byte[] id_mbc5 = { 0x19, 0x1a, 0x1b, 0x1c, 0x1d, 0x1e };

            // 打开文件
            string romFilePath = textBox_romPath.Text;
            FileStream fs;
            byte[] rom = new byte[0x150];

            try
            {
                fs = new FileStream(romFilePath, FileMode.Open, FileAccess.Read);
                if (fs.Length < 0x150)
                {
                    fs.Close();
                    return 0;
                }

                fs.Read(rom, 0, 0x150);
                fs.Close();
            }
            catch (System.IO.IOException)
            { return 0; }

            //
            byte cartType = rom[0x147];
            if (id_mbc5.Contains(cartType))
                return 5;
            else if (id_mbc3.Contains(cartType))
                return 3;
            else if (id_mbc2.Contains(cartType))
                return 2;
            else if (id_mbc1.Contains(cartType))
                return 1;
            else
                return 5;
        }

        int mbcTypeSelected()
        {
            string t = comboBox_mbcType.Text;

            return int.Parse(t.Substring(3,1));
        }
    }
}
