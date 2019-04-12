using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RoslynMacros;
using Samples.Interfaces;

namespace Samples.Model
{
    [AutoImplement("AutoImplement.Record.csmacro",typeof(IUser)),
     MacroFlags("@CreateKeyConstructor")
    ]
    public abstract partial class AbsUser

    {

    }
}
