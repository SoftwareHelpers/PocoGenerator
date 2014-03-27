﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PocoGenerator.Base.CodeGenerator
{
    using PocoGenerator.Base.Models;

    public interface ICodeGenerator
    {
        string CodeWriter(string @namespace, string nameofClass, IEnumerable<FieldDetails> fieldDetailses);
    }
}