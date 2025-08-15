using System;
using System.Collections.Generic;
using System.Diagnostics;
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

    }
}
