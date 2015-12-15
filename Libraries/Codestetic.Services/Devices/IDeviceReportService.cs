namespace Codestetic.Web.Services.Devices
{
    public interface IDeviceReportService
    {
        int NowConnectedDevices();
        int TodayConnected();
        int TodayActiveDevices();
        int TodayDeactivatedDevices();
    }
}
