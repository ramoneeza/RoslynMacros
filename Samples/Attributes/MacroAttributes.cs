using System;
using System.Collections.Generic;
using System.Text;

namespace Samples.Attributes
{
    [AttributeUsage(AttributeTargets.Interface|AttributeTargets.Class, Inherited = true, AllowMultiple = false)]
    public sealed class TableAttribute : Attribute
    {
        public string Name { get;  }
        public TableAttribute(string name)
        {
            Name = name;
        }
    }

    [AttributeUsage(AttributeTargets.Property, Inherited = true, AllowMultiple = false)]
    public sealed class KeyAttribute : Attribute
    {
        public KeyAttribute(){}
    }
    [AttributeUsage(AttributeTargets.Property, Inherited = true, AllowMultiple = false)]
    public sealed class ExplicitKeyAttribute : Attribute
    {
        public ExplicitKeyAttribute(){}
    }
    [AttributeUsage(AttributeTargets.Property, Inherited = true, AllowMultiple = false)]
    public sealed class InitValue : Attribute
    {
        public object Value;
        public InitValue(object value){}
    }

}
