using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Teknolider;
using System.Diagnostics;

namespace TEST
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        Bellek memory;
        Process[] prc = Process.GetProcessesByName("ac_client");
        int value = 0;
        Int64 MainAddress;
        private void Form1_Load(object sender, EventArgs e)
        {
            try
            {
                memory = new Bellek(prc[0]);
                foreach (ProcessModule modul in prc[0].Modules)
                {
                    if (modul.ModuleName == "ac_client.exe")
                    {
                        MainAddress = modul.BaseAddress.ToInt64();
                    }
                }
            }
            catch
            { 
                MessageBox.Show("İLK ÖNCE OYUNU AKTİF EDİN");

            }
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox1.Checked == true)
            {
                timer1.Start(); 
            }
            else
            {
                timer1.Stop();
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            value = memory.Int_Yaz_Offset(MainAddress + 0x109B74, "354+38+430+30", 30); // mermi için aratmıştım ben 
        }
    }
}
