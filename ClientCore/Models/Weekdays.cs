namespace ClientCore.Models
{
    /// <summary>
    /// UNKNOWN_WEEKDAY = 0; WEEKDAY_MONDAY = 1; WEEKDAY_TUESDAY = 2; WEEKDAY_WEDNESDAY = 3;
    /// WEEKDAY_THURSDAY = 4; WEEKDAY_FRIDAY = 5; WEEKDAY_SATURDAY = 6; WEEKDAY_SUNDAY = 7
    /// </summary>
    public partial class Weekdays
    {
        /// <summary>
        /// 日期类型， 0:UNKNOWN_WEEKDAY; 1: MONDAY; 2:TUESDAY; 3:WEDNESDAY; 4:THURSDAY; 5:FRIDAY;
        /// 6:SATURDAY; 7:SUNDAY
        /// </summary>
        [JsonPropertyName("days")]
        public Enums.WeekdayEnum[]? Days { get; set; }
    }
}

