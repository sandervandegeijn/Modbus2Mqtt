using System.ComponentModel;
using System.Text.RegularExpressions;

namespace Modbus2Mqtt.Infrastructure
{
    public static class StringExtensions
    {
        public static string StripNonAlphaNumeric(this string str)
        {
            return Regex.Replace(str, @"[^a-zA-Z0-9]+", "-");
        }
        
        public static T GetTfromString<T>(this string mystring)
        {
            var foo = TypeDescriptor.GetConverter(typeof(T));
            return (T)(foo.ConvertFromInvariantString(mystring));
        }
    }
}