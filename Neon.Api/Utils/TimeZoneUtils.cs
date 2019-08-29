using NodaTime.TimeZones;
using System;
using System.Linq;
using TimeZoneConverter;

namespace Neon.Api.Utils
{
	public static class TimeZoneUtils
	{
		public static TimeZoneInfo GetTimeZoneInfoForTzdbId(string tzdbId)
		{
			return TimeZoneInfo.FindSystemTimeZoneById(TZConvert.IanaToWindows(tzdbId)); 
		}

		public static string ToTzdb(TimeZoneInfo timeZoneInfo)
		{
			if (timeZoneInfo.Id.Contains("/")) return timeZoneInfo.Id;

			var item = TZConvert.WindowsToIana(timeZoneInfo.Id);

			return item;

		}
	}
}
