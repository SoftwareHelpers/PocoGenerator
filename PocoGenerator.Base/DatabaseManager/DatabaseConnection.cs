using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PocoGenerator.Base.DatabaseManager
{
    using System.Configuration;
    using System.Data;
    using System.Data.Common;

    using Npgsql;

    using PocoGenerator.Base.Common;

    /// <summary>
    /// The database connection.
    /// </summary>
    public class DatabaseConnection : IDbConnection
    {
        /// <summary>
        /// The db connection.
        /// </summary>
        private IDbConnection databaseConnection;

        /// <summary>
        /// The connection string.
        /// </summary>
        private string connectionString;

        /// <summary>
        /// Initializes a new instance of the <see cref="DatabaseConnection"/> class.
        /// </summary>
        /// <param name="databaseConnectionEnum">
        /// The database connection enumeration.
        /// </param>
        public DatabaseConnection(DatabaseConnectionEnum databaseConnectionEnum)
        {
            if (databaseConnectionEnum.Equals(DatabaseConnectionEnum.Sql))
            {
                this.CreateSqlConnection();
            }
            else if (databaseConnectionEnum.Equals(DatabaseConnectionEnum.Postgre))
            {
                this.CreatePostgreConnection();
            }
        }

        /// <summary>
        /// Gets or sets the string used to open a database.
        /// </summary>
        /// <returns>
        /// A string containing connection settings.
        /// </returns>
        public string ConnectionString
        {
            get
            {
                return this.connectionString;
            }

            set
            {
                this.connectionString = value;
            }
        }

        /// <summary>
        /// Gets the time to wait while trying to establish a connection before terminating the attempt and generating an error.
        /// </summary>
        /// <returns>
        /// The time (in seconds) to wait for a connection to open. The default value is 15 seconds.
        /// </returns>
        public int ConnectionTimeout
        {
            get
            {
                return this.databaseConnection.ConnectionTimeout;
            }
        }

        /// <summary>
        /// Gets the name of the current database or the database to be used after a connection is opened.
        /// </summary>
        /// <returns>
        /// The name of the current database or the name of the database to be used once a connection is open. The default value is an empty string.
        /// </returns>
        public string Database
        {
            get
            {
                return this.databaseConnection.Database;
            }
        }

        /// <summary>
        /// Gets the current state of the connection.
        /// </summary>
        /// <returns> One of the <see cref="T:System.Data.ConnectionState"/> values. </returns>
        public ConnectionState State
        {
            get
            {
                return this.databaseConnection.State;
            }
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            this.databaseConnection.Dispose();
        }

        /// <summary>
        /// Begins a database transaction.
        /// </summary>
        /// <returns>
        /// An object representing the new transaction.
        /// </returns>
        public IDbTransaction BeginTransaction()
        {
            return this.databaseConnection.BeginTransaction();
        }

        /// <summary>
        /// Begins a database transaction with the specified <see cref="T:System.Data.IsolationLevel"/> value.
        /// </summary>
        /// <returns>
        /// An object representing the new transaction.
        /// </returns>
        /// <param name="il">One of the <see cref="T:System.Data.IsolationLevel"/> values. </param>
        public IDbTransaction BeginTransaction(IsolationLevel il)
        {
            return this.databaseConnection.BeginTransaction(il);
        }

        /// <summary>
        /// Closes the connection to the database.
        /// </summary>
        public void Close()
        {
            this.databaseConnection.Close();
        }

        /// <summary>
        /// Changes the current database for an open Connection object.
        /// </summary>
        /// <param name="databaseName">The name of the database to use in place of the current database. </param>
        public void ChangeDatabase(string databaseName)
        {
        }

        /// <summary>
        /// Creates and returns a Command object associated with the connection.
        /// </summary>
        /// <returns>
        /// A Command object associated with the connection.
        /// </returns>
        public IDbCommand CreateCommand()
        {
            return this.databaseConnection.CreateCommand();
        }

        /// <summary>
        /// Opens a database connection with the settings specified by the ConnectionString property of the provider-specific Connection object.
        /// </summary>
        public void Open()
        {
            if (this.databaseConnection == null)
            {
                return;
            }

            this.databaseConnection.Open();
        }

        /// <summary>
        /// The get connection.
        /// </summary>
        /// <returns>
        /// The <see cref="IDbConnection"/>.
        /// </returns>
        public IDbConnection GetConnection()
        {
            return this.databaseConnection;
        }

        /// <summary>
        /// The open sql connection.
        /// </summary>
        private void CreateSqlConnection()
        {
            var databaseProviderFactories = DbProviderFactories.GetFactory("System.Data.SqlClient");
            this.databaseConnection = databaseProviderFactories.CreateConnection();

            if (this.databaseConnection == null)
            {
                return;
            }

            this.databaseConnection.ConnectionString = ConfigurationManager.ConnectionStrings["SqlDb"].ConnectionString;
            this.connectionString = this.databaseConnection.ConnectionString;
        }

        /// <summary>
        /// The open postgre connection.
        /// </summary>
        private void CreatePostgreConnection()
        {
            var postgreConnectionString = ConfigurationManager.ConnectionStrings["PostgreDb"].ConnectionString;
            this.databaseConnection = new NpgsqlConnection(postgreConnectionString);
            this.connectionString = this.databaseConnection.ConnectionString;
        }
    }
}
