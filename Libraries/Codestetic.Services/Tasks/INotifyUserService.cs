using Codestetic.Web.Core.Domain.Devices;
using Codestetic.Web.Core.Domain.Notices;
using Codestetic.Web.Core.Domain.Tasks;
using System.Collections.Generic;

namespace Codestetic.Web.Services.Tasks
{
    public interface INotifyService
    {
        void DeleteNotify(Notify notify);
        Notify GetNotifyById(long notifyId);
        IList<Notify> GetNotifiesByDevice(Device device);
        IList<Notify> GetNotifiesByDeviceId(long deviceId);
        IList<ScheduleNotifyTask> GetAllNotifyTasks(bool showHidden = false);
        void InsertNotify(Notify notify);
        void UpdateNotify(Notify notify);
    }
}
