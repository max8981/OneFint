using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace 屏幕管理.Systems
{
    internal class Regedit
    {
        /// <summary>
        /// 禁用系统UAC，制作安装包后报毒 ╮(╯▽╰)╭
        /// </summary>
        /// <returns></returns>
        internal static bool DisableUAC()
        {
            int value = 0;
            var result = false;
            try
            {
                string path = @"SOFTWARE\Microsoft\Windows\CurrentVersion\Policies\System";
                string uac = "EnableLUA";
                RegistryKey key = Registry.LocalMachine.CreateSubKey(path);
                if (key != null)
                {
                    value = (int)key.GetValue(uac, 1)!;
                    key.SetValue(uac, 0, RegistryValueKind.DWord);
                    key.Close();
                    result = true;
                }
            }
            catch (Exception ex)
            {
                Log.Default.Error(ex, "DisableUAC");
            }
            return result && value > 0;
        }
        internal static bool EnableUAC()
        {
            int value = 0;
            var result = false;
            try
            {
                string path = @"SOFTWARE\Microsoft\Windows\CurrentVersion\Policies\System";
                string uac = "EnableLUA";
                RegistryKey key = Registry.LocalMachine.CreateSubKey(path);
                if (key != null)
                {
                    value = (int)key.GetValue(uac, 0)!;
                    key.SetValue(uac, 1, RegistryValueKind.DWord);
                    key.Close();
                    result = true;
                }
            }
            catch (Exception ex)
            {
                Log.Default.Error(ex, "EnableUAC");
            }
            return result && value == 0;
        }
        internal static void SetAutoRun()
        {
            var startupPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Startup), "屏幕管理.lnk");
            var shellType = Type.GetTypeFromProgID("WScript.Shell");
            dynamic? shell = Activator.CreateInstance(shellType!);
            var shortcut = shell?.CreateShortcut(startupPath);
            if (shortcut != null)
            {
                shortcut.TargetPath = Process.GetCurrentProcess().MainModule?.FileName;
                shortcut.WorkingDirectory = AppDomain.CurrentDomain.SetupInformation.ApplicationBase;
                shortcut.Save();
            }
        }
        internal static void DeleteAutoRun()
        {
            var startupPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Startup), "屏幕管理.lnk");
            try
            {
                System.IO.File.Delete(startupPath);
            }
            catch
            {

            }

        }
    }
}
