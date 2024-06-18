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
        private string processName = Program.Configuration["ProcessName"]!;  // ���]�Ұʪ��i�{�O Java
        private string startCommand = Program.Configuration["StartApp"]!;  // �Ұ� start.bat �����|

        public Form1()
        {
            InitializeComponent();
        }

        /// <summary>
        /// ����U�u�}�l�ʱ��v���s��Ĳ�o���ƥ�B�z�禡�C
        /// </summary>
        private void buttonStart_Click(object sender, EventArgs e)
        {
            if (!monitoring)
            {
                monitoring = true;
                monitorThread = new Thread(MonitorProcess);
                monitorThread.Start();
                labelStatus.Text = "�ʱ���.";
                AppTools.Log("�ʱ��}�l", true);
            }
        }

        /// <summary>
        /// ����U�u����ʱ��v���s��Ĳ�o���ƥ�B�z�禡�C
        /// </summary>
        private void buttonStop_Click(object sender, EventArgs e)
        {
            if (monitoring)
            {
                monitoring = false;
                monitorThread.Join();
                labelStatus.Text = "�ʱ�����.";
                AppTools.Log("�ʱ�����", true);
            }
        }

        /// <summary>
        /// �ʱ��i�{���D�n�禡�C
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
                            labelStatus.Text = $"{processName} ���B��C���b�Ұ�...";
                        });

                        StartProcess(startCommand);

                        Invoke((MethodInvoker)delegate
                        {
                            labelStatus.Text = $"{processName} �Ұʧ���.";
                        });
                        AppTools.Log($"{processName} �Ұ�", true);
                    }
                    else
                    {
                        Invoke((MethodInvoker)delegate
                        {
                            labelStatus.Text = $"{processName} �w�Ұ�";
                        });
                    }

                    // �p�G�O���w�ɶ��N�h���a�ϳƥ�
                    await BackupMapAsync();

                    await Task.Delay(10000);  // �C 10 ���ˬd�@��
                }
                catch (Exception ex)
                {
                    Invoke((MethodInvoker)delegate
                    {
                        labelStatus.Text = $"�ʱ��L�{���o�Ϳ��~: {ex.Message}";
                    });
                    AppTools.Log("�ʱ��L�{���o�Ϳ��~", false);
                }
            }
        }

        /// <summary>
        /// �ˬd���w�i�{�O�_���b�B��C
        /// </summary>
        private bool IsProcessRunning(string processName)
        {
            Process[] processes = Process.GetProcessesByName(processName);
            return processes.Length > 0;
        }

        /// <summary>
        /// �Ұʫ��w���i�{�C
        /// </summary>
        private void StartProcess(string command)
        {
            try
            {
                Process.Start(command);
                AppTools.Log($"�Ұ� {command}", true);
            }
            catch (Exception ex)
            {
                Invoke((MethodInvoker)delegate
                {
                    labelStatus.Text = $"�Ұʥ��� ��]: {ex.Message}";
                });
                AppTools.Log($"�Ұ� {command} ����", false);
            }
        }

        /// <summary>
        /// �ƥ��a�ϡC
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
                        labelStatus.Text = "�a�ϳƥ���...";
                    });
                    AppTools.Log("�a�ϳƥ��}�l", true);

                    //���o�Ҧ��i�{
                    Process[] processes = Process.GetProcessesByName("java");
                    processes.ToList().ForEach(p =>
                    {
                        try
                        {
                            p.Kill();
                            AppTools.Log($"�פ�i�{: {p.ProcessName} (PID: {p.Id})", true);
                        }
                        catch (Exception ex)
                        {
                            Invoke((MethodInvoker)delegate
                            {
                                labelStatus.Text = $"�L�k�פ�i�{: {p.ProcessName} (PID: {p.Id})�C���~: {ex.Message}";
                            });
                            AppTools.Log($"�L�k�פ�i�{: {p.ProcessName} (PID: {p.Id})", false);
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
                        labelStatus.Text = "�a�ϳƥ�����.";
                    });
                    AppTools.Log("�a�ϳƥ�����", true);
                }
            }
            catch (Exception ex)
            {
                Invoke((MethodInvoker)delegate
                {
                    labelStatus.Text = $"�ƥ��L�{���o�Ϳ��~: {ex.Message}";
                });
                AppTools.Log("�ƥ��L�{���o�Ϳ��~", false);
            }
        }
    }
}
