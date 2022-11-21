namespace ClientCore.Models
{
    public class TimingBootPolicy : Policy
    {
        /// <summary>
        /// 循环类型，可选0，1，2，3; 0:UNKNOWN_BOOT_POLICY_LOOP_TYPE; 1:EVERYDAY（每天）; 2:SOMEDAY（每周几）;
        /// 3:PREIOD（某时段，含日期）
        /// </summary>
        [JsonPropertyName("loop_type")]
        public Enums.LoopTypeEnum LoopType { get; set; }
        /// <summary>
        /// UNKNOWN_WEEKDAY = 0; WEEKDAY_MONDAY = 1; WEEKDAY_TUESDAY = 2; WEEKDAY_WEDNESDAY = 3;
        /// WEEKDAY_THURSDAY = 4; WEEKDAY_FRIDAY = 5; WEEKDAY_SATURDAY = 6; WEEKDAY_SUNDAY = 7
        /// </summary>
        [JsonPropertyName("weekdays")]
        public Weekdays? Weekdays { get; set; }
    }
}

