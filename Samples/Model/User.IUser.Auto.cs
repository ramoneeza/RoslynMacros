// Autoimplementing class User
using System;
using System.Collections.Generic;
using System.ComponentModel;
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
    public partial class User : INotifyPropertyChanged
    {
	public event PropertyChangedEventHandler PropertyChanged;
	protected virtual void OnPropertyChanged(params string[] properties){
		foreach(var p in properties)
			PropertyChanged?.Invoke(this,new PropertyChangedEventArgs(p));
	}
	public override string Name{
		get=>base.Name;
		set{
			if (Name.Equals(value)) return;
			base.Name=value;
			OnPropertyChanged(nameof(Name));
		}
	}
	public override string Surname{
		get=>base.Surname;
		set{
			if (Surname.Equals(value)) return;
			base.Surname=value;
			OnPropertyChanged(nameof(Surname));
		}
	}
	public override string Password{
		get=>base.Password;
		set{
			if (Password.Equals(value)) return;
			base.Password=value;
			OnPropertyChanged(nameof(Password));
		}
	}
	public override bool Verified{
		get=>base.Verified;
		set{
			if (Verified.Equals(value)) return;
			base.Verified=value;
			OnPropertyChanged(nameof(Verified));
		}
	}
	public override bool Admin{
		get=>base.Admin;
		set{
			if (Admin.Equals(value)) return;
			base.Admin=value;
			OnPropertyChanged(nameof(Admin));
		}
	}
	public override bool Watcher{
		get=>base.Watcher;
		set{
			if (Watcher.Equals(value)) return;
			base.Watcher=value;
			OnPropertyChanged(nameof(Watcher));
		}
	}
	public override string Email{
		get=>base.Email;
		set{
			if (Email.Equals(value)) return;
			base.Email=value;
			OnPropertyChanged(nameof(Email));
		}
	}
    }
}
