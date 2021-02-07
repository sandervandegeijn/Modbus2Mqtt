using System.Text.RegularExpressions;

namespace Modbus2Mqtt.Infrastructure
{
    public static class StringExtensions
    {
        public static string StripNonAlphaNumeric(this string str)
        {
            return Regex.Replace(str, @"[^a-zA-Z0-9]+", "-");
        }
    }
}