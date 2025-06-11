using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using System.IO;
using System.IO.Ports;
using Microsoft.Win32;
using System.Management;
using System.Collections;
using System.Threading;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.StartPanel;

namespace ChisFlashBurner
{
    public partial class Form1 : Form
    {
        Thread thread;
        SerialPort port;

        Dictionary<string, string> serialPorts = new Dictionary<string, string>();

        public Form1()
        {
            InitializeComponent();

            label_progress.Text = "";
            label_speed.Text = "";

            // 搜索串口
            btn_renewPort_Click(null, null);

            CheckForIllegalCrossThreadCalls = false;

        }

        // 搜索串口
        private void btn_renewPort_Click(object sender, EventArgs e)
        {
            //搜索串口
            comboBox_com.Items.Clear();
            serialPorts.Clear();

            int targetPortIndex = -1;

            using (var searcher = new ManagementObjectSearcher("SELECT * FROM Win32_PnPEntity WHERE Caption LIKE '%(COM%'"))
            {
                foreach (var device in searcher.Get())
                {
                    string caption = device["Caption"]?.ToString();
                    string deviceId = device["DeviceID"]?.ToString();

                    if (string.IsNullOrEmpty(caption) || !caption.Contains("(COM"))
                        continue;

                    // 提取COM端口名，例如 "COM3"
                    int start = caption.IndexOf("(COM") + 1;
                    int end = caption.IndexOf(")", start);
                    if (start < 0 || end < 0)
                        continue;

                    // 提取端口号
                    string comPort = caption.Substring(start, end - start);
                    comboBox_com.Items.Add(caption);
                    serialPorts.Add(caption, comPort);

                    // 提取 VID 和 PID（如果有）
                    string vid = null, pid = null;
                    if (deviceId != null && deviceId.Contains("VID_") && deviceId.Contains("PID_"))
                    {
                        vid = deviceId.Substring(deviceId.IndexOf("VID_") + 4, 4);
                        pid = deviceId.Substring(deviceId.IndexOf("PID_") + 4, 4);

                        if (vid == "0483" && pid == "0721")
                        {
                            targetPortIndex = comboBox_com.Items.Count - 1;
                        }

                    }
                }
            }

            if (targetPortIndex != -1)
            {
                comboBox_com.SelectedIndex = targetPortIndex;
            }

            textBox_log.Clear();
        }

        // 浏览rom文件路径
        private void btn_browseRom_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "rom files (*.gb;*.gbc;*.gba)|*.gb;*.gbc;*.gba|All files (*.*)|*.*";
            ofd.Multiselect = false;
            ofd.CheckFileExists = false;
            ofd.CheckPathExists = false;
            ofd.FilterIndex = 0;

            if (ofd.ShowDialog() == DialogResult.OK)
            {
                string filePath = ofd.FileName;
                string extension = Path.GetExtension(filePath);

                textBox_romPath.Text = filePath;

                // 获取文件大小
                double fileSizeInMiB;
                FileInfo fileInfo = new FileInfo(filePath);
                if (File.Exists(filePath))
                    fileSizeInMiB = (double)(fileInfo.Length) / 1024.0 / 1024.0;
                else
                    fileSizeInMiB = 0.0;

                if (extension.ToLower() == ".gba")
                {
                    if (fileSizeInMiB > 0)
                        comboBox_romSize.Text = fileSizeInMiB.ToString("f3");
                    else
                        comboBox_romSize.Text = "32";
                    tabControl1.SelectTab(0);
                    comboBox_ramType.SelectedIndex = 1;
                    comboBox_gbaMultiCartSelect.SelectedIndex = 0;
                }
                else if (extension.ToLower() == ".gbc" || extension.ToLower() == ".gb")
                {
                    if (fileSizeInMiB > 0)
                        comboBox_romSize_mbc5.Text = fileSizeInMiB.ToString("f3");
                    else
                        comboBox_romSize_mbc5.Text = "8";
                    tabControl1.SelectTab(1);
                    comboBox_mbc5MultiCartSelect.SelectedIndex = 0;
                }


            }

        }

        // 浏览存档文件路径
        private void btn_browseSave_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "save files (*.sav)|*.sav|All files (*.*)|*.*";
            ofd.Multiselect = false;
            ofd.CheckFileExists = false;
            ofd.CheckPathExists = false;
            ofd.FilterIndex = 0;

            if (ofd.ShowDialog() == DialogResult.OK)
            {
                string filePath = ofd.FileName;

                textBox_savePath.Text = filePath;

                // 获取文件大小
                double sizeInKib = 0.0;
                FileInfo fileInfo = new FileInfo(filePath);
                if (fileInfo.Exists)
                {
                    sizeInKib = (double)(fileInfo.Length) / 1024.0;
                }


                if (sizeInKib > 0)
                {
                    comboBox_saveSize.Text = sizeInKib.ToString("f3");
                    comboBox_saveSize_mbc5.Text = sizeInKib.ToString("f3");
                }
                else
                {
                    comboBox_saveSize.Text = "128";
                    comboBox_saveSize_mbc5.Text = "128";
                }


                comboBox_ramType.SelectedIndex = 1;
                comboBox_mbc5MultiCartSelect.SelectedIndex = 0;

            }
        }

        // 读取ID
        private void btn_readID_Click(object sender, EventArgs e)
        {
            if (!openPort())
                return;

            disableButton(true);

            if (tabControl1.SelectedTab.Text == "GBA")
                thread = new Thread(new ThreadStart(mission_readRomID));
            else if (tabControl1.SelectedTab.Text == "MBC5")
                thread = new Thread(new ThreadStart(mission_readRomID_mbc5));

            thread.Start();
        }

        // 全片擦除
        private void btn_eraseChip_Click(object sender, EventArgs e)
        {
            if (btn_eraseChip.Text == "全片擦除")
            {
                if (!openPort())
                    return;

                printLog("开始擦除");

                disableButton(false);

                thread = new Thread(new ThreadStart(mission_eraseChip));
                thread.Start();
            }
            else // 取消任务
            {

                port.DiscardOutBuffer();
                port.DiscardInBuffer();

                Thread.Sleep(123);

                if (thread != null && thread.IsAlive)
                    thread.Abort();
                if (port != null && port.IsOpen)
                    port.Close();

                enableButton();

                printLog("已取消");
            }
        }

        // 写入rom
        private void btn_writeRom_Click(object sender, EventArgs e)
        {
            if (!File.Exists(textBox_romPath.Text))
            {
                printLog("没找到文件");
                return;
            }

            if (!openPort())
                return;

            disableButton(true);

            thread = new Thread(new ThreadStart(mission_programRom));
            thread.Start();
        }

        // 导出rom
        private void btn_readRom_Click(object sender, EventArgs e)
        {
            try
            {
                double _ = double.Parse(comboBox_romSize.Text);
            }
            catch
            {
                printLog("rom大小有问题");
                return;
            }

            if (textBox_romPath.Text == "")
            {
                printLog("文件路径为空");
                return;
            }

            if (!openPort())
                return;

            printLog("开导");

            disableButton(true);

            thread = new Thread(new ThreadStart(mission_dumpRom));
            thread.Start();
        }

        // 校验rom
        private void btn_verifyRom_Click(object sender, EventArgs e)
        {
            if (!File.Exists(textBox_romPath.Text))
            {
                printLog("没找到文件");
                return;
            }

            if (!openPort())
                return;

            printLog("开始校验");

            disableButton(true);

            thread = new Thread(new ThreadStart(mission_verifyRom));
            thread.Start();
        }

        // 写入ram
        private void btn_writeSave_Click(object sender, EventArgs e)
        {
            if (!File.Exists(textBox_savePath.Text))
            {
                printLog("没找到文件");
                return;
            }

            if (!openPort())
                return;

            printLog("开始写入");

            disableButton(true);

            thread = new Thread(new ThreadStart(mission_wrtieSram));

            thread.Start();
        }

        // 导出ram
        private void btn_readSave_Click(object sender, EventArgs e)
        {
            try
            {
                double _ = double.Parse(comboBox_saveSize.Text);
            }
            catch
            {
                printLog("存档大小有问题");
                return;
            }

            if (textBox_savePath.Text == "")
            {
                printLog("文件路径为空");
                return;
            }

            if (!openPort())
                return;

            printLog("开导");

            disableButton(true);

            thread = new Thread(new ThreadStart(mission_dumpRam));

            thread.Start();
        }

        // 校验ram
        private void btn_verifySave_Click(object sender, EventArgs e)
        {
            if (!File.Exists(textBox_savePath.Text))
            {
                printLog("没找到文件");
                return;
            }

            if (!openPort())
                return;

            printLog("开始校验");

            disableButton(true);

            thread = new Thread(new ThreadStart(mission_verifyRam));

            thread.Start();
        }


        //////////////////////////////////////////////////////
        /// 下面是mbc5的功能
        /// 这泡屎，算是越拉越大了
        ///

        // mbc5 全片擦除
        private void btn_eraseChip_mbc5_Click(object sender, EventArgs e)
        {

            if (btn_eraseChip.Text == "全片擦除")
            {
                if (!openPort())
                    return;

                printLog("开始擦除");

                disableButton(false);

                thread = new Thread(new ThreadStart(mission_eraseChip_mbc5));
                thread.Start();
            }
            else // 取消任务
            {

                port.DiscardOutBuffer();
                port.DiscardInBuffer();

                Thread.Sleep(123);

                if (thread != null && thread.IsAlive)
                    thread.Abort();
                if (port != null && port.IsOpen)
                    port.Close();

                enableButton();

                printLog("已取消");
            }
        }

        // mbc5 写入rom
        private void btn_writeRom_mbc5_Click(object sender, EventArgs e)
        {

            if (!File.Exists(textBox_romPath.Text))
            {
                printLog("没找到文件");
                return;
            }

            if (!openPort())
                return;

            disableButton(true);

            thread = new Thread(new ThreadStart(mission_programRom_mbc5));
            thread.Start();
        }

        // mbc5 导出rom
        private void btn_readRom_mbc5_Click(object sender, EventArgs e)
        {
            try
            {
                double _ = double.Parse(comboBox_romSize_mbc5.Text);
            }
            catch
            {
                printLog("rom大小有问题");
                return;
            }

            if (textBox_romPath.Text == "")
            {
                printLog("文件路径为空");
                return;
            }

            if (!openPort())
                return;

            printLog("开导");

            disableButton(true);

            thread = new Thread(new ThreadStart(mission_dumpRom_mbc5));
            thread.Start();
        }

        // mbc5 校验rom
        private void btn_verifyRom_mbc5_Click(object sender, EventArgs e)
        {

            if (!File.Exists(textBox_romPath.Text))
            {
                printLog("没找到文件");
                return;
            }

            if (!openPort())
                return;

            printLog("开始校验");

            disableButton(true);

            thread = new Thread(new ThreadStart(mission_verifyRom_mbc5));
            thread.Start();
        }

        // mbc5 写入ram
        private void btn_writeSave_mbc5_Click(object sender, EventArgs e)
        {
            if (!File.Exists(textBox_savePath.Text))
            {
                printLog("没找到文件");
                return;
            }

            if (!openPort())
                return;

            printLog("开始写入");

            disableButton(true);

            thread = new Thread(new ThreadStart(mission_wrtieRam_mbc5));

            thread.Start();
        }


        // mbc5 导出ram
        private void btn_readSave_mbc5_Click(object sender, EventArgs e)
        {
            try
            {
                double _ = double.Parse(comboBox_saveSize_mbc5.Text);
            }
            catch
            {
                printLog("存档大小有问题");
                return;
            }

            if (textBox_savePath.Text == "")
            {
                printLog("文件路径为空");
                return;
            }

            if (!openPort())
                return;

            printLog("开导");

            disableButton(true);

            thread = new Thread(new ThreadStart(mission_dumpRam_mbc5));

            thread.Start();
        }

        // mbc5 校验ram
        private void btn_verifySave_mbc5_Click(object sender, EventArgs e)
        {
            if (!File.Exists(textBox_savePath.Text))
            {
                printLog("没找到文件");
                return;
            }

            if (!openPort())
                return;

            printLog("开始校验");

            disableButton(true);

            thread = new Thread(new ThreadStart(mission_verifyRam_mbc5));

            thread.Start();
        }

        private void comboBox_mbc5MultiCartSelect_SelectedIndexChanged(object sender, EventArgs e)
        {
            int i = comboBox_mbc5MultiCartSelect.SelectedIndex;

            if (i == 1 || i == 2)
            {
                comboBox_romSize_mbc5.Text = "1.0";
                comboBox_saveSize_mbc5.Text = "32.0";
            }
            else if (i > 2)
            {
                comboBox_romSize_mbc5.Text = "2.0";
                comboBox_saveSize_mbc5.Text = "32.0";
            }
        }
    }
}
