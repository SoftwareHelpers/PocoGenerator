// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EnumerableExtensions.cs" company="Company">
//   Copyrights 2014.
// </copyright>
// <summary>
//   The numerable extensions.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace PocoGenerator.Base.Common
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.Common;
    using System.Linq;

    /// <summary>
    /// The numerable extensions.
    /// </summary>
    public static class EnumerableExtensions
    {
        /// <summary>
        /// The from data reader.
        /// </summary>
        /// <param name="list"> The list. </param>
        /// <param name="dataReader"> The data reader. </param>
        /// <typeparam name="T"> Type of parameter</typeparam>
        /// <returns> Returns list </returns>
        public static IEnumerable<T> FromDataReader<T>(this IEnumerable<T> list, DbDataReader dataReader)
        {
            var reflection = new Reflection();
            var objectList = new List<object>();

            while (dataReader.Read())
            {
                //// Create an instance of the object needed.
                //// The instance is created by obtaining the object type T of the object
                //// list, which is the object that calls the extension method
                //// Type T is inferred and is instantiated
                var instance = Activator.CreateInstance(list.GetType().GetGenericArguments()[0]);

                //// Loop all the fields of each row of dataReader, and through the object
                //// reflector (first step method) fill the object instance with the datareader values
                var schemaTable = dataReader.GetSchemaTable();
                if (schemaTable != null)
                {
                    foreach (DataRow drow in schemaTable.Rows)
                    {
                        reflection.FillObjectWithProperty(ref instance, drow.ItemArray[0].ToString(), dataReader[drow.ItemArray[0].ToString()]);
                    }
                }

                //// Add object instance to list
                objectList.Add(instance);
            }

            return objectList.Select(item => (T)Convert.ChangeType(item, typeof(T))).ToList();
        }

        /// <summary>
        /// The from data reader.
        /// </summary>
        /// <param name="list"> The list. </param>
        /// <param name="dataReader"> The data reader. </param>
        /// <typeparam name="T"> Type of parameter</typeparam>
        /// <returns> Returns list </returns>
        public static IEnumerable<T> FromDataReaderEx<T>(this IEnumerable<T> list, DbDataReader dataReader)
        {
            var objectList = new List<object>();
            for (var fieldCount = 0; fieldCount < dataReader.FieldCount; fieldCount++)
            {
                var fieldName = dataReader.GetName(fieldCount);
                ////objectList.Add(new FieldDetails { Name = fieldName, TypeName = dataReader.GetFieldType(fieldCount), IsPrimaryKey = false });
            }

            return objectList.Select(item => (T)Convert.ChangeType(item, typeof(T))).ToList();
        }
    }
}
