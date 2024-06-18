using System.Diagnostics;
using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.IO.Compression;
using System.Linq;

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

        /// <summary>
        /// 當按下「開始監控」按鈕時觸發的事件處理函式。
        /// </summary>
        private void buttonStart_Click(object sender, EventArgs e)
        {
            if (!monitoring)
            {
                monitoring = true;
                monitorThread = new Thread(MonitorProcess);
                monitorThread.Start();
                labelStatus.Text = "監控中.";
                AppTools.Log("監控開始", true);
            }
        }

        /// <summary>
        /// 當按下「停止監控」按鈕時觸發的事件處理函式。
        /// </summary>
        private void buttonStop_Click(object sender, EventArgs e)
        {
            if (monitoring)
            {
                monitoring = false;
                monitorThread.Join();
                labelStatus.Text = "監控停止.";
                AppTools.Log("監控停止", true);
            }
        }

        /// <summary>
        /// 監控進程的主要函式。
        /// </summary>
        private async void MonitorProcess()
        {
            while (monitoring)
            {
                try
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
                        AppTools.Log($"{processName} 啟動", true);
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
                catch (Exception ex)
                {
                    Invoke((MethodInvoker)delegate
                    {
                        labelStatus.Text = $"監控過程中發生錯誤: {ex.Message}";
                    });
                    AppTools.Log("監控過程中發生錯誤", false);
                }
            }
        }

        /// <summary>
        /// 檢查指定進程是否正在運行。
        /// </summary>
        private bool IsProcessRunning(string processName)
        {
            Process[] processes = Process.GetProcessesByName(processName);
            return processes.Length > 0;
        }

        /// <summary>
        /// 啟動指定的進程。
        /// </summary>
        private void StartProcess(string command)
        {
            try
            {
                Process.Start(command);
                AppTools.Log($"啟動 {command}", true);
            }
            catch (Exception ex)
            {
                Invoke((MethodInvoker)delegate
                {
                    labelStatus.Text = $"啟動失敗 原因: {ex.Message}";
                });
                AppTools.Log($"啟動 {command} 失敗", false);
            }
        }

        /// <summary>
        /// 備份地圖。
        /// </summary>
        private async Task BackupMapAsync()
        {
            try
            {
                DateTime dateTime = DateTime.Now;
                if ((dateTime.Hour == 0 || dateTime.Hour == 6 || dateTime.Hour == 12 || dateTime.Hour == 18) && dateTime.Minute == 0 && dateTime.Second == 0)
                {
                    Invoke((MethodInvoker)delegate
                    {
                        labelStatus.Text = "地圖備份中...";
                    });
                    AppTools.Log("地圖備份開始", true);

                    //取得所有進程
                    Process[] processes = Process.GetProcessesByName("java");
                    processes.ToList().ForEach(p =>
                    {
                        try
                        {
                            p.Kill();
                            AppTools.Log($"終止進程: {p.ProcessName} (PID: {p.Id})", true);
                        }
                        catch (Exception ex)
                        {
                            Invoke((MethodInvoker)delegate
                            {
                                labelStatus.Text = $"無法終止進程: {p.ProcessName} (PID: {p.Id})。錯誤: {ex.Message}";
                            });
                            AppTools.Log($"無法終止進程: {p.ProcessName} (PID: {p.Id})", false);
                        }
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

                    Invoke((MethodInvoker)delegate
                    {
                        labelStatus.Text = "地圖備份完成.";
                    });
                    AppTools.Log("地圖備份完成", true);
                }
            }
            catch (Exception ex)
            {
                Invoke((MethodInvoker)delegate
                {
                    labelStatus.Text = $"備份過程中發生錯誤: {ex.Message}";
                });
                AppTools.Log("備份過程中發生錯誤", false);
            }
        }
    }
}
