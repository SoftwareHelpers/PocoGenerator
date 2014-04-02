// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ResultCode.cs" company="Company">
//   Copyrights 2014
// </copyright>
// <summary>
//   The result code.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace PocoGenerator.Base.Common
{
    /// <summary>
    /// The result code.
    /// </summary>
    public class ResultCode
    {
        /// <summary>
        /// The result code successfully generated.
        /// </summary>
        public const int ResultCode_SuccessfullyGenerated = 0;

        /// <summary>
        /// The result code successfully generated_ message.
        /// </summary>
        public const string ResultCode_SuccessfullyGenerated_Message = "Successfully generated classes.";

        /// <summary>
        /// The result code project not selected.
        /// </summary>
        public const int ResultCode_ProjectNotSelected = 1;

        /// <summary>
        /// The result code project not selected_ message.
        /// </summary>
        public const string ResultCode_ProjectNotSelected_Message = "Project not selected.";

        /// <summary>
        /// The result code unknown error.
        /// </summary>
        public const int ResultCode_UnknownError = 2;

        /// <summary>
        /// The result code unknown error_ message.
        /// </summary>
        public const string ResultCode_UnknownError_Message = "Unknown error.";
    }
}
