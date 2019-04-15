namespace RoslynMacros.Common.Interfaces
{
    public interface IOutputEngine
    {
        IProject Project { get; }
        bool Verbose { get; }
        void LogWrite(string line);
        void LogWriteError(string line);
        void ConsoleWrite(string line);
        void ErrorWrite(string line);
        void LogConsoleWrite(string line);
        void LogConsoleErrorWrite(string line);
    }
}