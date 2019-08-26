using NodaTime.TimeZones;
using System;
using System.Linq;

namespace Neon.Api.Utils
{
	public static class TimeZoneUtils
	{
		public static TimeZoneInfo GetTimeZoneInfoForTzdbId(string tzdbId)
		{
			var mappings = TzdbDateTimeZoneSource.Default.WindowsMapping.MapZones;
			var map = mappings.FirstOrDefault(x =>
				x.TzdbIds.Any(z => z.Equals(tzdbId, StringComparison.OrdinalIgnoreCase)));
			return map == null ? null : TimeZoneInfo.FindSystemTimeZoneById(map.WindowsId);
		}

		public static string ToTzdb(TimeZoneInfo timeZoneInfo)
		{
			var item = TzdbDateTimeZoneSource.Default.WindowsMapping.MapZones.FirstOrDefault(m =>
				m.WindowsId == timeZoneInfo.Id);


			return item.TzdbIds.First();

		}
	}
}
