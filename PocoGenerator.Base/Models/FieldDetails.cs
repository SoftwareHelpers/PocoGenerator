using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PocoGenerator.Base.Models
{
    public class FieldDetails
    {
        public bool IsPrimaryKey { get; set; }
        public string Name { get; set; }
        public Type TypeName { get; set; }
    }
}
