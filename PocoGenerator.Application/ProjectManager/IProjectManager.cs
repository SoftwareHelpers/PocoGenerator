using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PocoGenerator.Base.ProjectManager
{
    using EnvDTE;

    /// <summary>
    /// The ProjectManager interface.
    /// </summary>
    public interface IProjectManager
    {
        /// <summary>
        /// Gets or sets the projects.
        /// </summary>
        List<Project> Projects { get; set; }
    }
}
