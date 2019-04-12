using System;
using RoslynMacros;
using Samples.Attributes;

namespace Samples.Interfaces
{
    [Table("Users")]
    [AutoInterface(typeof(IRecord))]
    public interface IUser : IRecord
    {
        [ExplicitKey]
        [InitValue("")]
        string Account { get; }
        [InitValue("")]
        string Name { get; set; }
        [InitValue("")]
        string Surname { get; set; }
        [InitValue("")]
        string Password { get; set; }
        bool Verified { get; set; }
        bool Admin { get; set; }
        bool Watcher { get; set; }
        [InitValue("")]
        string Email { get; set; }
    }
    
}