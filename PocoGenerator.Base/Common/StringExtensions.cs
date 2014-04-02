// --------------------------------------------------------------------------------------------------------------------
// <copyright file="StringExtensions.cs" company="Company">
//   Copyrights 2014.
// </copyright>
// <summary>
//   The string extensions.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace PocoGenerator.Base.Common
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Text.RegularExpressions;

    /// <summary>
    /// The string extensions.
    /// </summary>
    public static class StringExtensions
    {
        /// <summary>
        /// The _migration id pattern.
        /// </summary>
        [SuppressMessage("StyleCop.CSharp.NamingRules", "SA1311:StaticReadonlyFieldsMustBeginWithUpperCaseLetter", Justification = "Reviewed. Suppression is OK here.")]
        private static readonly Regex _migrationIdPattern = new Regex(@"\d{15}_.+");

        /// <summary>
        /// The equals ignore case.
        /// </summary>
        /// <param name="s1">
        /// The s 1.
        /// </param>
        /// <param name="s2">
        /// The s 2.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public static bool EqualsIgnoreCase(this string s1, string s2)
        {
            return string.Equals(s1, s2, StringComparison.OrdinalIgnoreCase);
        }
    }
}
