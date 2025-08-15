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
    public partial class Form_mbc3_rtc : Form
    {
        public int day;
        public int hour;
        public int minute;
        public int second;

        public bool isSet = false;


        public Form_mbc3_rtc()
        {
            InitializeComponent();

        }

        private void Form_mbc3_rtc_Load(object sender, EventArgs e)
        {
            DateTime currentTime = DateTime.Now;

            nud_day.Value = currentTime.DayOfYear;
            nud_hour.Value = currentTime.Hour;
            nud_minute.Value = currentTime.Minute;
            nud_second.Value = currentTime.Second;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            isSet = true;

            day = (int)nud_day.Value;
            hour = (int)nud_hour.Value;
            minute = (int)nud_minute.Value;
            second = (int)nud_second.Value;

            this.Close();
        }
    }
}
