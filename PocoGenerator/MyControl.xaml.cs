using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace Company.PocoGenerator
{
    using System.Globalization;
    using System.IO;
    using System.Runtime.InteropServices;
    using EnvDTE;
    using global::PocoGenerator.Base.CodeGenerator;
    using global::PocoGenerator.Base.Common;
    using global::PocoGenerator.Base.DatabaseManager;
    using global::PocoGenerator.Base.Models;
    using global::PocoGenerator.Base.ProjectManager;

    /// <summary>
    /// The my control.
    /// </summary>
    public partial class MyControl : UserControl
    {
        #region Variables
        /// <summary>
        /// The target visual studio version.
        /// </summary>
        private const int targetVsVersion = 12;

        /// <summary>
        /// The server name.
        /// </summary>
        private string serverName, userName, password;

        /// <summary>
        /// The tables list.
        /// </summary>
        private List<TableName> tablesList;

        /// <summary>
        /// The fields list.
        /// </summary>
        private List<FieldDetails> fieldsList;

        /// <summary>
        /// The selected database name.
        /// </summary>
        private DatabaseName selectedDatabaseName;

        /// <summary>
        /// The code generator.
        /// </summary>
        private ICodeGenerator codeGenerator;

        /// <summary>
        /// The full path.
        /// </summary>
        private string fullPath;

        /// <summary>
        /// The selected table name.
        /// </summary>
        private string selectedTableName;

        /// <summary>
        /// The project.
        /// </summary>
        private Project project;

        /// <summary>
        /// The database connection enumeration.
        /// </summary>
        private DatabaseConnectionEnum databaseConnectionEnum;

        /// <summary>
        /// The remove dir list.
        /// </summary>
        private List<string> removeDirList = new List<string> { "bin", "Properties", "obj" };

        #endregion

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="MyControl"/> class.
        /// </summary>
        public MyControl()
        {
            this.InitializeComponent();
        } 
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the full path.
        /// </summary>
        public string FullPath
        {
            get
            {
                return this.fullPath;
            }

            set
            {
                this.fullPath = value;
            }
        } 
        #endregion

        #region Events
        /// <summary>
        /// The button 1_ click.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void OnGenerateButtonClick(object sender, RoutedEventArgs e)
        {
            this.OnGenerateCommand();
        }

        /// <summary>
        /// The fields list_ on selection changed.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void OnFieldsListSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
        }

        /// <summary>
        /// The tables list_ on selection changed.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void OnTablesListSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            this.selectedTableName = ((TableName)this.TablesList.SelectedItem).TABLE_NAME;
            this.PopulateFields(this.selectedTableName);
        }

        /// <summary>
        /// The database list_ on selection changed.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void OnDatabaseListSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            this.PopulateTables();
        }

        /// <summary>
        /// The my control_ on loaded.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            this.Dispatcher.CheckBeginInvokeOnUi(
                () =>
                {
                    var dte = (DTE)Marshal.GetActiveObject(string.Format(CultureInfo.InvariantCulture, "VisualStudio.DTE.{0}.0", targetVsVersion));
                    var projects = dte.Solution.Projects.OfType<Project>()
                                    .SelectMany(p => p.ProjectItems.OfType<ProjectItem>())
                                    .Select(p => p.SubProject)
                                    .Where(s => s != null);
                    this.ProjectsComboBox.ItemsSource = projects;
                    this.codeGenerator = new PocoCodeGenerator();
                    this.databaseConnectionEnum = this.RadioButtonSql.IsChecked != null
                                                  && Convert.ToBoolean(this.RadioButtonSql.IsChecked)
                                                      ? DatabaseConnectionEnum.Sql
                                                      : DatabaseConnectionEnum.Postgre;
                });
        }

        /// <summary>
        /// The projects combo box_ on selection changed.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void OnProjectsComboBoxSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            this.project = this.ProjectsComboBox.SelectedItem as Project;
            var directories = Directory.GetDirectories(this.project.GetProjectDir()).Select(dir => dir.Split(new char[] { '\\' }, StringSplitOptions.RemoveEmptyEntries)).Select(tt => tt[tt.Length - 1]).ToList();
            var result = directories.ToList();
            foreach (var dir in directories.Where(dir => this.removeDirList.Contains(dir)))
            {
                result.Remove(dir);
            }

            this.DirectoryListBox.ItemsSource = result;
        }

        /// <summary>
        /// The button test connection_ on click.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void OnTestConnectionButtonClick(object sender, RoutedEventArgs e)
        {
            this.Dispatcher.CheckBeginInvokeOnUi(
               () =>
               {
                   this.serverName = TxtServerName.Text;
                   this.userName = TxtUserName.Text;
                   this.password = TxtPassword.Text;

                   if (string.IsNullOrEmpty(this.serverName))
                   {
                       return;
                   }

                   if (string.IsNullOrEmpty(this.userName))
                   {
                       return;
                   }

                   if (string.IsNullOrEmpty(this.password))
                   {
                       return;
                   }

                   var result = DatabaseOperations.TestConnection(
                       this.databaseConnectionEnum,
                       this.serverName,
                       this.userName,
                       this.password);

                   this.GridGenerate.IsEnabled = result;
                   this.GridDetail.IsEnabled = result;
                   if (result)
                   {
                       this.PopulateDatabases();
                   }
               });
        }
        #endregion

        #region Methods
        /// <summary>
        /// The on generate command.
        /// </summary>
        private void OnGenerateCommand()
        {
            if (this.project == null)
            {
                this.Dispatcher.CheckBeginInvokeOnUi(
                    () =>
                    {
                        MessageBox.Show("No project is selected. Plesase select a project");
                        this.ProjectsComboBox.Focus();
                    });

                return;
            }


            if (this.FieldsList.ItemsSource == null)
            {
                MessageBox.Show("No table is selected. Plesase select a table");
                this.FieldsList.Focus();
                return;
            }

            var rootNamespace = this.project.GetRootNamespace();
            var list = this.FieldsList.ItemsSource as IEnumerable<FieldDetails>;
            var code = this.codeGenerator.CodeWriter(rootNamespace, this.selectedTableName, list);
            var projectDir = this.project.GetProjectDir();
            var dir = Convert.ToString(this.DirectoryListBox.SelectedItem);
            this.FullPath = Path.Combine(projectDir, dir);
            var classFilePath = Path.Combine(this.FullPath, string.Format(CultureInfo.InvariantCulture, "{0}.cs", this.selectedTableName));
            File.WriteAllText(classFilePath, code);
            this.project.ProjectItems.AddFromFile(classFilePath);
        }

        /// <summary>
        /// The populate databases.
        /// </summary>
        private void PopulateDatabases()
        {
            var connect = DatabaseOperations.ExecuteReader<DatabaseName>("select Name from master.dbo.sysdatabases", DatabaseConnectionEnum.Sql, this.serverName, this.userName, this.password);
            this.Dispatcher.CheckBeginInvokeOnUi(
                () =>
                {

                    this.DatabaseList.ItemsSource = connect.SaveRest();
                });
        }

        /// <summary>
        /// The populate tables.
        /// </summary>
        private void PopulateTables()
        {
            var connect = DatabaseOperations.ExecuteReader<TableName>("SELECT TABLE_NAME FROM INFORMATION_SCHEMA.TABLES", DatabaseConnectionEnum.Sql, this.serverName, this.userName, this.password);
            this.Dispatcher.CheckBeginInvokeOnUi(
                () => { this.TablesList.ItemsSource = connect.SaveRest(); });
        }

        /// <summary>
        /// The populate tables.
        /// </summary>
        /// <param name="tableName">
        /// The table Name.
        /// </param>
        private void PopulateFields(string tableName)
        {
            this.Dispatcher.CheckBeginInvokeOnUi(
                () =>
                {
                    this.FieldsList.ItemsSource = DatabaseOperations.ExecuteReaderEx(
                        "select * from " + tableName,
                        DatabaseConnectionEnum.Sql, this.serverName, this.userName, this.password);
                });
        } 
        #endregion
    }
}