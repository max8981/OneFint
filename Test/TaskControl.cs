using Microsoft.Win32.TaskScheduler;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Test
{
    internal class TaskControl : IDisposable
    {
        private readonly TaskService _taskService;
        private readonly TaskFolder _taskFolder;
        private readonly string _path;
        public TaskControl(string path)
        {
            _path = path;
            _taskService = new();
            _taskFolder = _taskService.GetFolder(path);
        }
        public TaskCollection GetAllTasks()
        {
            return _taskFolder.GetTasks();
        }
        public bool IsExists(string taskName)
        {
            foreach (var task in GetAllTasks())
                if (task.Name.Equals(taskName))
                    return true;
            return false;
        }
        public void DeleteTask(string taskName)
        {
            if (IsExists(taskName))
                _taskFolder.DeleteTask(taskName);
        }
        public void CreateTask(string taskName,ExecAction exec,DateTime date)
        {
            TaskDefinition definition = _taskService.NewTask();
            definition.RegistrationInfo.Author = "Author";
            definition.RegistrationInfo.Description = "Description";
            definition.RegistrationInfo.Version=new Version(0,0);
            definition.RegistrationInfo.Date = DateTime.Now;

            DailyTrigger trigger = new DailyTrigger();
            trigger.Enabled = true;
            trigger.StartBoundary= date;

            definition.Triggers.Add(trigger);

            //definition.Actions.Add(new ExecAction("C:\\Windows\\notepad.exe", "", null));
            definition.Actions.Add(exec);

            definition.Principal.RunLevel = TaskRunLevel.Highest;
            definition.Principal.UserId = "maxin";
            definition.Principal.LogonType = TaskLogonType.S4U;
            definition.Principal.ProcessTokenSidType = TaskProcessTokenSidType.Default;

            definition.Settings.DisallowStartIfOnBatteries = false;
            definition.Settings.RunOnlyIfIdle = false;
            definition.Settings.Enabled = true;
            definition.Settings.AllowDemandStart = true;
            definition.Settings.AllowHardTerminate = true;

            var task = _taskService.RootFolder.RegisterTaskDefinition($"{_path}\\{taskName}", definition);
        }
        public void Dispose()
        {
            _taskService.Dispose();
            _taskFolder.Dispose();
        }
    }
}
