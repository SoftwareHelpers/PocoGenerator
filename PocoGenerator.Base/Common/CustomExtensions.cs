// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CustomExtensions.cs" company="Company">
//   Copyrights 2014.
// </copyright>
// <summary>
//   The custom extensions.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace PocoGenerator.Base.Common
{
    using System;
    using System.Collections.Generic;
    using System.Windows.Threading;

    /// <summary>
    /// The custom extensions.
    /// </summary>
    public static class CustomExtensions
    {
        /// <summary>
        /// The save rest.
        /// </summary>
        /// <param name="enumerator"> The enumerator. </param>
        /// <typeparam name="T">Type of parameter </typeparam> <returns>
        /// The <see cref="List"/>. </returns>
        public static List<T> SaveRest<T>(this IEnumerator<T> enumerator)
        {
            var list = new List<T>();
            while (enumerator.MoveNext())
            {
                list.Add(enumerator.Current);
            }

            return list;
        }

        /// <summary>
        /// The check begin invoke on user interface.
        /// </summary>
        /// <param name="dispatcher"> The dispatcher. </param>
        /// <param name="action"> The action. </param>
        public static void CheckBeginInvokeOnUi(this Dispatcher dispatcher, Action action)
        {
            if (dispatcher.CheckAccess())
            {
                action();
            }
            else
            {
                dispatcher.BeginInvoke(action);
            }
        }
    }
}
