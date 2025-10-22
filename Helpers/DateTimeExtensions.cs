using System;

namespace CETExamApp.Helpers
{
    public static class DateTimeExtensions
    {
        /// <summary>
        /// Gets current DateTime in IST
        /// </summary>
        /// <returns>Current DateTime in IST</returns>
        public static DateTime NowIST()
        {
            return DateTime.UtcNow.ToIST();
        }

        /// <summary>
        /// Converts UTC DateTime to Indian Standard Time (IST)
        /// IST is UTC+5:30
        /// </summary>
        /// <param name="utcDateTime">The UTC DateTime to convert</param>
        /// <returns>DateTime in IST</returns>
        public static DateTime ToIST(this DateTime utcDateTime)
        {
            if (utcDateTime.Kind == DateTimeKind.Unspecified)
            {
                // If the DateTime is unspecified, assume it's UTC
                utcDateTime = DateTime.SpecifyKind(utcDateTime, DateTimeKind.Utc);
            }

            // IST is UTC+5:30
            TimeZoneInfo istTimeZone = TimeZoneInfo.FindSystemTimeZoneById("India Standard Time");
            return TimeZoneInfo.ConvertTimeFromUtc(utcDateTime, istTimeZone);
        }

        /// <summary>
        /// Converts nullable UTC DateTime to Indian Standard Time (IST)
        /// IST is UTC+5:30
        /// </summary>
        /// <param name="utcDateTime">The nullable UTC DateTime to convert</param>
        /// <returns>Nullable DateTime in IST</returns>
        public static DateTime? ToIST(this DateTime? utcDateTime)
        {
            if (utcDateTime.HasValue)
            {
                return utcDateTime.Value.ToIST();
            }
            return null;
        }

        /// <summary>
        /// Formats DateTime in IST with standard format
        /// </summary>
        /// <param name="istDateTime">The IST DateTime to format</param>
        /// <returns>Formatted string in IST</returns>
        public static string ToISTString(this DateTime istDateTime)
        {
            return istDateTime.ToString("dd/MM/yyyy HH:mm");
        }

        /// <summary>
        /// Formats nullable DateTime in IST with standard format
        /// </summary>
        /// <param name="istDateTime">The nullable IST DateTime to format</param>
        /// <returns>Formatted string in IST or empty string if null</returns>
        public static string ToISTString(this DateTime? istDateTime)
        {
            if (istDateTime.HasValue)
            {
                return istDateTime.Value.ToISTString();
            }
            return string.Empty;
        }

        /// <summary>
        /// Formats DateTime in IST for datetime-local input
        /// </summary>
        /// <param name="istDateTime">The IST DateTime to format</param>
        /// <returns>Formatted string for datetime-local input</returns>
        public static string ToISTDateTimeLocal(this DateTime istDateTime)
        {
            return istDateTime.ToString("yyyy-MM-ddTHH:mm");
        }

        /// <summary>
        /// Formats nullable DateTime in IST for datetime-local input
        /// </summary>
        /// <param name="istDateTime">The nullable IST DateTime to format</param>
        /// <returns>Formatted string for datetime-local input or empty string if null</returns>
        public static string ToISTDateTimeLocal(this DateTime? istDateTime)
        {
            if (istDateTime.HasValue)
            {
                return istDateTime.Value.ToISTDateTimeLocal();
            }
            return string.Empty;
        }

        /// <summary>
        /// Converts IST DateTime string to IST DateTime
        /// </summary>
        /// <param name="istDateTimeString">IST DateTime string</param>
        /// <returns>IST DateTime</returns>
        public static DateTime ISTStringToIST(this string istDateTimeString)
        {
            if (DateTime.TryParse(istDateTimeString, out DateTime istDateTime))
            {
                return istDateTime;
            }
            throw new ArgumentException("Invalid DateTime string format");
        }

        /// <summary>
        /// Converts IST DateTime string to IST DateTime (nullable)
        /// </summary>
        /// <param name="istDateTimeString">IST DateTime string</param>
        /// <returns>Nullable IST DateTime</returns>
        public static DateTime? ISTStringToISTNullable(this string? istDateTimeString)
        {
            if (string.IsNullOrEmpty(istDateTimeString))
                return null;

            if (DateTime.TryParse(istDateTimeString, out DateTime istDateTime))
            {
                return istDateTime;
            }
            return null;
        }
    }
}
