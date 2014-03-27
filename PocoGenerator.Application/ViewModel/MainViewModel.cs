namespace PocoGenerator.Application.ViewModel
{
    using System.Collections.Generic;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Runtime.InteropServices;
    using System.Windows.Controls;
    using System.Windows.Forms.VisualStyles;
    using System.Xml.Serialization.Configuration;

    using EnvDTE;

    using GalaSoft.MvvmLight;
    using GalaSoft.MvvmLight.Command;

    using PocoGenerator.Base.CodeGenerator;
    using PocoGenerator.Base.Common;
    using PocoGenerator.Base.DatabaseManager;
    using PocoGenerator.Base.Models;
    using PocoGenerator.Base.ProjectManager;

    using ListBox = System.Windows.Controls.ListBox;

    /// <summary>
    /// The main view model.
    /// </summary>
    public class MainViewModel : ViewModelBase
    {
        /// <summary>
        /// The databases list.
        /// </summary>
        private List<DatabaseName> databasesList;

        /// <summary>
        /// The tables list.
        /// </summary>
        private List<TableName> tablesList;

        /// <summary>
        /// The fields list.
        /// </summary>
        private List<FieldDetails> fieldsList;

        /// <summary>
        /// Relay command for list box selection changed.
        /// </summary>
        private RelayCommand<SelectionChangedEventArgs> selectDatabaseCommand;

        /// <summary>
        /// Relay command for list box selection changed.
        /// </summary>
        private RelayCommand generateCommand;

        /// <summary>
        /// The selected database name.
        /// </summary>
        private DatabaseName selectedDatabaseName;

        /// <summary>
        /// The selected table name.
        /// </summary>
        private string selectedTableName;

        /// <summary>
        /// The directory browser command.
        /// </summary>
        private RelayCommand directoryBrowserCommand;

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
        /// The project.
        /// </summary>
        private Project project;

        /// <summary>
        /// Initializes a new instance of the <see cref="MainViewModel"/> class.
        /// </summary>
        /// <param name="codeGenerator">
        /// The code generator. 
        /// </param>
        public MainViewModel(ICodeGenerator codeGenerator)
        {
            this.codeGenerator = codeGenerator;
            var dte2 = (DTE)Marshal.GetActiveObject(string.Format(CultureInfo.InvariantCulture, "VisualStudio.DTE.{0}.0", targetVsVersion));
            this.project = dte2.Solution
                .Projects
                .OfType<Project>()
                .SelectMany(p => p.ProjectItems.OfType<ProjectItem>())
                .Select(p => p.SubProject).FirstOrDefault();

            if (this.project != null)
            {
                this.fullPath = this.project.GetProjectDir();
            }

            this.PopulateDatabases();
        }

        /// <summary>
        /// Gets select command.
        /// </summary>
        public RelayCommand<SelectionChangedEventArgs> SelectCommand
        {
            get
            {
                return this.selectDatabaseCommand
                       ?? (this.selectDatabaseCommand =
                           new RelayCommand<SelectionChangedEventArgs>(this.OnSelectionChanged));
            }
        }

        /// <summary>
        /// Gets select command.
        /// </summary>
        public RelayCommand GenerateCommand
        {
            get
            {
                return this.generateCommand
                       ?? (this.generateCommand =
                           new RelayCommand(this.OnGenerateCommand));
            }
        }

        /// <summary>
        /// Gets or sets the databases list.
        /// </summary>
        public List<DatabaseName> DatabasesList
        {
            get
            {
                return this.databasesList;
            }

            set
            {
                this.databasesList = value;
                this.RaisePropertyChanged(() => this.DatabasesList);
            }
        }

        /// <summary>
        /// Gets or sets the tables list.
        /// </summary>
        public List<TableName> TablesList
        {
            get
            {
                return this.tablesList;
            }

            set
            {
                this.tablesList = value;
                this.RaisePropertyChanged(() => this.TablesList);
            }
        }

        /// <summary>
        /// Gets or sets the fields list.
        /// </summary>
        public List<FieldDetails> FieldsList
        {
            get
            {
                return this.fieldsList;
            }

            set
            {
                this.fieldsList = value;
                this.RaisePropertyChanged(() => this.FieldsList);
            }
        }

        /// <summary>
        /// Gets or sets the selected database name.
        /// </summary>
        public DatabaseName SelectedDatabaseName
        {
            get
            {
                return this.selectedDatabaseName;
            }

            set
            {
                this.selectedDatabaseName = value;
                this.RaisePropertyChanged(() => this.SelectedDatabaseName);
            }
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
                this.RaisePropertyChanged(() => this.FullPath);
            }
        }

        /// <summary>
        /// Handles selection changed event of list box.
        /// </summary>
        /// <param name="eventArgs">Event arguments.</param>
        private void OnSelectionChanged(SelectionChangedEventArgs eventArgs)
        {
            var control = (ListBox)eventArgs.OriginalSource;
            if (control.SelectedItem == null)
            {
                return;
            }

            switch (control.SelectedItem.GetType().ToString())
            {
                case "PocoGenerator.Base.Models.DatabaseName":
                    this.PopulateTables();
                    break;
                case "PocoGenerator.Base.Models.TableName":
                    this.selectedTableName = ((TableName)control.SelectedItem).TABLE_NAME;
                    this.PopulateFields(this.selectedTableName);
                    break;
                case "PocoGenerator.Base.Models.FieldDetails":
                    var result = (FieldDetails)control.SelectedItem;
                    var index = this.FieldsList.IndexOf(result);
                    this.FieldsList[index].IsPrimaryKey = true;
                    break;
            }
        }

        /// <summary>
        /// The on generate command.
        /// </summary>
        private void OnGenerateCommand()
        {
            var rootNamespace = this.project.GetRootNamespace();
            var code = this.codeGenerator.CodeWriter(rootNamespace, this.selectedTableName, this.FieldsList);
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
            this.DatabasesList = connect.SaveRest();
        }

        /// <summary>
        /// The populate tables.
        /// </summary>
        private void PopulateTables()
        {
            var connect = DatabaseOperations.ExecuteReader<TableName>("SELECT TABLE_NAME FROM INFORMATION_SCHEMA.TABLES", DatabaseConnectionEnum.Sql);
            this.TablesList = connect.SaveRest();
        }

        /// <summary>
        /// The populate tables.
        /// </summary>
        /// <param name="tableName">
        /// The table Name.
        /// </param>
        private void PopulateFields(string tableName)
        {
            this.FieldsList = DatabaseOperations.ExecuteReaderEx("select * from " + tableName, DatabaseConnectionEnum.Sql);
        }
    }
}