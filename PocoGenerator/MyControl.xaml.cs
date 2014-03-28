using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Company.PocoGenerator
{
    using System.Globalization;
    using System.IO;
    using System.Runtime.InteropServices;
    using System.Windows.Threading;

    using EnvDTE;

    using global::PocoGenerator.Base.CodeGenerator;
    using global::PocoGenerator.Base.Common;
    using global::PocoGenerator.Base.DatabaseManager;
    using global::PocoGenerator.Base.Models;
    using global::PocoGenerator.Base.ProjectManager;

    /// <summary>
    /// Interaction logic for MyControl.xaml
    /// </summary>
    public partial class MyControl : UserControl
    {
        /// <summary>
        /// The tables list.
        /// </summary>
        private List<TableName> tablesList;

        /// <summary>
        /// The fields list.
        /// </summary>
        private List<FieldDetails> fieldsList;
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
        /// The target vs version.
        /// </summary>
        private const int targetVsVersion = 12;
        /// <summary>
        /// The selected table name.
        /// </summary>
        private string selectedTableName;

        /// <summary>
        /// The project.
        /// </summary>
        private Project project;

        public MyControl()
        {
            InitializeComponent();
            var dte2 = (DTE)Marshal.GetActiveObject(string.Format(CultureInfo.InvariantCulture, "VisualStudio.DTE.{0}.0", targetVsVersion));
            this.project = dte2.Solution
                .Projects
                .OfType<Project>()
                .SelectMany(p => p.ProjectItems.OfType<ProjectItem>())
                .Select(p => p.SubProject).FirstOrDefault();

            if (this.project != null)
            {
                this.Dispatcher.CheckBeginInvokeOnUi(
                    () => { this.ProjectPath.Text = this.project.GetProjectDir(); });
            }

            this.PopulateDatabases();
        }


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

        private void button1_Click(object sender, RoutedEventArgs e)
        {
            this.OnGenerateCommand();
        }

        private void FieldsList_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void TablesList_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            this.selectedTableName = ((TableName)this.TablesList.SelectedItem).TABLE_NAME;
            this.PopulateFields(this.selectedTableName);
        }

        private void DatabaseList_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            this.PopulateTables();
        }

        /// <summary>
        /// The on generate command.
        /// </summary>
        private void OnGenerateCommand()
        {
            var rootNamespace = this.project.GetRootNamespace();
            var code = this.codeGenerator.CodeWriter(rootNamespace, this.selectedTableName, this.FieldsList.ItemsSource as IEnumerable<FieldDetails>);
            var classFilePath = Path.Combine(this.FullPath, string.Format(CultureInfo.InvariantCulture, "{0}.cs", this.selectedTableName));
            File.WriteAllText(classFilePath, code);
            this.project.ProjectItems.AddFromFile(classFilePath);
        }

        /// <summary>
        /// The populate databases.
        /// </summary>
        private void PopulateDatabases()
        {
            var connect = DatabaseOperations.ExecuteReader<DatabaseName>("select Name from master.dbo.sysdatabases", DatabaseConnectionEnum.Sql);
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
            var connect = DatabaseOperations.ExecuteReader<TableName>("SELECT TABLE_NAME FROM INFORMATION_SCHEMA.TABLES", DatabaseConnectionEnum.Sql);
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
                        DatabaseConnectionEnum.Sql);
                });
        }
    }
}