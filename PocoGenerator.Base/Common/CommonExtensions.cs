using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PocoGenerator.Base.Common
{
    using System.Runtime.InteropServices;
    using System.Runtime.InteropServices.ComTypes;
    using System.Windows.Threading;

    public static class CustomExtensions
    {
        public static List<T> SaveRest<T>(this IEnumerator<T> enumerator)
        {
            var list = new List<T>();
            while (enumerator.MoveNext())
            {
                list.Add(enumerator.Current);
            }
            return list;
        }

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
