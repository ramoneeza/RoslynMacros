using System;
using System.Collections.Generic;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using LightInject;
using RoslynMacros.Common.Classes;

namespace RoslynMacros
{
    public class Command:BaseCommand
    {
        public Command(IServiceFactory serviceFactory):base(serviceFactory)
        {
            
        }
    }
}
