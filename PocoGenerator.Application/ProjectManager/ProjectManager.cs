using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PocoGenerator.Base.ProjectManager
{
    using System.Globalization;
    using System.Runtime.InteropServices;

    using EnvDTE;

    /// <summary>
    /// The project manager.
    /// </summary>
    public class ProjectManager : IProjectManager
    {
        /// <summary>
        /// The target vs version.
        /// </summary>
        private const int TargetVsVersion = 12;

        /// <summary>
        /// Initializes a new instance of the <see cref="ProjectManager"/> class.
        /// </summary>
        public ProjectManager()
        {
            var dte2 = (DTE)Marshal.GetActiveObject(string.Format(CultureInfo.InvariantCulture, "VisualStudio.DTE.{0}.0", TargetVsVersion));
            this.Projects = dte2.Solution.OfType<Project>().ToList();
        }

        /// <summary>
        /// Gets or sets the projects.
        /// </summary>
        public List<Project> Projects { get; set; }
    }
}
