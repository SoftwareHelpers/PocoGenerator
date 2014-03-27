using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PocoGenerator.Base.Common
{
    /// <summary>
    /// The data object field attribute.
    /// </summary>
    [AttributeUsageAttribute(AttributeTargets.Property)]
    public sealed class DataObjectFieldAttribute : Attribute
    {
        /// <summary>
        /// The primary key.
        /// </summary>
        private readonly bool primaryKey;

        /// <summary>
        /// The is identity.
        /// </summary>
        private readonly bool isIdentity;

        /// <summary>
        /// The is nullable.
        /// </summary>
        private readonly bool isNullable;

        /// <summary>
        /// The length.
        /// </summary>
        private readonly int length = -1;

        /// <summary>
        /// Initializes a new instance of the <see cref="DataObjectFieldAttribute"/> class.
        /// </summary>
        /// <param name="primaryKey">
        /// The primary key.
        /// </param>
        public DataObjectFieldAttribute(bool primaryKey)
        {
            this.primaryKey = primaryKey;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DataObjectFieldAttribute"/> class.
        /// </summary>
        /// <param name="primaryKey">
        /// The primary key.
        /// </param>
        /// <param name="isIdentity">
        /// The is identity.
        /// </param>
        public DataObjectFieldAttribute(bool primaryKey, bool isIdentity)
        {
            this.primaryKey = primaryKey;
            this.isIdentity = isIdentity;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DataObjectFieldAttribute"/> class.
        /// </summary>
        /// <param name="primaryKey"> The primary key. </param>
        /// <param name="isIdentity"> The is identity. </param>
        /// <param name="isNullable"> The is nullable. </param>
        public DataObjectFieldAttribute(bool primaryKey, bool isIdentity, bool isNullable)
        {
            this.primaryKey = primaryKey;
            this.isIdentity = isIdentity;
            this.isNullable = isNullable;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DataObjectFieldAttribute"/> class.
        /// </summary>
        /// <param name="primaryKey"> The primary key. </param>
        /// <param name="isIdentity"> The is identity. </param>
        /// <param name="isNullable"> The is nullable. </param>
        /// <param name="length"> The length. </param>
        public DataObjectFieldAttribute(bool primaryKey, bool isIdentity, bool isNullable, int length)
        {
            this.primaryKey = primaryKey;
            this.isIdentity = isIdentity;
            this.isNullable = isNullable;
            this.length = length;
        }

        /// <summary>
        /// Gets a value indicating whether is identity.
        /// </summary>
        public bool IsIdentity
        {
            get { return this.isIdentity; }
        }

        /// <summary>
        /// Gets a value indicating whether is nullable.
        /// </summary>
        public bool IsNullable
        {
            get { return this.isNullable; }
        }

        /// <summary>
        /// Gets the length.
        /// </summary>
        public int Length
        {
            get { return this.length; }
        }

        /// <summary>
        /// Gets a value indicating whether primary key.
        /// </summary>
        public bool PrimaryKey
        {
            get { return this.primaryKey; }
        }

        /// <summary>
        /// The equals.
        /// </summary>
        /// <param name="obj"> The object. </param>
        /// <returns> The <see cref="bool"/>. </returns>
        public override bool Equals(object obj)
        {
            var other = obj as DataObjectFieldAttribute;
            if (other == null)
            {
                return false;
            }

            return other.primaryKey == this.primaryKey &&
                other.isIdentity == this.isIdentity &&
                other.isNullable == this.isNullable &&
                other.length == this.length;
        }

        /// <summary>
        /// The get hash code.
        /// </summary>
        /// <returns> The <see cref="int"/>. </returns>
        public override int GetHashCode()
        {
            return ((this.primaryKey ? 1 : 0) |
                (this.isIdentity ? 2 : 0) |
                (this.isNullable ? 4 : 0)) ^
                this.length;
        }
    }
}
