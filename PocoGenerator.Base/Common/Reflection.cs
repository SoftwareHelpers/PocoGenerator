// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Reflection.cs" company="Company">
//   Copyrights 2014.
// </copyright>
// <summary>
//   The reflection.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace PocoGenerator.Base.Common
{
    /// <summary>
    /// The reflection.
    /// </summary>
    public class Reflection
    {
        /// <summary>
        /// The fill object with property.
        /// </summary>
        /// <param name="objectTo"> The object to. </param>
        /// <param name="propertyName"> The property name. </param>
        /// <param name="propertyValue"> The property value. </param>
        public void FillObjectWithProperty(ref object objectTo, string propertyName, object propertyValue)
        {
            var returnObject = objectTo.GetType();
            returnObject.GetProperty(propertyName).SetValue(objectTo, propertyValue);
        }

        /// <summary>
        /// The fill object with property.
        /// </summary>
        /// <param name="objectTo"> The object to. </param>
        /// <param name="propertyName"> The property name. </param>
        /// <param name="propertyType"> The property type. </param>
        public void FillObjectWithPropertyEx(ref object objectTo, string propertyName, object propertyType)
        {
            ////var returnObject = objectTo.GetType();
            ////var returnObject = typeof(FieldDetails);
            ////var fieldDetail = new FieldDetails { Name = propertyName, TypeName = propertyType.ToString() };
            ////returnObject.GetProperty("Name").SetValue(objectTo, propertyName);
            ////returnObject.GetProperty("TypeName").SetValue(objectTo, propertyType);
        }
    }
}
