using Newtonsoft.Json;
using RobotQQ.Core;
using System;
using System.Diagnostics;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;

namespace RobotQQ
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if(WebQQ.instance.status==RobotStatus.offline)
            pictureBox1.Image = WebQQ.instance.Login_QCode();
            //WebQQ.instance.save();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            WebQQ.instance.onLoginMessage += WebQQ_onLoginMessage;
            WebQQ.instance.onLog += Instance_onLog;
        }
        
        private void Instance_onLog(object sender, string e)
        {
            textBox1.BeginInvoke((Action)delegate {
                textBox1.Text = e;
            });
        }

        private void WebQQ_onLoginMessage(object sender, string e)
        {
            state1.Text = e;
            Debug.WriteLine(e);
        }
    }
}
