using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PocoGenerator.Base.DatabaseManager
{
    using System.Data;
    using System.Data.Common;

    using PocoGenerator.Base.Common;
    using PocoGenerator.Base.Models;

    /// <summary>
    /// The database operations.
    /// </summary>
    public static class DatabaseOperations
    {
        /// <summary>
        /// The execute non query.
        /// </summary>
        /// <param name="commandText"> The command text. </param>
        /// <param name="databaseConnectionEnum"> The database connection enumeration. </param>
        /// <returns> The <see cref="int"/>. </returns>
        public static int ExecuteNonQuery(string commandText, DatabaseConnectionEnum databaseConnectionEnum)
        {
            using (var connectionFactory = new DatabaseConnection(databaseConnectionEnum))
            {
                using (var connection = connectionFactory.GetConnection())
                {
                    if (connection.State.Equals(ConnectionState.Open))
                    {
                        connection.Close();
                    }

                    connection.Open();
                    using (var command = connection.CreateCommand())
                    {
                        command.Connection = connection;
                        command.CommandText = commandText;
                        command.CommandType = CommandType.Text;
                        return command.ExecuteNonQuery();
                    }
                }
            }
        }

        /// <summary>
        /// The execute reader.
        /// </summary>
        /// <param name="commandText"> The command text. </param>
        /// <param name="databaseConnectionEnum"> The database connection enumeration. </param>
        /// <typeparam name="T">Type of parameter </typeparam>
        /// <returns> The <see cref="Dictionary{TKey,TValue}.Enumerator"/>. </returns>
        public static IEnumerator<T> ExecuteReader<T>(string commandText, DatabaseConnectionEnum databaseConnectionEnum)
        {
            using (var connectionFactory = new DatabaseConnection(databaseConnectionEnum))
            {
                using (var connection = connectionFactory.GetConnection())
                {
                    if (connection.State.Equals(ConnectionState.Open))
                    {
                        connection.Close();
                    }

                    connection.Open();
                    using (var command = connection.CreateCommand())
                    {
                        command.Connection = connection;
                        command.CommandText = commandText;
                        command.CommandType = CommandType.Text;
                        var dataReader = command.ExecuteReader();
                        return new List<T>().FromDataReader((DbDataReader)dataReader).GetEnumerator();
                    }
                }
            }
        }

        /// <summary>
        /// The execute reader ex.
        /// </summary>
        /// <param name="commandText"> The command text. </param>
        /// <param name="databaseConnectionEnum"> The database connection enumeration. </param>
        /// <returns> The <see cref="IDataReader"/>. </returns>
        public static List<FieldDetails> ExecuteReaderEx(string commandText, DatabaseConnectionEnum databaseConnectionEnum)
        {
            using (var connectionFactory = new DatabaseConnection(databaseConnectionEnum))
            {
                using (var connection = connectionFactory.GetConnection())
                {
                    if (connection.State.Equals(ConnectionState.Open))
                    {
                        connection.Close();
                    }

                    connection.Open();
                    using (var command = connection.CreateCommand())
                    {
                        command.Connection = connection;
                        command.CommandText = commandText;
                        command.CommandType = CommandType.Text;
                        var reader = command.ExecuteReader();
                        var result = new List<FieldDetails>();
                        for (var fieldCount = 0; fieldCount < reader.FieldCount; fieldCount++)
                        {
                            var fieldName = reader.GetName(fieldCount);
                            result.Add(new FieldDetails { Name = fieldName, TypeName = reader.GetFieldType(fieldCount), IsPrimaryKey = false });
                        }

                        return result;
                    }
                }
            }
        }
    }
}
