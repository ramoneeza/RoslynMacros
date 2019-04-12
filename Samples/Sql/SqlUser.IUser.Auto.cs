// Autoimplementing class SqlUser
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RoslynMacros;
using Samples.Attributes;
using Samples.Interfaces;

#pragma warning disable CS0105
using System;
#pragma warning restore CS0105
#pragma warning disable CS0105
using Samples.Attributes;
#pragma warning restore CS0105
namespace Samples.Sql
{
    public partial class SqlUser : IUser
    {
		[ExplicitKey]
		public  string Account { get; set;}="";
		public virtual string Name { get; set;}="";
		public virtual string Surname { get; set;}="";
		public virtual string Password { get; set;}="";
		public virtual bool Verified { get; set;}
		public virtual bool Admin { get; set;}
		public virtual bool Watcher { get; set;}
		public virtual string Email { get; set;}="";
        public SqlUser(){}
		public SqlUser(string key):this(){
			Account=key;
		}
        public SqlUser(IUser other):this(){
			other.ShallowCopy(this);
			Account=other.Account;
        }
        public virtual SqlUser Clone()=>new SqlUser(this);
		object ICloneable.Clone()=>Clone();
        public override bool Equals(object obj)
        {
            if (obj is IUser other)
                return HelperIUser.Equals(this,other);
            else
                return false;
        }
        public override int GetHashCode()=>HelperIUser.GetHashCode(this);
	   public static string Table()=>"Users";
    }
}
