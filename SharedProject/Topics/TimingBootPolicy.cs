using System;
namespace SharedProject
{
	public class TimingBootPolicy : Policy
	{
        /// <summary>
        /// UNKNOWN_WEEKDAY = 0; WEEKDAY_MONDAY = 1; WEEKDAY_TUESDAY = 2; WEEKDAY_WEDNESDAY = 3;
        /// WEEKDAY_THURSDAY = 4; WEEKDAY_FRIDAY = 5; WEEKDAY_SATURDAY = 6; WEEKDAY_SUNDAY = 7
        /// </summary>
        [JsonPropertyName("weekdays")]
        public Weekdays Weekdays { get; set; }
    }
}

