using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RoslynMacros;
using Samples.Interfaces;

namespace Samples.Model
{
    [AutoImplement("AutoImplement.INotifyPropertyChanged.csmacro",typeof(IUser)),
     MacroFlags()
    ]
    public partial class User:AbsUser
    {
        protected override IUser IntClone()=>new User(this);

        public User(string cuenta):base(cuenta)
        {
        }
        public User(IUser other) : base(other)
        {
        }
    }
}
