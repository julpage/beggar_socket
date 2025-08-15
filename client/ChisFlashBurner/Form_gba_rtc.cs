using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ChisFlashBurner
{
    public partial class Form_gba_rtc : Form
    {
        public byte year;
        public byte month;
        public byte date;
        public byte day;
        public byte hour;
        public byte minute;
        public byte second;

        public bool isSet = false;

        public Form_gba_rtc()
        {
            InitializeComponent();
        }
        byte IntToCompressedBCD(int number)
        {
            // 分离十位和个位
            int tens = number / 10;
            int units = number % 10;

            // 组合成压缩BCD码（十位左移4位 + 个位）
            return (byte)((tens << 4) | units);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            year = IntToCompressedBCD((int)nud_year.Value);
            month = IntToCompressedBCD((int)nud_month.Value);
            date = IntToCompressedBCD((int)nud_date.Value);
            day = IntToCompressedBCD((int)nud_day.Value);
            hour = IntToCompressedBCD((int)nud_hour.Value);
            minute = IntToCompressedBCD((int)nud_minute.Value);
            second = IntToCompressedBCD((int)nud_second.Value);

            isSet = true;

            this.Close();
        }

        private void Form_gba_rtc_Load(object sender, EventArgs e)
        {
            DateTime currentTime = DateTime.Now;

            nud_year.Value = currentTime.Year - 2000;
            nud_month.Value = currentTime.Month;
            nud_date.Value = currentTime.Day;
            nud_day.Value = (decimal)currentTime.DayOfWeek; // sun:0 sat:6
            nud_hour.Value = currentTime.Hour;
            nud_minute.Value = currentTime.Minute;
            nud_second.Value = currentTime.Second;

        }
    }
}
