namespace ChisFlashBurner
{
    partial class Form_gba_rtc
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.button1 = new System.Windows.Forms.Button();
            this.nud_year = new System.Windows.Forms.NumericUpDown();
            this.label1 = new System.Windows.Forms.Label();
            this.nud_month = new System.Windows.Forms.NumericUpDown();
            this.nud_date = new System.Windows.Forms.NumericUpDown();
            this.nud_day = new System.Windows.Forms.NumericUpDown();
            this.nud_hour = new System.Windows.Forms.NumericUpDown();
            this.nud_minute = new System.Windows.Forms.NumericUpDown();
            this.nud_second = new System.Windows.Forms.NumericUpDown();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.nud_year)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nud_month)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nud_date)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nud_day)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nud_hour)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nud_minute)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nud_second)).BeginInit();
            this.SuspendLayout();
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(198, 67);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(56, 23);
            this.button1.TabIndex = 0;
            this.button1.Text = "OK";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // nud_year
            // 
            this.nud_year.Location = new System.Drawing.Point(12, 24);
            this.nud_year.Maximum = new decimal(new int[] {
            99,
            0,
            0,
            0});
            this.nud_year.Name = "nud_year";
            this.nud_year.Size = new System.Drawing.Size(56, 21);
            this.nud_year.TabIndex = 1;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(17, 12);
            this.label1.TabIndex = 2;
            this.label1.Text = "年";
            // 
            // nud_month
            // 
            this.nud_month.Location = new System.Drawing.Point(74, 24);
            this.nud_month.Maximum = new decimal(new int[] {
            12,
            0,
            0,
            0});
            this.nud_month.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.nud_month.Name = "nud_month";
            this.nud_month.Size = new System.Drawing.Size(56, 21);
            this.nud_month.TabIndex = 3;
            this.nud_month.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // nud_date
            // 
            this.nud_date.Location = new System.Drawing.Point(136, 24);
            this.nud_date.Maximum = new decimal(new int[] {
            31,
            0,
            0,
            0});
            this.nud_date.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.nud_date.Name = "nud_date";
            this.nud_date.Size = new System.Drawing.Size(56, 21);
            this.nud_date.TabIndex = 4;
            this.nud_date.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // nud_day
            // 
            this.nud_day.Location = new System.Drawing.Point(198, 24);
            this.nud_day.Maximum = new decimal(new int[] {
            6,
            0,
            0,
            0});
            this.nud_day.Name = "nud_day";
            this.nud_day.Size = new System.Drawing.Size(56, 21);
            this.nud_day.TabIndex = 5;
            // 
            // nud_hour
            // 
            this.nud_hour.Location = new System.Drawing.Point(12, 68);
            this.nud_hour.Maximum = new decimal(new int[] {
            23,
            0,
            0,
            0});
            this.nud_hour.Name = "nud_hour";
            this.nud_hour.Size = new System.Drawing.Size(56, 21);
            this.nud_hour.TabIndex = 6;
            // 
            // nud_minute
            // 
            this.nud_minute.Location = new System.Drawing.Point(74, 68);
            this.nud_minute.Maximum = new decimal(new int[] {
            59,
            0,
            0,
            0});
            this.nud_minute.Name = "nud_minute";
            this.nud_minute.Size = new System.Drawing.Size(56, 21);
            this.nud_minute.TabIndex = 7;
            // 
            // nud_second
            // 
            this.nud_second.Location = new System.Drawing.Point(136, 68);
            this.nud_second.Maximum = new decimal(new int[] {
            59,
            0,
            0,
            0});
            this.nud_second.Name = "nud_second";
            this.nud_second.Size = new System.Drawing.Size(56, 21);
            this.nud_second.TabIndex = 8;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(72, 9);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(17, 12);
            this.label2.TabIndex = 9;
            this.label2.Text = "月";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(134, 9);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(17, 12);
            this.label3.TabIndex = 10;
            this.label3.Text = "日";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(196, 9);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(29, 12);
            this.label4.TabIndex = 11;
            this.label4.Text = "星期";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(12, 53);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(17, 12);
            this.label5.TabIndex = 12;
            this.label5.Text = "时";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(72, 53);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(17, 12);
            this.label6.TabIndex = 13;
            this.label6.Text = "分";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(134, 53);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(17, 12);
            this.label7.TabIndex = 14;
            this.label7.Text = "秒";
            // 
            // Form_gba_rtc
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(263, 101);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.nud_second);
            this.Controls.Add(this.nud_minute);
            this.Controls.Add(this.nud_hour);
            this.Controls.Add(this.nud_day);
            this.Controls.Add(this.nud_date);
            this.Controls.Add(this.nud_month);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.nud_year);
            this.Controls.Add(this.button1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "Form_gba_rtc";
            this.Load += new System.EventHandler(this.Form_gba_rtc_Load);
            ((System.ComponentModel.ISupportInitialize)(this.nud_year)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nud_month)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nud_date)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nud_day)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nud_hour)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nud_minute)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nud_second)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.NumericUpDown nud_year;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.NumericUpDown nud_month;
        private System.Windows.Forms.NumericUpDown nud_date;
        private System.Windows.Forms.NumericUpDown nud_day;
        private System.Windows.Forms.NumericUpDown nud_hour;
        private System.Windows.Forms.NumericUpDown nud_minute;
        private System.Windows.Forms.NumericUpDown nud_second;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label7;
    }
}