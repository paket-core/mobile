using System;

namespace PaketGlobal
{
	public static class DateTimeHelper
	{
		public static long ToUnixTime(DateTime dt)
		{
			return (long)(dt.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;
		}

		public static long ToMilliUnixTime(DateTime dt)
		{
			return (long)(dt.Subtract(new DateTime(1970, 1, 1))).TotalMilliseconds;
		}

		public static DateTime FromMilliUnixTime(long ut)
		{
			return new DateTime(1970, 1, 1).AddMilliseconds(ut);
		}

		public static DateTime FromUnixTime(long ut)
		{
			return new DateTime(1970, 1, 1).AddSeconds(ut);
		}
	}
}
