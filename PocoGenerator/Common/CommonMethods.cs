// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CommonMethods.cs" company="Company">
//   Copyrights 2014.
// </copyright>
// <summary>
//   The common methods.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Company.PocoGenerator
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Globalization;
    using System.Runtime.InteropServices;
    using System.Runtime.InteropServices.ComTypes;

    using EnvDTE;

    using Microsoft.VisualStudio;
    using Microsoft.VisualStudio.Shell.Interop;

    /// <summary>
    /// The common methods.
    /// </summary>
    public static class CommonMethods
    {
        /// <summary>
        /// The target visual studio version.
        /// </summary>
        public const int targetVsVersion = 12;

        /// <summary>
        /// The get current solution.
        /// </summary>
        /// <param name="processId"> The process id. </param>
        /// <returns> The <see cref="DTE"/>. </returns>
        [SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1121:UseBuiltInTypeAlias", Justification = "Reviewed. Suppression is OK here.")]
        public static DTE GetDTE(int processId)
        {
            var progId = String.Format(CultureInfo.InvariantCulture, "VisualStudio.DTE.{0}.0:", targetVsVersion) + processId.ToString(CultureInfo.InvariantCulture);
            object runningObject = null;

            IBindCtx bindCtx = null;
            IRunningObjectTable rot = null;
            IEnumMoniker enumMonikers = null;

            try
            {
                Marshal.ThrowExceptionForHR(CreateBindCtx(reserved: 0, ppbc: out bindCtx));
                bindCtx.GetRunningObjectTable(out rot);
                rot.EnumRunning(out enumMonikers);

                var moniker = new IMoniker[1];
                IntPtr numberFetched = IntPtr.Zero;
                while (enumMonikers.Next(1, moniker, numberFetched) == 0)
                {
                    IMoniker runningObjectMoniker = moniker[0];

                    string name = null;

                    try
                    {
                        if (runningObjectMoniker != null)
                        {
                            runningObjectMoniker.GetDisplayName(bindCtx, null, out name);
                        }
                    }
                    catch (UnauthorizedAccessException)
                    {
                        // Do nothing, there is something in the ROT that we do not have access to.
                    }

                    if (!String.IsNullOrEmpty(name) && String.Equals(name, progId, StringComparison.Ordinal))
                    {
                        Marshal.ThrowExceptionForHR(rot.GetObject(runningObjectMoniker, out runningObject));
                        break;
                    }
                }
            }
            finally
            {
                if (enumMonikers != null)
                {
                    Marshal.ReleaseComObject(enumMonikers);
                }

                if (rot != null)
                {
                    Marshal.ReleaseComObject(rot);
                }

                if (bindCtx != null)
                {
                    Marshal.ReleaseComObject(bindCtx);
                }
            }

            return (DTE)runningObject;
        }

        /// <summary>
        /// The get projects.
        /// </summary>
        /// <param name="solution">
        /// The solution.
        /// </param>
        /// <returns>
        /// The <see cref="IEnumerable"/>.
        /// </returns>
        public static IEnumerable<EnvDTE.Project> GetProjects(IVsSolution solution)
        {
            foreach (IVsHierarchy hier in GetProjectsInSolution(solution))
            {
                var project = GetDTEProject(hier);
                if (project != null)
                {
                    yield return project;
                }
            }
        }

        /// <summary>
        /// The get projects in solution.
        /// </summary>
        /// <param name="solution">
        /// The solution.
        /// </param>
        /// <param name="flags">
        /// The flags.
        /// </param>
        /// <returns>
        /// The <see cref="IEnumerable"/>.
        /// </returns>
        public static IEnumerable<IVsHierarchy> GetProjectsInSolution(IVsSolution solution, __VSENUMPROJFLAGS flags = __VSENUMPROJFLAGS.EPF_LOADEDINSOLUTION)
        {
            if (solution == null)
            {
                yield break;
            }

            IEnumHierarchies enumHierarchies;
            var guid = Guid.Empty;
            ErrorHandler.ThrowOnFailure(solution.GetProjectEnum((uint)flags, ref guid, out enumHierarchies));
            if (enumHierarchies == null)
            {
                yield break;
            }

            var hierarchy = new IVsHierarchy[1];
            uint fetched;
            while (enumHierarchies.Next(1, hierarchy, out fetched) == VSConstants.S_OK && fetched == 1)
            {
                if (hierarchy.Length > 0 && hierarchy[0] != null)
                {
                    yield return hierarchy[0];
                }
            }
        }

        /// <summary>
        /// The get dte project.
        /// </summary>
        /// <param name="hierarchy">
        /// The hierarchy.
        /// </param>
        /// <returns>
        /// The <see cref="Project"/>.
        /// </returns>
        /// <exception cref="ArgumentNullException">Argument null exception </exception>
        public static Project GetDTEProject(IVsHierarchy hierarchy)
        {
            if (hierarchy == null)
            {
                throw new ArgumentNullException("hierarchy");
            }

            object obj;
            hierarchy.GetProperty(VSConstants.VSITEMID_ROOT, (int)__VSHPROPID.VSHPROPID_ExtObject, out obj);
            return obj as Project;
        }

        [DllImport("ole32.dll")]
        private static extern int CreateBindCtx(uint reserved, out IBindCtx ppbc);
    }
}
