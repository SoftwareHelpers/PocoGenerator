// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ICodeGenerator.cs" company="Company">
//   Copyrights 2014.
// </copyright>
// <summary>
//   The CodeGenerator interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace PocoGenerator.Base.CodeGenerator
{
    using System.Collections.Generic;

    using EnvDTE;

    using PocoGenerator.Base.Models;

    /// <summary>
    /// The CodeGenerator interface.
    /// </summary>
    public interface ICodeGenerator
    {
        /// <summary>
        /// The code writer.
        /// </summary>
        /// <param name="namespace"> The namespace. </param>
        /// <param name="nameofClass"> The nameof class. </param>
        /// <param name="fieldDetailses"> The field detailses. </param>
        /// <returns> The <see cref="string"/>. </returns>
        string CodeWriter(string @namespace, string nameofClass, IEnumerable<FieldDetails> fieldDetailses);

        /// <summary>
        /// The code writer.
        /// </summary>
        /// <param name="project"> The project. </param>
        /// <param name="nameofClass"> The nameof class. </param>
        /// <param name="fieldDetailses"> The field detailses. </param>
        /// <param name="directoryName"> The directory name. </param>
        /// <returns> The <see cref="int"/>. </returns>
        int CodeWriter(Project project, string nameofClass, IEnumerable<FieldDetails> fieldDetailses, string directoryName);
    }
}
