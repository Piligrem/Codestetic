namespace Specter.Web.Core.Domain.Logging
{
    /// <summary>
    /// Represents a log level
    /// </summary>
    public enum LogLevel : long
    {
        Debug = 10,
        Information = 20,
        Warning = 30,
        Error = 40,
        Fatal = 50
    }
}
