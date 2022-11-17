using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.ServiceProcess;
using System.Configuration.Install;
using Cjwdev.WindowsApi;
using System.Runtime.InteropServices;

namespace ClientServiceInstaller
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        private const string SERVICE_NAME = "ClientService";
        private const string SERVICE_PATH = "./ClientService.exe";
        private void InstallBtn_Click(object sender, EventArgs e)
        {
            string message;
            if (System.IO.File.Exists(SERVICE_PATH))
            {
                if(TryGetPath(out var filePath))
                {
                    System.IO.File.WriteAllText("c:/ClientLauncher.ini", filePath);
                    if (IsServiceExisted(SERVICE_NAME))
                        UninstallService(SERVICE_NAME);
                    InstallService(SERVICE_PATH);
                    ServiceStart(SERVICE_NAME, new string[] { });
                    message = "安装成功！";
                }
                else
                {
                    message = "安装失败！未选择启动程序";
                }
            }
            else
            {
                message = "安装失败！没有找到服务程序";
            }
            MessageBox.Show(message);
        }
        private bool TryGetPath(out string filePath)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.ShowDialog();
            filePath = dialog.FileName;
            return !string.IsNullOrEmpty(filePath);
        }
        private bool IsServiceExisted(string serviceName)
        {
            ServiceController[] services = ServiceController.GetServices();
            return services.Any(_ => _.ServiceName.ToLower() == serviceName.ToLower());
        }
        private void InstallService(string servicePath)
        {
            AssemblyInstaller installer = new AssemblyInstaller
            {
                UseNewContext = true,
                Path = servicePath
            };
            System.Collections.IDictionary saveState = new System.Collections.Hashtable();
            installer.Install(saveState);
            installer.Commit(saveState);
            installer.Dispose();
        }
        private void UninstallService(string servicePath)
        {
            AssemblyInstaller installer = new AssemblyInstaller
            {
                UseNewContext = true,
                Path = servicePath
            };
            installer.Uninstall(null);
            installer.Dispose();
        }
        private void ServiceStart(string serviceName, string[] args=null)
        {
            ServiceController controller = new ServiceController(serviceName);
            if (controller.Status == ServiceControllerStatus.Stopped)
                controller.Start(args);
            controller.Dispose();
        }
        private void ServiceStop(string serviceName)
        {
            ServiceController controller = new ServiceController(serviceName);
            if (controller.Status == ServiceControllerStatus.Running)
                controller.Stop();
            controller.Dispose();
        }

        private void UnInstallBtn_Click(object sender, EventArgs e)
        {
            ServiceStop(SERVICE_NAME);
            UninstallService(SERVICE_PATH);
        }
        private void Launcher(string path)
        {
            var appPath = path;
            IntPtr userTokenHandle = IntPtr.Zero;
            ApiDefinitions.WTSQueryUserToken(ApiDefinitions.WTSGetActiveConsoleSessionId(), ref userTokenHandle);
            ApiDefinitions.PROCESS_INFORMATION procInfo = new ApiDefinitions.PROCESS_INFORMATION();
            ApiDefinitions.STARTUPINFO startInfo = new ApiDefinitions.STARTUPINFO();
            startInfo.cb = (uint)Marshal.SizeOf(startInfo);
            ApiDefinitions.CreateProcessAsUser(userTokenHandle, appPath, "", IntPtr.Zero, IntPtr.Zero, false, 0, IntPtr.Zero, null, ref startInfo, out procInfo);
            if (userTokenHandle != IntPtr.Zero)
                ApiDefinitions.CloseHandle(userTokenHandle);
            int _currentAquariusProcessId = (int)procInfo.dwProcessId;
        }

        private void LauncherBtn_Click(object sender, EventArgs e)
        {
            if (TryGetPath(out var path))
                Launcher(path);
        }
    }
}
