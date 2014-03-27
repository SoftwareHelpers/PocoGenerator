using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PocoGenerator.Base.Common
{
    using System.Text.RegularExpressions;

    public static class StringExtensions
    {
        private static readonly Regex _migrationIdPattern = new Regex(@"\d{15}_.+");

        public static bool EqualsIgnoreCase(this string s1, string s2)
        {
            return string.Equals(s1, s2, StringComparison.OrdinalIgnoreCase);
        }
    }
}
