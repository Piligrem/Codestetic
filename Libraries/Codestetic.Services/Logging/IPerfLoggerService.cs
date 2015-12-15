namespace Specter.Web.Services.Logging
{
    public partial interface IPerfLoggerService
    {
        void Save();
        string GetTaskName();
    }
}
