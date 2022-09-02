using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;
using System.Timers;

namespace SharedProject
{
    internal class ClockHelper : INotifyPropertyChanged
    {
        const string clockType1 = "yyyy-MM-dd ddd hh:mm:ss";
        const string clockType2 = "hh:mm:ss\nyyyy-MM-dd ddd";
        public event PropertyChangedEventHandler? PropertyChanged;
        private readonly Timer _timer = new Timer(1000)
        {
            AutoReset = true,
        };
        public ClockHelper(Component.ClockTypeEnum clockType)
        {
            _timer.Elapsed += (o, e) =>
            {
                Value = clockType switch
                {
                    Component.ClockTypeEnum.TYPE_1 => e.SignalTime.ToString(clockType1),
                    Component.ClockTypeEnum.TYPE_2 => e.SignalTime.ToString(clockType2),
                    _ => e.SignalTime.ToString(clockType1),
                };
                ValueChanged();
            };
        }
        private string date = "";
        public string Value { get { return date; }set { date = value;ValueChanged(); } }
        private void ValueChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        ~ClockHelper()
        {
            _timer.Dispose();
        }
    }
}
