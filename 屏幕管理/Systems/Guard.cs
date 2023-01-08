using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace 屏幕管理.Systems
{
    class Guard
    {
        [DllImport("kernel32")]
        private static extern ExecutionStateEnum SetThreadExecutionState(ExecutionStateEnum esFlags);
        private static readonly System.Timers.Timer _timer = new(1000);
        private static readonly string _guardPath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Watchdog.exe");
        private static readonly string _name = "Watchdog";
        public static void StartWatchdog(string appPath, int interval = 3000,TimeSpan? timeSpan = null)
        {
            var time = $"{timeSpan?.Hours:00}:{timeSpan?.Minutes:00}";
            time = time.Length > 1 ? time : null;
            interval = Math.Clamp(interval, 1000, 600000);
            _timer.AutoReset = true;
            _timer.Elapsed += (o, e) =>
            {
                var ps = Process.GetProcessesByName(_name);
                if (!ps.Any())
                {
                    Log.Default.Warn("启动守护", "StartWatchdog");
                    Process.Start(_guardPath, $" {appPath} {interval} {time}");
                }
            };
            if (System.IO.File.Exists(_guardPath))
            {
                Log.Default.Warn("开始监视守护", "StartWatchdog");
                _timer.Start();
            }
        }
        public static void StopWatchDog()
        {
            _timer.Stop();
            var ps = Process.GetProcessesByName(_name);
            foreach (var item in ps)
                item.Kill();
        }
        public static void RestoreForCurrentThread()
        {
            SetThreadExecutionState(ExecutionStateEnum.Continuous);
        }
        enum ExecutionStateEnum : uint
        {
            /// <summary>
            /// Forces the system to be in the working state by resetting the system idle timer.
            /// </summary>
            SystemRequired = 0x01,

            /// <summary>
            /// Forces the display to be on by resetting the display idle timer.
            /// </summary>
            DisplayRequired = 0x02,

            /// <summary>
            /// This value is not supported. If <see cref="UserPresent"/> is combined with other esFlags values, the call will fail and none of the specified states will be set.
            /// </summary>
            [Obsolete("This value is not supported.")]
            UserPresent = 0x04,

            /// <summary>
            /// Enables away mode. This value must be specified with <see cref="Continuous"/>.
            /// <para />
            /// Away mode should be used only by media-recording and media-distribution applications that must perform critical background processing on desktop computers while the computer appears to be sleeping.
            /// </summary>
            AwaymodeRequired = 0x40,

            /// <summary>
            /// Informs the system that the state being set should remain in effect until the next call that uses <see cref="Continuous"/> and one of the other state flags is cleared.
            /// </summary>
            Continuous = 0x80000000,
        }
    }
}
