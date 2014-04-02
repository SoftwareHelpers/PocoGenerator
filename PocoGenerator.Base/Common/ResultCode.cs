using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PocoGenerator.Base.Common
{
    public class ResultCode
    {
        public const int ResultCode_SuccessfullyGenerated = 0;
        public const string ResultCode_SuccessfullyGenerated_Message = "Successfully generated classes.";

        public const int ResultCode_ProjectNotSelected = 1;
        public const string ResultCode_ProjectNotSelected_Message = "Project not selected.";

        public const int ResultCode_UnknownError = 2;
        public const string ResultCode_UnknownError_Message = "Unknown error.";
    }
}
