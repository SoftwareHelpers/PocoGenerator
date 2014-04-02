// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FieldDetails.cs" company="Company">
//   Copyrights 2014.
// </copyright>
// <summary>
//   The field details.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace PocoGenerator.Base.Models
{
    using System;

    /// <summary>
    /// The field details.
    /// </summary>
    public class FieldDetails
    {
        /// <summary>
        /// Gets or sets a value indicating whether is primary key.
        /// </summary>
        public bool IsPrimaryKey { get; set; }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the type name.
        /// </summary>
        public Type TypeName { get; set; }
    }
}
