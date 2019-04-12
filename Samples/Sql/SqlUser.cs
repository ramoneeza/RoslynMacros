using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RoslynMacros;
using Samples.Attributes;
using Samples.Interfaces;

namespace Samples.Sql
{
    [AutoImplement("AutoImplement.Record.csmacro",typeof(IUser)),
     MacroFlags("@KeySetter","@CreateKeyConstructor")
    ]
    [Table("Usuarios")]
    public partial class SqlUser : IUser
    {
       
        
    }
}
