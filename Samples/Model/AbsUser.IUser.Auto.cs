// Autoimplementing class AbsUser
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RoslynMacros;
using Samples.Interfaces;

#pragma warning disable CS0105
using System;
#pragma warning restore CS0105
#pragma warning disable CS0105
using Samples.Attributes;
#pragma warning restore CS0105
namespace Samples.Model
{
    public partial class AbsUser : IUser
    {
		[ExplicitKey]
		public  string Account { get; private set;}="";
		public virtual string Name { get; set;}="";
		public virtual string Surname { get; set;}="";
		public virtual string Password { get; set;}="";
		public virtual bool Verified { get; set;}
		public virtual bool Admin { get; set;}
		public virtual bool Watcher { get; set;}
		public virtual string Email { get; set;}="";
        protected AbsUser(){}
		protected AbsUser(string key):this(){
			Account=key;
		}
        protected AbsUser(IUser other):this(){
			other.ShallowCopy(this);
			Account=other.Account;
        }
		protected abstract IUser IntClone();
		object ICloneable.Clone()=>IntClone();
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
