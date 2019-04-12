// AutoInterface HelperIUser
using System;
using RoslynMacros;
using Samples.Attributes;

#pragma warning disable CS0105
using System;
#pragma warning restore CS0105
namespace Samples.Interfaces
{
    public static partial class HelperIUser
    {
		public static bool Equals(IUser x, IUser y)
        {
            if (ReferenceEquals(x, y)) return true;
            if ((x == null) || (y == null)) return false;
            return
                x.Account == y.Account &&
                x.Name == y.Name &&
                x.Surname == y.Surname &&
                x.Password == y.Password &&
                x.Verified == y.Verified &&
                x.Admin == y.Admin &&
                x.Watcher == y.Watcher &&
                x.Email == y.Email &&
                true;
        }
        public static int GetHashCode(this IUser obj)
        {
            return 
			((obj.Account as object)?.GetHashCode()??0)*17 ^
			((obj.Name as object)?.GetHashCode()??0)*17 ^
			((obj.Surname as object)?.GetHashCode()??0)*17 ^
			((obj.Password as object)?.GetHashCode()??0)*17 ^
			((obj.Verified as object)?.GetHashCode()??0)*17 ^
			((obj.Admin as object)?.GetHashCode()??0)*17 ^
			((obj.Watcher as object)?.GetHashCode()??0)*17 ^
			((obj.Email as object)?.GetHashCode()??0)*17 ^
            0;
        }
        public static void ShallowCopy(this IUser from, IUser to)
        {
            to.Name = from.Name;
            to.Surname = from.Surname;
            to.Password = from.Password;
            to.Verified = from.Verified;
            to.Admin = from.Admin;
            to.Watcher = from.Watcher;
            to.Email = from.Email;
        }
        
	}	
}
