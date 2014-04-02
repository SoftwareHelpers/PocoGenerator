// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MyControl.xaml.cs" company="Company">
//   Copyrights 2014.
// </copyright>
// <summary>
//   The my control.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Company.PocoGenerator
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Windows;
    using System.Windows.Controls;

    using EnvDTE;

    using Microsoft.VisualStudio.Shell;
    using Microsoft.VisualStudio.Shell.Interop;

    using global::PocoGenerator.Base.CodeGenerator;

    using global::PocoGenerator.Base.Common;

    using global::PocoGenerator.Base.DatabaseManager;

    using global::PocoGenerator.Base.Models;

    using global::PocoGenerator.Base.ProjectManager;

    using Task = System.Threading.Tasks.Task;

    /// <summary>
    /// The my control.
    /// </summary>
    public partial class MyControl : UserControl
    {
        #region Variables

        /// <summary>
        /// The remove directory list.
        /// </summary>
        private readonly List<string> removeDirList = new List<string> { "bin", "Properties", "obj" };

        /// <summary>
        /// The server name.
        /// </summary>
        private string serverName, userName, password;

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
        private Project projectForPoco;

        /// <summary>
        /// The project.
        /// </summary>
        private Project projectForDto;

        /// <summary>
        /// The project.
        /// </summary>
        private Project projectForMapper;

        /// <summary>
        /// The project.
        /// </summary>
        private Project projectForRepo;

        /// <summary>
        /// The database connection enumeration.
        /// </summary>
        private DatabaseConnectionEnum databaseConnectionEnum;

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
                    this.GridGenerate.IsEnabled = false;
                    ////this.dte = (DTE)Marshal.GetActiveObject(string.Format(CultureInfo.InvariantCulture, "VisualStudio.DTE.{0}.0", CommonMethods.targetVsVersion));
                    ////var projects = this.dte.Solution.Projects.OfType<Project>()
                    ////                .SelectMany(p => p.ProjectItems.OfType<ProjectItem>())
                    ////                .Select(p => p.SubProject)
                    ////                .Where(s => s != null).ToList();

                    var solution = (IVsSolution)Package.GetGlobalService(typeof(IVsSolution));
                    var projects = CommonMethods.GetProjects(solution).Where(item => item.FullName != string.Empty).ToList();
                    this.ComboBoxPocoProjects.ItemsSource = projects;
                    this.ComboBoxDtoProjects.ItemsSource = projects;
                    this.ComboBoxMapperProjects.ItemsSource = projects;
                    this.ComboBoxRepoProjects.ItemsSource = projects;
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
            var combo = (ComboBox)sender;
            var tempProject = combo.SelectedItem as Project;
            var directories = Directory.GetDirectories(tempProject.GetProjectDir()).Select(dir => dir.Split(new[] { '\\' }, StringSplitOptions.RemoveEmptyEntries)).Select(tt => tt[tt.Length - 1]).ToList();
            var result = directories.ToList();
            foreach (var dir in directories.Where(dir => this.removeDirList.Contains(dir)))
            {
                result.Remove(dir);
            }

            this.Dispatcher.CheckBeginInvokeOnUi(
                () =>
                {
                    switch (combo.Name)
                    {
                        case "ComboBoxPocoProjects":
                            this.ComboBoxPocoDirectories.ItemsSource = result;
                            this.projectForPoco = combo.SelectedItem as Project;
                            break;
                        case "ComboBoxDtoProjects":
                            this.ComboBoxDtoDirectories.ItemsSource = result;
                            this.projectForDto = combo.SelectedItem as Project;
                            break;
                        case "ComboBoxMapperProjects":
                            this.ComboBoxMapperDirectories.ItemsSource = result;
                            this.projectForMapper = combo.SelectedItem as Project;
                            break;
                        case "ComboBoxRepoProjects":
                            this.ComboBoxRepoDirectories.ItemsSource = result;
                            this.projectForRepo = combo.SelectedItem as Project;
                            break;
                    }
                });

            ////this.project = this.ComboBoxPocoProjects.SelectedItem as Project;
            ////var directories = Directory.GetDirectories(this.project.GetProjectDir()).Select(dir => dir.Split(new[] { '\\' }, StringSplitOptions.RemoveEmptyEntries)).Select(tt => tt[tt.Length - 1]).ToList();
            ////var result = directories.ToList();
            ////foreach (var dir in directories.Where(dir => this.removeDirList.Contains(dir)))
            ////{
            ////    result.Remove(dir);
            ////}

            ////this.ComboBoxPocoDirectories.ItemsSource = result;
        }

        /// <summary>
        /// The button test connection_ on click.
        /// </summary>
        /// <param name="sender"> The sender. </param>
        /// <param name="e"> The e. </param>
        private void OnTestConnectionButtonClick(object sender, RoutedEventArgs e)
        {
            Task.Run(() => this.Dispatcher.CheckBeginInvokeOnUi(
                () =>
                {
                    this.serverName = this.TxtServerName.Text;
                    this.userName = this.TxtUserName.Text;
                    this.password = this.TxtPassword.Text;

                    if (string.IsNullOrEmpty(this.serverName))
                    {
                        this.TextBlockConnectionError.Text = "Server name cannot be blank.";
                        this.TxtServerName.Focus();
                        return;
                    }

                    if (string.IsNullOrEmpty(this.userName))
                    {
                        this.TextBlockConnectionError.Text = "User name cannot be blank.";
                        this.TxtUserName.Focus();
                        return;
                    }

                    if (string.IsNullOrEmpty(this.password))
                    {
                        this.TextBlockConnectionError.Text = "Password cannot be blank.";
                        this.TxtPassword.Focus();
                        return;
                    }

                    var message = string.Empty;
                    var result = false;
                    this.DatabaseList.ItemsSource = new List<DatabaseName>();
                    this.TablesList.ItemsSource = new List<TableName>();
                    this.FieldsList.ItemsSource = new List<FieldDetails>();
                    try
                    {
                        result = DatabaseOperations.TestConnection(this.databaseConnectionEnum, this.serverName, this.userName, this.password);
                    }
                    catch (Exception exception)
                    {
                        message = exception.Message;
                    }

                    this.GridGenerate.IsEnabled = result;
                    if (result)
                    {
                        this.PopulateDatabases();
                        message = "Successfully connected.";
                    }

                    this.TextBlockConnectionError.Text = message;
                }));
        }
        #endregion

        #region Methods

        /// <summary>
        /// The on generate command.
        /// </summary>
        private void OnGenerateCommand()
        {
            Task.Run(() => this.Dispatcher.CheckBeginInvokeOnUi(
            () =>
            {
                try
                {
                    if (this.FieldsList.ItemsSource == null)
                    {
                        MessageBox.Show("No table is selected. Plesase select a table");
                        this.FieldsList.Focus();
                        return;
                    }

                    var resultPoco = this.GeneratePocoClasses();
                    if (resultPoco != ResultCode.ResultCode_SuccessfullyGenerated)
                    {
                        this.ShowErrorMessage(resultPoco);
                        return;
                    }

                    var resultDto = this.GenerateDtoClasses();
                    if (resultDto != ResultCode.ResultCode_SuccessfullyGenerated)
                    {
                        this.ShowErrorMessage(resultDto);
                        return;
                    }

                    var resultMapper = this.GenerateMapperClasses();
                    if (resultMapper != ResultCode.ResultCode_SuccessfullyGenerated)
                    {
                        this.ShowErrorMessage(resultMapper);
                        return;
                    }

                    var resultRepo = this.GenerateRepoClasses();
                    if (resultMapper != ResultCode.ResultCode_SuccessfullyGenerated)
                    {
                        this.ShowErrorMessage(resultRepo);
                    }
                }
                catch (Exception exception)
                {
                    var errorMessage = string.Format(CultureInfo.InvariantCulture, "{0}{1}{2}", "Unable to generate classes", Environment.NewLine, exception.Message);
                    MessageBox.Show(errorMessage);
                }
            }));

            ////var rootNamespace = this.projectForPoco.GetRootNamespace();
            ////var list = this.FieldsList.ItemsSource as IEnumerable<FieldDetails>;
            ////var code = this.codeGenerator.CodeWriter(rootNamespace, this.selectedTableName, list);
            ////var projectDir = this.projectForPoco.GetProjectDir();
            ////var dir = Convert.ToString(this.ComboBoxPocoDirectories.SelectedItem);
            ////this.FullPath = Path.Combine(projectDir, dir);
            ////var classFilePath = Path.Combine(this.FullPath, string.Format(CultureInfo.InvariantCulture, "{0}.cs", this.selectedTableName));
            ////File.WriteAllText(classFilePath, code);
            ////this.projectForPoco.ProjectItems.AddFromFile(classFilePath);
        }

        /// <summary>
        /// The show error message.
        /// </summary>
        /// <param name="resultCode">
        /// The result code.
        /// </param>
        private void ShowErrorMessage(int resultCode)
        {
            switch (resultCode)
            {
                case ResultCode.ResultCode_SuccessfullyGenerated:
                    MessageBox.Show(ResultCode.ResultCode_SuccessfullyGenerated_Message);
                    break;
                case ResultCode.ResultCode_ProjectNotSelected:
                    MessageBox.Show(ResultCode.ResultCode_ProjectNotSelected_Message);
                    break;
                case ResultCode.ResultCode_UnknownError:
                    MessageBox.Show(ResultCode.ResultCode_UnknownError_Message);
                    break;
            }
        }

        /// <summary>
        /// The generate data objects classes.
        /// </summary>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        private int GenerateDtoClasses()
        {
            if (this.projectForDto == null)
            {
                MessageBox.Show("No DTO project is selected. Plesase select a project");
                return ResultCode.ResultCode_ProjectNotSelected;
            }

            var list = this.FieldsList.ItemsSource as IEnumerable<FieldDetails>;
            return this.codeGenerator.CodeWriter(this.projectForDto, this.selectedTableName, list, this.ComboBoxDtoDirectories.SelectedItem.ToString());
        }

        /// <summary>
        /// The generate mapper classes.
        /// </summary>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        private int GenerateMapperClasses()
        {
            if (this.projectForMapper == null)
            {
                MessageBox.Show("No Mapper project is selected. Plesase select a project");
                return ResultCode.ResultCode_ProjectNotSelected;
            }

            var list = this.FieldsList.ItemsSource as IEnumerable<FieldDetails>;
            return this.codeGenerator.CodeWriter(this.projectForMapper, this.selectedTableName, list, this.ComboBoxMapperDirectories.SelectedItem.ToString());
        }

        /// <summary>
        /// The generate repository classes.
        /// </summary>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        private int GenerateRepoClasses()
        {
            if (this.projectForRepo == null)
            {
                MessageBox.Show("No Repository project is selected. Plesase select a project");
                return ResultCode.ResultCode_ProjectNotSelected;
            }

            var list = this.FieldsList.ItemsSource as IEnumerable<FieldDetails>;
            return this.codeGenerator.CodeWriter(this.projectForRepo, this.selectedTableName, list, this.ComboBoxRepoDirectories.SelectedItem.ToString());
        }

        /// <summary>
        /// The generate poco classes.
        /// </summary>
        /// <returns> The <see cref="bool"/>. </returns>
        private int GeneratePocoClasses()
        {
            if (this.projectForPoco == null)
            {
                MessageBox.Show("No Poco project is selected. Plesase select a project");
                return ResultCode.ResultCode_ProjectNotSelected;
            }

            var list = this.FieldsList.ItemsSource as IEnumerable<FieldDetails>;
            var selectedDir = string.Empty;
            this.Dispatcher.CheckBeginInvokeOnUi(() => selectedDir = Convert.ToString(this.ComboBoxPocoDirectories.SelectedItem));
            return this.codeGenerator.CodeWriter(this.projectForPoco, this.selectedTableName, list, selectedDir);
        }

        /// <summary>
        /// The populate databases.
        /// </summary>
        private void PopulateDatabases()
        {
            var connect = DatabaseOperations.ExecuteReader<DatabaseName>("SELECT Name FROM master.dbo.sysdatabases", DatabaseConnectionEnum.Sql, this.serverName, this.userName, this.password);
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
                    this.FieldsList.ItemsSource = DatabaseOperations.ExecuteReaderEx("select * FROM " + tableName, DatabaseConnectionEnum.Sql, this.serverName, this.userName, this.password);
                });
        }
        #endregion
    }
}