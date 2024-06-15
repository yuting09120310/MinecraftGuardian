using System.Diagnostics;
using System;
using System.Threading;
using System.Windows.Forms;
using System.IO.Compression;

namespace MinecraftGuardian
{
    public partial class Form1 : Form
    {
        private Thread monitorThread;
        private bool monitoring;
        private string processName = Program.Configuration["ProcessName"]!;  // 假設啟動的進程是 Java
        private string startCommand = Program.Configuration["StartApp"]!;  // 啟動 start.bat 的路徑


        public Form1()
        {
            InitializeComponent();
        }


        private void buttonStart_Click(object sender, EventArgs e)
        {
            if (!monitoring)
            {
                monitoring = true;
                monitorThread = new Thread(MonitorProcess);
                monitorThread.Start();
                labelStatus.Text = "監控中.";
            }
        }


        private void buttonStop_Click(object sender, EventArgs e)
        {
            if (monitoring)
            {
                monitoring = false;
                monitorThread.Join();
                labelStatus.Text = "監控停止.";
            }
        }


        private async void MonitorProcess()
        {
            while (monitoring)
            {
                if (!IsProcessRunning(processName))
                {
                    Invoke((MethodInvoker)delegate
                    {
                        labelStatus.Text = $"{processName} 未運行。正在啟動...";
                    });

                    StartProcess(startCommand);

                    Invoke((MethodInvoker)delegate
                    {
                        labelStatus.Text = $"{processName} 啟動完成.";
                    });
                }
                else
                {
                    Invoke((MethodInvoker)delegate
                    {
                        labelStatus.Text = $"{processName} 已啟動";
                    });
                }

                // 如果是指定時間就去做地圖備份
                await BackupMapAsync();

                await Task.Delay(10000);  // 每 10 秒檢查一次
            }
        }


        private bool IsProcessRunning(string processName)
        {
            Process[] processes = Process.GetProcessesByName(processName);
            return processes.Length > 0;
        }


        private void StartProcess(string command)
        {
            try
            {
                Process.Start(command);
            }
            catch (Exception ex)
            {
                Invoke((MethodInvoker)delegate
                {
                    labelStatus.Text = $"啟動失敗 原因: {ex.Message}";
                });
            }
        }


        /// <summary>
        /// 備份地圖
        /// </summary>
        private async Task BackupMapAsync()
        {
            DateTime dateTime = DateTime.Now;
            if(dateTime.Hour == 6 || dateTime.Hour == 18 && dateTime.Minute == 0 && dateTime.Second == 0)
            {
                labelStatus.Text = "地圖備份中...";

                //取得所有進程
                Process[] processes = Process.GetProcessesByName("java");
                processes.ToList().ForEach(p =>
                {
                    p.Kill();
                });

                string mapPath = Program.Configuration["MapPath"]!;
                string backupPath = Program.Configuration["BackupPath"]!;
                string zipFileName = $"backup_{DateTime.Now:yyyyMMddHHmm}.zip";
                string zipFilePath = Path.Combine(backupPath, zipFileName);

                if (!Directory.Exists(backupPath))
                {
                    Directory.CreateDirectory(backupPath);
                }

                ZipFile.CreateFromDirectory(mapPath, zipFilePath);
            }
        }

    }
}