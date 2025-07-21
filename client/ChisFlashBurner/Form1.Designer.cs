namespace ChisFlashBurner
{
    partial class Form1
    {
        /// <summary>
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows 窗体设计器生成的代码

        /// <summary>
        /// 设计器支持所需的方法 - 不要修改
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.btn_browseRom = new System.Windows.Forms.Button();
            this.textBox_romPath = new System.Windows.Forms.TextBox();
            this.comboBox_com = new System.Windows.Forms.ComboBox();
            this.btn_readID = new System.Windows.Forms.Button();
            this.textBox_log = new System.Windows.Forms.TextBox();
            this.progressBar_total = new System.Windows.Forms.ProgressBar();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.label_progress = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStripStatusLabel2 = new System.Windows.Forms.ToolStripStatusLabel();
            this.label_speed = new System.Windows.Forms.ToolStripStatusLabel();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.textBox_savePath = new System.Windows.Forms.TextBox();
            this.btn_browseSave = new System.Windows.Forms.Button();
            this.btn_eraseChip = new System.Windows.Forms.Button();
            this.btn_writeRom = new System.Windows.Forms.Button();
            this.btn_readRom = new System.Windows.Forms.Button();
            this.btn_verifyRom = new System.Windows.Forms.Button();
            this.btn_writeSave = new System.Windows.Forms.Button();
            this.btn_readSave = new System.Windows.Forms.Button();
            this.btn_verifySave = new System.Windows.Forms.Button();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.comboBox_saveSize = new System.Windows.Forms.ComboBox();
            this.comboBox_romSize = new System.Windows.Forms.ComboBox();
            this.tmr_showSpeed = new System.Windows.Forms.Timer(this.components);
            this.btn_renewPort = new System.Windows.Forms.Button();
            this.comboBox_ramType = new System.Windows.Forms.ComboBox();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.comboBox_gbaMultiCartSelect = new System.Windows.Forms.ComboBox();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.btn_eraseChip_mbc5 = new System.Windows.Forms.Button();
            this.comboBox_mbc5MultiCartSelect = new System.Windows.Forms.ComboBox();
            this.btn_verifyRom_mbc5 = new System.Windows.Forms.Button();
            this.btn_verifySave_mbc5 = new System.Windows.Forms.Button();
            this.comboBox_saveSize_mbc5 = new System.Windows.Forms.ComboBox();
            this.comboBox_romSize_mbc5 = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.btn_readRom_mbc5 = new System.Windows.Forms.Button();
            this.label6 = new System.Windows.Forms.Label();
            this.btn_readSave_mbc5 = new System.Windows.Forms.Button();
            this.btn_writeRom_mbc5 = new System.Windows.Forms.Button();
            this.btn_writeSave_mbc5 = new System.Windows.Forms.Button();
            this.statusStrip1.SuspendLayout();
            this.tabControl1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.tabPage2.SuspendLayout();
            this.SuspendLayout();
            // 
            // btn_browseRom
            // 
            this.btn_browseRom.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btn_browseRom.Location = new System.Drawing.Point(397, 10);
            this.btn_browseRom.Name = "btn_browseRom";
            this.btn_browseRom.Size = new System.Drawing.Size(60, 23);
            this.btn_browseRom.TabIndex = 0;
            this.btn_browseRom.Text = "浏览";
            this.btn_browseRom.UseVisualStyleBackColor = true;
            this.btn_browseRom.Click += new System.EventHandler(this.btn_browseRom_Click);
            // 
            // textBox_romPath
            // 
            this.textBox_romPath.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBox_romPath.Location = new System.Drawing.Point(46, 12);
            this.textBox_romPath.Name = "textBox_romPath";
            this.textBox_romPath.Size = new System.Drawing.Size(345, 21);
            this.textBox_romPath.TabIndex = 1;
            // 
            // comboBox_com
            // 
            this.comboBox_com.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.comboBox_com.BackColor = System.Drawing.SystemColors.Window;
            this.comboBox_com.DropDownWidth = 130;
            this.comboBox_com.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.comboBox_com.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.comboBox_com.ForeColor = System.Drawing.SystemColors.InfoText;
            this.comboBox_com.FormattingEnabled = true;
            this.comboBox_com.Location = new System.Drawing.Point(463, 12);
            this.comboBox_com.Name = "comboBox_com";
            this.comboBox_com.Size = new System.Drawing.Size(123, 20);
            this.comboBox_com.TabIndex = 2;
            // 
            // btn_readID
            // 
            this.btn_readID.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btn_readID.Location = new System.Drawing.Point(526, 39);
            this.btn_readID.Name = "btn_readID";
            this.btn_readID.Size = new System.Drawing.Size(60, 23);
            this.btn_readID.TabIndex = 3;
            this.btn_readID.Text = "读取ID";
            this.btn_readID.UseVisualStyleBackColor = true;
            this.btn_readID.Click += new System.EventHandler(this.btn_readID_Click);
            // 
            // textBox_log
            // 
            this.textBox_log.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBox_log.BackColor = System.Drawing.SystemColors.Window;
            this.textBox_log.Location = new System.Drawing.Point(12, 172);
            this.textBox_log.Multiline = true;
            this.textBox_log.Name = "textBox_log";
            this.textBox_log.ReadOnly = true;
            this.textBox_log.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.textBox_log.Size = new System.Drawing.Size(574, 243);
            this.textBox_log.TabIndex = 4;
            // 
            // progressBar_total
            // 
            this.progressBar_total.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.progressBar_total.Location = new System.Drawing.Point(12, 421);
            this.progressBar_total.Name = "progressBar_total";
            this.progressBar_total.Size = new System.Drawing.Size(574, 23);
            this.progressBar_total.TabIndex = 7;
            // 
            // statusStrip1
            // 
            this.statusStrip1.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.label_progress,
            this.toolStripStatusLabel2,
            this.label_speed});
            this.statusStrip1.Location = new System.Drawing.Point(0, 451);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(598, 22);
            this.statusStrip1.TabIndex = 10;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // label_progress
            // 
            this.label_progress.Name = "label_progress";
            this.label_progress.Size = new System.Drawing.Size(131, 17);
            this.label_progress.Text = "toolStripStatusLabel1";
            // 
            // toolStripStatusLabel2
            // 
            this.toolStripStatusLabel2.Name = "toolStripStatusLabel2";
            this.toolStripStatusLabel2.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.toolStripStatusLabel2.Size = new System.Drawing.Size(439, 17);
            this.toolStripStatusLabel2.Spring = true;
            // 
            // label_speed
            // 
            this.label_speed.Name = "label_speed";
            this.label_speed.Size = new System.Drawing.Size(13, 17);
            this.label_speed.Text = "_";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(10, 14);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(23, 12);
            this.label2.TabIndex = 11;
            this.label2.Text = "ROM";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(11, 42);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(29, 12);
            this.label3.TabIndex = 12;
            this.label3.Text = "存档";
            // 
            // textBox_savePath
            // 
            this.textBox_savePath.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBox_savePath.Location = new System.Drawing.Point(46, 39);
            this.textBox_savePath.Name = "textBox_savePath";
            this.textBox_savePath.Size = new System.Drawing.Size(345, 21);
            this.textBox_savePath.TabIndex = 13;
            // 
            // btn_browseSave
            // 
            this.btn_browseSave.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btn_browseSave.Location = new System.Drawing.Point(397, 38);
            this.btn_browseSave.Name = "btn_browseSave";
            this.btn_browseSave.Size = new System.Drawing.Size(60, 23);
            this.btn_browseSave.TabIndex = 14;
            this.btn_browseSave.Text = "浏览";
            this.btn_browseSave.UseVisualStyleBackColor = true;
            this.btn_browseSave.Click += new System.EventHandler(this.btn_browseSave_Click);
            // 
            // btn_eraseChip
            // 
            this.btn_eraseChip.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btn_eraseChip.Location = new System.Drawing.Point(485, 11);
            this.btn_eraseChip.Name = "btn_eraseChip";
            this.btn_eraseChip.Size = new System.Drawing.Size(75, 52);
            this.btn_eraseChip.TabIndex = 15;
            this.btn_eraseChip.Text = "全片擦除";
            this.btn_eraseChip.UseVisualStyleBackColor = true;
            this.btn_eraseChip.Click += new System.EventHandler(this.btn_eraseChip_Click);
            // 
            // btn_writeRom
            // 
            this.btn_writeRom.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btn_writeRom.Location = new System.Drawing.Point(242, 11);
            this.btn_writeRom.Name = "btn_writeRom";
            this.btn_writeRom.Size = new System.Drawing.Size(75, 23);
            this.btn_writeRom.TabIndex = 16;
            this.btn_writeRom.Text = "写入ROM";
            this.btn_writeRom.UseVisualStyleBackColor = true;
            this.btn_writeRom.Click += new System.EventHandler(this.btn_writeRom_Click);
            // 
            // btn_readRom
            // 
            this.btn_readRom.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btn_readRom.Location = new System.Drawing.Point(323, 11);
            this.btn_readRom.Name = "btn_readRom";
            this.btn_readRom.Size = new System.Drawing.Size(75, 23);
            this.btn_readRom.TabIndex = 17;
            this.btn_readRom.Text = "导出ROM";
            this.btn_readRom.UseVisualStyleBackColor = true;
            this.btn_readRom.Click += new System.EventHandler(this.btn_readRom_Click);
            // 
            // btn_verifyRom
            // 
            this.btn_verifyRom.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btn_verifyRom.Location = new System.Drawing.Point(404, 11);
            this.btn_verifyRom.Name = "btn_verifyRom";
            this.btn_verifyRom.Size = new System.Drawing.Size(75, 23);
            this.btn_verifyRom.TabIndex = 18;
            this.btn_verifyRom.Text = "校验ROM";
            this.btn_verifyRom.UseVisualStyleBackColor = true;
            this.btn_verifyRom.Click += new System.EventHandler(this.btn_verifyRom_Click);
            // 
            // btn_writeSave
            // 
            this.btn_writeSave.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btn_writeSave.Location = new System.Drawing.Point(242, 40);
            this.btn_writeSave.Name = "btn_writeSave";
            this.btn_writeSave.Size = new System.Drawing.Size(75, 23);
            this.btn_writeSave.TabIndex = 19;
            this.btn_writeSave.Text = "写入存档";
            this.btn_writeSave.UseVisualStyleBackColor = true;
            this.btn_writeSave.Click += new System.EventHandler(this.btn_writeSave_Click);
            // 
            // btn_readSave
            // 
            this.btn_readSave.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btn_readSave.Location = new System.Drawing.Point(323, 40);
            this.btn_readSave.Name = "btn_readSave";
            this.btn_readSave.Size = new System.Drawing.Size(75, 23);
            this.btn_readSave.TabIndex = 20;
            this.btn_readSave.Text = "导出存档";
            this.btn_readSave.UseVisualStyleBackColor = true;
            this.btn_readSave.Click += new System.EventHandler(this.btn_readSave_Click);
            // 
            // btn_verifySave
            // 
            this.btn_verifySave.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btn_verifySave.Location = new System.Drawing.Point(404, 40);
            this.btn_verifySave.Name = "btn_verifySave";
            this.btn_verifySave.Size = new System.Drawing.Size(75, 23);
            this.btn_verifySave.TabIndex = 21;
            this.btn_verifySave.Text = "校验存档";
            this.btn_verifySave.UseVisualStyleBackColor = true;
            this.btn_verifySave.Click += new System.EventHandler(this.btn_verifySave_Click);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(6, 16);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(77, 12);
            this.label4.TabIndex = 22;
            this.label4.Text = "ROM大小(MiB)";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(6, 45);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(83, 12);
            this.label5.TabIndex = 23;
            this.label5.Text = "存档大小(KiB)";
            // 
            // comboBox_saveSize
            // 
            this.comboBox_saveSize.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.comboBox_saveSize.DropDownWidth = 55;
            this.comboBox_saveSize.FormattingEnabled = true;
            this.comboBox_saveSize.Items.AddRange(new object[] {
            "64",
            "128"});
            this.comboBox_saveSize.Location = new System.Drawing.Point(89, 42);
            this.comboBox_saveSize.Name = "comboBox_saveSize";
            this.comboBox_saveSize.Size = new System.Drawing.Size(64, 20);
            this.comboBox_saveSize.TabIndex = 24;
            // 
            // comboBox_romSize
            // 
            this.comboBox_romSize.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.comboBox_romSize.FormattingEnabled = true;
            this.comboBox_romSize.Location = new System.Drawing.Point(89, 13);
            this.comboBox_romSize.Name = "comboBox_romSize";
            this.comboBox_romSize.Size = new System.Drawing.Size(64, 20);
            this.comboBox_romSize.TabIndex = 25;
            // 
            // tmr_showSpeed
            // 
            this.tmr_showSpeed.Enabled = true;
            this.tmr_showSpeed.Interval = 1500;
            this.tmr_showSpeed.Tick += new System.EventHandler(this.tmr_showSpeed_Tick);
            // 
            // btn_renewPort
            // 
            this.btn_renewPort.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btn_renewPort.Location = new System.Drawing.Point(463, 39);
            this.btn_renewPort.Name = "btn_renewPort";
            this.btn_renewPort.Size = new System.Drawing.Size(60, 23);
            this.btn_renewPort.TabIndex = 26;
            this.btn_renewPort.Text = "刷新";
            this.btn_renewPort.UseVisualStyleBackColor = true;
            this.btn_renewPort.Click += new System.EventHandler(this.btn_renewPort_Click);
            // 
            // comboBox_ramType
            // 
            this.comboBox_ramType.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.comboBox_ramType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBox_ramType.DropDownWidth = 53;
            this.comboBox_ramType.FormattingEnabled = true;
            this.comboBox_ramType.Items.AddRange(new object[] {
            "FLASH",
            "SRAM"});
            this.comboBox_ramType.Location = new System.Drawing.Point(159, 42);
            this.comboBox_ramType.Name = "comboBox_ramType";
            this.comboBox_ramType.Size = new System.Drawing.Size(77, 20);
            this.comboBox_ramType.TabIndex = 27;
            // 
            // tabControl1
            // 
            this.tabControl1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Location = new System.Drawing.Point(12, 68);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(574, 98);
            this.tabControl1.TabIndex = 28;
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.comboBox_gbaMultiCartSelect);
            this.tabPage1.Controls.Add(this.btn_eraseChip);
            this.tabPage1.Controls.Add(this.comboBox_ramType);
            this.tabPage1.Controls.Add(this.btn_verifyRom);
            this.tabPage1.Controls.Add(this.btn_verifySave);
            this.tabPage1.Controls.Add(this.comboBox_saveSize);
            this.tabPage1.Controls.Add(this.comboBox_romSize);
            this.tabPage1.Controls.Add(this.label5);
            this.tabPage1.Controls.Add(this.btn_readRom);
            this.tabPage1.Controls.Add(this.label4);
            this.tabPage1.Controls.Add(this.btn_readSave);
            this.tabPage1.Controls.Add(this.btn_writeRom);
            this.tabPage1.Controls.Add(this.btn_writeSave);
            this.tabPage1.Location = new System.Drawing.Point(4, 22);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(566, 72);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "GBA";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // comboBox_gbaMultiCartSelect
            // 
            this.comboBox_gbaMultiCartSelect.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.comboBox_gbaMultiCartSelect.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBox_gbaMultiCartSelect.DropDownWidth = 100;
            this.comboBox_gbaMultiCartSelect.FormattingEnabled = true;
            this.comboBox_gbaMultiCartSelect.Items.AddRange(new object[] {
            "整卡",
            "合卡菜单",
            "合卡8M",
            "合卡12M",
            "合卡16M",
            "合卡20M",
            "合卡24M",
            "合卡28M",
            "合卡32M",
            "合卡36M",
            "合卡40M",
            "合卡44M",
            "合卡48M",
            "合卡52M",
            "合卡56M",
            "合卡60M",
            "合卡64M",
            "合卡68M",
            "合卡72M",
            "合卡76M",
            "合卡80M",
            "合卡84M",
            "合卡88M",
            "合卡92M",
            "合卡96M",
            "合卡100M",
            "合卡104M",
            "合卡108M",
            "合卡112M",
            "合卡116M",
            "合卡120M",
            "合卡124M",
            "合卡128M",
            "合卡132M",
            "合卡136M",
            "合卡140M",
            "合卡144M",
            "合卡148M",
            "合卡152M",
            "合卡156M",
            "合卡160M",
            "合卡164M",
            "合卡168M",
            "合卡172M",
            "合卡176M",
            "合卡180M",
            "合卡184M",
            "合卡188M",
            "合卡192M",
            "合卡196M",
            "合卡200M",
            "合卡204M",
            "合卡208M",
            "合卡212M",
            "合卡216M",
            "合卡220M",
            "合卡224M",
            "合卡228M",
            "合卡232M",
            "合卡236M",
            "合卡240M",
            "合卡244M",
            "合卡248M",
            "合卡252M",
            "合卡256M",
            "合卡260M",
            "合卡264M",
            "合卡268M",
            "合卡272M",
            "合卡276M",
            "合卡280M",
            "合卡284M",
            "合卡288M",
            "合卡292M",
            "合卡296M",
            "合卡300M",
            "合卡304M",
            "合卡308M",
            "合卡312M",
            "合卡316M",
            "合卡320M",
            "合卡324M",
            "合卡328M",
            "合卡332M",
            "合卡336M",
            "合卡340M",
            "合卡344M",
            "合卡348M",
            "合卡352M",
            "合卡356M",
            "合卡360M",
            "合卡364M",
            "合卡368M",
            "合卡372M",
            "合卡376M",
            "合卡380M",
            "合卡384M",
            "合卡388M",
            "合卡392M",
            "合卡396M",
            "合卡400M",
            "合卡404M",
            "合卡408M",
            "合卡412M",
            "合卡416M",
            "合卡420M",
            "合卡424M",
            "合卡428M",
            "合卡432M",
            "合卡436M",
            "合卡440M",
            "合卡444M",
            "合卡448M",
            "合卡452M",
            "合卡456M",
            "合卡460M",
            "合卡464M",
            "合卡468M",
            "合卡472M",
            "合卡476M",
            "合卡480M",
            "合卡484M",
            "合卡488M",
            "合卡492M",
            "合卡496M",
            "合卡500M",
            "合卡504M",
            "合卡508M",
            "合卡512M"});
            this.comboBox_gbaMultiCartSelect.Location = new System.Drawing.Point(159, 13);
            this.comboBox_gbaMultiCartSelect.Name = "comboBox_gbaMultiCartSelect";
            this.comboBox_gbaMultiCartSelect.Size = new System.Drawing.Size(77, 20);
            this.comboBox_gbaMultiCartSelect.TabIndex = 40;
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.btn_eraseChip_mbc5);
            this.tabPage2.Controls.Add(this.comboBox_mbc5MultiCartSelect);
            this.tabPage2.Controls.Add(this.btn_verifyRom_mbc5);
            this.tabPage2.Controls.Add(this.btn_verifySave_mbc5);
            this.tabPage2.Controls.Add(this.comboBox_saveSize_mbc5);
            this.tabPage2.Controls.Add(this.comboBox_romSize_mbc5);
            this.tabPage2.Controls.Add(this.label1);
            this.tabPage2.Controls.Add(this.btn_readRom_mbc5);
            this.tabPage2.Controls.Add(this.label6);
            this.tabPage2.Controls.Add(this.btn_readSave_mbc5);
            this.tabPage2.Controls.Add(this.btn_writeRom_mbc5);
            this.tabPage2.Controls.Add(this.btn_writeSave_mbc5);
            this.tabPage2.Location = new System.Drawing.Point(4, 22);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(566, 72);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "MBC5";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // btn_eraseChip_mbc5
            // 
            this.btn_eraseChip_mbc5.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btn_eraseChip_mbc5.Location = new System.Drawing.Point(485, 11);
            this.btn_eraseChip_mbc5.Name = "btn_eraseChip_mbc5";
            this.btn_eraseChip_mbc5.Size = new System.Drawing.Size(75, 52);
            this.btn_eraseChip_mbc5.TabIndex = 28;
            this.btn_eraseChip_mbc5.Text = "全片擦除";
            this.btn_eraseChip_mbc5.UseVisualStyleBackColor = true;
            this.btn_eraseChip_mbc5.Click += new System.EventHandler(this.btn_eraseChip_mbc5_Click);
            // 
            // comboBox_mbc5MultiCartSelect
            // 
            this.comboBox_mbc5MultiCartSelect.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.comboBox_mbc5MultiCartSelect.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBox_mbc5MultiCartSelect.DropDownWidth = 100;
            this.comboBox_mbc5MultiCartSelect.FormattingEnabled = true;
            this.comboBox_mbc5MultiCartSelect.Items.AddRange(new object[] {
            "整卡",
            "菜单",
            "合卡 1/4",
            "合卡 2/4",
            "合卡 3/4",
            "合卡 4/4",
            "5/16  (max)",
            "6/16  (max)",
            "7/16  (max)",
            "8/16  (max)",
            "9/16  (max)",
            "10/16 (max)",
            "11/16 (max)",
            "12/16 (max)",
            "13/16 (max)",
            "14/16 (max)",
            "15/16 (max)",
            "16/16 (max)"});
            this.comboBox_mbc5MultiCartSelect.Location = new System.Drawing.Point(163, 13);
            this.comboBox_mbc5MultiCartSelect.Name = "comboBox_mbc5MultiCartSelect";
            this.comboBox_mbc5MultiCartSelect.Size = new System.Drawing.Size(73, 20);
            this.comboBox_mbc5MultiCartSelect.TabIndex = 39;
            this.comboBox_mbc5MultiCartSelect.SelectedIndexChanged += new System.EventHandler(this.comboBox_mbc5MultiCartSelect_SelectedIndexChanged);
            // 
            // btn_verifyRom_mbc5
            // 
            this.btn_verifyRom_mbc5.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btn_verifyRom_mbc5.Location = new System.Drawing.Point(404, 11);
            this.btn_verifyRom_mbc5.Name = "btn_verifyRom_mbc5";
            this.btn_verifyRom_mbc5.Size = new System.Drawing.Size(75, 23);
            this.btn_verifyRom_mbc5.TabIndex = 31;
            this.btn_verifyRom_mbc5.Text = "校验ROM";
            this.btn_verifyRom_mbc5.UseVisualStyleBackColor = true;
            this.btn_verifyRom_mbc5.Click += new System.EventHandler(this.btn_verifyRom_mbc5_Click);
            // 
            // btn_verifySave_mbc5
            // 
            this.btn_verifySave_mbc5.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btn_verifySave_mbc5.Location = new System.Drawing.Point(404, 40);
            this.btn_verifySave_mbc5.Name = "btn_verifySave_mbc5";
            this.btn_verifySave_mbc5.Size = new System.Drawing.Size(75, 23);
            this.btn_verifySave_mbc5.TabIndex = 34;
            this.btn_verifySave_mbc5.Text = "校验存档";
            this.btn_verifySave_mbc5.UseVisualStyleBackColor = true;
            this.btn_verifySave_mbc5.Click += new System.EventHandler(this.btn_verifySave_mbc5_Click);
            // 
            // comboBox_saveSize_mbc5
            // 
            this.comboBox_saveSize_mbc5.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.comboBox_saveSize_mbc5.DropDownWidth = 55;
            this.comboBox_saveSize_mbc5.FormattingEnabled = true;
            this.comboBox_saveSize_mbc5.Items.AddRange(new object[] {
            "8",
            "16",
            "32",
            "128",
            "512"});
            this.comboBox_saveSize_mbc5.Location = new System.Drawing.Point(89, 42);
            this.comboBox_saveSize_mbc5.Name = "comboBox_saveSize_mbc5";
            this.comboBox_saveSize_mbc5.Size = new System.Drawing.Size(147, 20);
            this.comboBox_saveSize_mbc5.TabIndex = 37;
            // 
            // comboBox_romSize_mbc5
            // 
            this.comboBox_romSize_mbc5.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.comboBox_romSize_mbc5.FormattingEnabled = true;
            this.comboBox_romSize_mbc5.Items.AddRange(new object[] {
            "1.0",
            "2.0",
            "8.0",
            "32.0"});
            this.comboBox_romSize_mbc5.Location = new System.Drawing.Point(89, 13);
            this.comboBox_romSize_mbc5.Name = "comboBox_romSize_mbc5";
            this.comboBox_romSize_mbc5.Size = new System.Drawing.Size(68, 20);
            this.comboBox_romSize_mbc5.TabIndex = 38;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(6, 45);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(83, 12);
            this.label1.TabIndex = 36;
            this.label1.Text = "存档大小(KiB)";
            // 
            // btn_readRom_mbc5
            // 
            this.btn_readRom_mbc5.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btn_readRom_mbc5.Location = new System.Drawing.Point(323, 11);
            this.btn_readRom_mbc5.Name = "btn_readRom_mbc5";
            this.btn_readRom_mbc5.Size = new System.Drawing.Size(75, 23);
            this.btn_readRom_mbc5.TabIndex = 30;
            this.btn_readRom_mbc5.Text = "导出ROM";
            this.btn_readRom_mbc5.UseVisualStyleBackColor = true;
            this.btn_readRom_mbc5.Click += new System.EventHandler(this.btn_readRom_mbc5_Click);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(6, 16);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(77, 12);
            this.label6.TabIndex = 35;
            this.label6.Text = "ROM大小(MiB)";
            // 
            // btn_readSave_mbc5
            // 
            this.btn_readSave_mbc5.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btn_readSave_mbc5.Location = new System.Drawing.Point(323, 40);
            this.btn_readSave_mbc5.Name = "btn_readSave_mbc5";
            this.btn_readSave_mbc5.Size = new System.Drawing.Size(75, 23);
            this.btn_readSave_mbc5.TabIndex = 33;
            this.btn_readSave_mbc5.Text = "导出存档";
            this.btn_readSave_mbc5.UseVisualStyleBackColor = true;
            this.btn_readSave_mbc5.Click += new System.EventHandler(this.btn_readSave_mbc5_Click);
            // 
            // btn_writeRom_mbc5
            // 
            this.btn_writeRom_mbc5.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btn_writeRom_mbc5.Location = new System.Drawing.Point(242, 11);
            this.btn_writeRom_mbc5.Name = "btn_writeRom_mbc5";
            this.btn_writeRom_mbc5.Size = new System.Drawing.Size(75, 23);
            this.btn_writeRom_mbc5.TabIndex = 29;
            this.btn_writeRom_mbc5.Text = "写入ROM";
            this.btn_writeRom_mbc5.UseVisualStyleBackColor = true;
            this.btn_writeRom_mbc5.Click += new System.EventHandler(this.btn_writeRom_mbc5_Click);
            // 
            // btn_writeSave_mbc5
            // 
            this.btn_writeSave_mbc5.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btn_writeSave_mbc5.Location = new System.Drawing.Point(242, 40);
            this.btn_writeSave_mbc5.Name = "btn_writeSave_mbc5";
            this.btn_writeSave_mbc5.Size = new System.Drawing.Size(75, 23);
            this.btn_writeSave_mbc5.TabIndex = 32;
            this.btn_writeSave_mbc5.Text = "写入存档";
            this.btn_writeSave_mbc5.UseVisualStyleBackColor = true;
            this.btn_writeSave_mbc5.Click += new System.EventHandler(this.btn_writeSave_mbc5_Click);
            // 
            // Form1
            // 
            this.AllowDrop = true;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(598, 473);
            this.Controls.Add(this.tabControl1);
            this.Controls.Add(this.btn_renewPort);
            this.Controls.Add(this.btn_browseSave);
            this.Controls.Add(this.textBox_savePath);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.progressBar_total);
            this.Controls.Add(this.textBox_log);
            this.Controls.Add(this.btn_readID);
            this.Controls.Add(this.comboBox_com);
            this.Controls.Add(this.textBox_romPath);
            this.Controls.Add(this.btn_browseRom);
            this.MinimumSize = new System.Drawing.Size(600, 512);
            this.Name = "Form1";
            this.Text = "ChisFlash Burner";
            this.DragDrop += new System.Windows.Forms.DragEventHandler(this.Form1_DragDrop);
            this.DragEnter += new System.Windows.Forms.DragEventHandler(this.Form1_DragEnter);
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.tabControl1.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.tabPage1.PerformLayout();
            this.tabPage2.ResumeLayout(false);
            this.tabPage2.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btn_browseRom;
        private System.Windows.Forms.TextBox textBox_romPath;
        private System.Windows.Forms.ComboBox comboBox_com;
        private System.Windows.Forms.Button btn_readID;
        private System.Windows.Forms.TextBox textBox_log;
        private System.Windows.Forms.ProgressBar progressBar_total;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox textBox_savePath;
        private System.Windows.Forms.Button btn_browseSave;
        private System.Windows.Forms.Button btn_eraseChip;
        private System.Windows.Forms.Button btn_writeRom;
        private System.Windows.Forms.Button btn_readRom;
        private System.Windows.Forms.Button btn_verifyRom;
        private System.Windows.Forms.Button btn_writeSave;
        private System.Windows.Forms.Button btn_readSave;
        private System.Windows.Forms.Button btn_verifySave;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.ComboBox comboBox_saveSize;
        private System.Windows.Forms.ComboBox comboBox_romSize;
        private System.Windows.Forms.ToolStripStatusLabel label_speed;
        private System.Windows.Forms.Timer tmr_showSpeed;
        private System.Windows.Forms.ToolStripStatusLabel label_progress;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel2;
        private System.Windows.Forms.Button btn_renewPort;
        private System.Windows.Forms.ComboBox comboBox_ramType;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.Button btn_eraseChip_mbc5;
        private System.Windows.Forms.ComboBox comboBox_mbc5MultiCartSelect;
        private System.Windows.Forms.Button btn_verifyRom_mbc5;
        private System.Windows.Forms.Button btn_verifySave_mbc5;
        private System.Windows.Forms.ComboBox comboBox_saveSize_mbc5;
        private System.Windows.Forms.ComboBox comboBox_romSize_mbc5;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btn_readRom_mbc5;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Button btn_readSave_mbc5;
        private System.Windows.Forms.Button btn_writeRom_mbc5;
        private System.Windows.Forms.Button btn_writeSave_mbc5;
        private System.Windows.Forms.ComboBox comboBox_gbaMultiCartSelect;
    }
}

