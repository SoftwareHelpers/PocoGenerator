using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PocoGenerator.Base.ProjectManager
{
    using System.Diagnostics;
    using System.IO;
    using System.Runtime.InteropServices;

    using EnvDTE;

    using PocoGenerator.Base.Common;

    public static class ProjectExtensions
    {
        public const int S_OK = 0;
        public const string WebApplicationProjectTypeGuid = "{349C5851-65DF-11DA-9384-00065B846F21}";
        public const string WebSiteProjectTypeGuid = "{E24C65DC-7377-472B-9ABA-BC803B73C61A}";
        public const string VsProjectItemKindPhysicalFolder = "{6BB5F8EF-4483-11D3-8BCF-00C04F8EC28C}";

        public static string GetTargetName(this Project project)
        {
            DebugCheck.NotNull(project);

            return project.GetPropertyValue<string>("AssemblyName");
        }

        public static string GetProjectDir(this Project project)
        {
            DebugCheck.NotNull(project);

            return project.GetPropertyValue<string>("FullPath");
        }

        // <summary>
        // Gets the string abbreviation for the language of a VS project.
        // </summary>
        public static string GetLanguage(this Project project)
        {
            DebugCheck.NotNull(project);

            ////switch (project.CodeModel.Language)
            ////{
            ////    case CodeModelLanguageConstants.vsCMLanguageVB:
            ////        return "vb";

            ////    case CodeModelLanguageConstants.vsCMLanguageCSharp:
            ////        return "cs";
            ////}

            return "cs";
        }

        // <summary>
        // Gets the root namespace configured for a VS project.
        // </summary>
        public static string GetRootNamespace(this Project project)
        {
            DebugCheck.NotNull(project);

            return project.GetPropertyValue<string>("RootNamespace");
        }

        public static string GetFileName(this Project project, string projectItemName)
        {
            DebugCheck.NotNull(project);
            DebugCheck.NotEmpty(projectItemName);

            ProjectItem projectItem;

            try
            {
                projectItem = project.ProjectItems.Item(projectItemName);
            }
            catch
            {
                return Path.Combine(project.GetProjectDir(), projectItemName);
            }

            Debug.Assert(projectItem.FileCount == 1);

            return projectItem.FileNames[0];
        }

        public static void EditFile(this Project project, string path)
        {
            DebugCheck.NotNull(project);
            DebugCheck.NotEmpty(path);
            Debug.Assert(!Path.IsPathRooted(path));

            var absolutePath = Path.Combine(project.GetProjectDir(), path);
            var dte = project.DTE;

            if (dte.SourceControl != null
                && dte.SourceControl.IsItemUnderSCC(absolutePath)
                && !dte.SourceControl.IsItemCheckedOut(absolutePath))
            {
                dte.SourceControl.CheckOutItem(absolutePath);
            }
        }

        public static void AddFile(this Project project, string path, string contents)
        {
            DebugCheck.NotNull(project);
            DebugCheck.NotEmpty(path);
            Debug.Assert(!Path.IsPathRooted(path));

            var absolutePath = Path.Combine(project.GetProjectDir(), path);

            project.EditFile(path);
            Directory.CreateDirectory(Path.GetDirectoryName(absolutePath));
            File.WriteAllText(absolutePath, contents);

            project.AddFile(path);
        }

        public static void AddFile(this Project project, string path)
        {
            DebugCheck.NotNull(project);
            DebugCheck.NotEmpty(path);
            Debug.Assert(!Path.IsPathRooted(path));

            var directory = Path.GetDirectoryName(path);
            var fileName = Path.GetFileName(path);
            var projectDir = project.GetProjectDir();
            var absolutePath = Path.Combine(projectDir, path);

            var projectItems
                = directory
                    .Split(Path.DirectorySeparatorChar)
                    .Aggregate(
                        project.ProjectItems,
                        (pi, dir) =>
                        {
                            Debug.Assert(pi != null);
                            Debug.Assert(pi.Kind == VsProjectItemKindPhysicalFolder);

                            projectDir = Path.Combine(projectDir, dir);

                            try
                            {
                                var projectItem = pi.Item(dir);

                                return projectItem.ProjectItems;
                            }
                            catch
                            {
                            }

                            return pi.AddFromDirectory(projectDir).ProjectItems;
                        });

            try
            {
                projectItems.Item(fileName);
            }
            catch
            {
                projectItems.AddFromFileCopy(absolutePath);
            }
        }

        public static bool TryBuild(this Project project)
        {
            var dte = project.DTE;
            var configuration = dte.Solution.SolutionBuild.ActiveConfiguration.Name;

            dte.Solution.SolutionBuild.BuildProject(configuration, project.UniqueName, true);

            return dte.Solution.SolutionBuild.LastBuildInfo == 0;
        }

        private static T GetPropertyValue<T>(this Project project, string propertyName)
        {
            DebugCheck.NotNull(project);
            DebugCheck.NotEmpty(propertyName);

            var property = project.Properties.Item(propertyName);

            if (property == null)
            {
                return default(T);
            }

            return (T)property.Value;
        }

        private static T GetConfigurationPropertyValue<T>(this Project project, string propertyName)
        {
            DebugCheck.NotNull(project);
            DebugCheck.NotEmpty(propertyName);

            var property = project.ConfigurationManager.ActiveConfiguration.Properties.Item(propertyName);

            if (property == null)
            {
                return default(T);
            }

            return (T)property.Value;
        }
    }
}
