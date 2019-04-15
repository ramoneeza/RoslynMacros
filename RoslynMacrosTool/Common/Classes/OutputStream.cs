using System;
using System.IO;
using RoslynMacros.Common.Interfaces;

namespace RoslynMacros.Common.Classes
{
    public class OutputStream : IOutputStream, IDisposable
    {
        private FileStream FileStream { get; set; }
        private StreamWriter StreamWriter { get; set; }
        public FileInfo File { get; }
        public FileInfo Nest { get; set; }

        public void End()
        {
            StreamWriter?.Close();
            FileStream?.Close();
            StreamWriter?.Dispose();
            FileStream?.Dispose();
            StreamWriter = null;
            FileStream = null;
        }

        public OutputStream(FileInfo file, FileInfo nest = null)
        {
            File = file;
            Nest = nest;
            FileStream = new FileStream(File.FullName, FileMode.Create);
            StreamWriter = new StreamWriter(FileStream);
        }

        public void Write(string cad) => StreamWriter.Write(cad);
        public void WriteLine(string cad) => StreamWriter.WriteLine(cad);

        public void Dispose()
        {
            End();
        }
    }
}