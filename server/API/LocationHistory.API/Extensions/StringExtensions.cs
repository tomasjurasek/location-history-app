using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LocationHistory.API.Extensions
{
    public static class StringExtensions
    {

        public static bool IsValidPhone(this string phone)
        {
            return phone?.Length == 9 || phone?.Length == 12;
        }

        public static string GetPhone(this string phone)
        {
            string[] phonePrefixes = { "420, 421" };
            var containsPrefix = phonePrefixes.Select(s => phone.StartsWith(s));

            if (containsPrefix.Any())
            {
                return phone.Remove(0, 3);
            }

            return phone;
        }
    }
}
