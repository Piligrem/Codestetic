using System;
using System.Collections.Generic;
using System.Linq;
using Codestetic.Web.Core.Data;
using Codestetic.Web.Core.Domain.Tasks;
using Codestetic.Web.Core.Domain.Users;
using Codestetic.Web.Core.Domain.GPS;
using Codestetic.Web.Core.Domain.Notices;
using Codestetic.Web.Core.Domain.Devices;

namespace Codestetic.Web.Services.Tasks
{
    /// <summary>
    /// Task service
    /// </summary>
    public partial class NotifyService : INotifyService
    {
        #region Fields
        private readonly IRepository<ScheduleNotifyTask> _taskNotifyRepository;
        private readonly IRepository<Notify> _notifyRepository;
        private readonly IRepository<User> _userRepository;
        private readonly IRepository<Device> _deviceRepository;
        #endregion Fields

        #region Constructors

        public NotifyService(IRepository<ScheduleNotifyTask> taskNotifyRepository, 
            IRepository<Notify> notifyUserRepository,
            IRepository<User> userRepository)
        {
            this._taskNotifyRepository = taskNotifyRepository;
            this._notifyRepository = notifyUserRepository;
            this._userRepository = userRepository;
        }
        #endregion Constructors

        #region Methods
        /// <summary>
        /// Deletes a notify user
        /// </summary>
        /// <param name="notify">Notify User</param>
        public virtual void DeleteNotify(Notify notify)
        {
            if (notify == null)
                throw new ArgumentNullException("notify");

            _notifyRepository.Delete(notify);
        }

        /// <summary>
        /// Gets a notify user
        /// </summary>
        /// <param name="notifyUserId">Notify user identifier</param>
        /// <returns>Notify user</returns>
        public virtual Notify GetNotifyById(long notifyId)
        {
            if (notifyId == 0)
                return null;

            return _notifyRepository.GetById(notifyId);
        }

        /// <summary>
        /// Gets a notify by its user
        /// </summary>
        /// <param name="userId">User identifier</param>
        /// <returns>Notify collection</returns>
        public virtual IList<Notify> GetNotifiesByDevice(Device device)
        {
            if (device == null)
                return null;

            var query = device.Notifies;
            return query.ToList();
        }

        public virtual IList<Notify> GetNotifiesByDeviceId(long deviceId)
        {
            if (deviceId == 0)
                return null;

            var device = _deviceRepository.GetById(deviceId);
            return GetNotifiesByDevice(device);
        }

        /// <summary>
        /// Gets all notify
        /// </summary>
        /// <param name="showHidden">A value indicating whether to show hidden records</param>
        /// <returns>Tasks</returns>
        public virtual IList<ScheduleNotifyTask> GetAllNotifyTasks(bool showHidden = false)
        {
            var query = _taskNotifyRepository.Table;

            if (!showHidden)
            {
                query = query.Where(nu => nu.Enabled);
            }
            return query.ToList();
        }

        /// <summary>
        /// Inserts a notify
        /// </summary>
        /// <param name="task">Notify</param>
        public virtual void InsertNotify(Notify notify)
        {
            if (notify == null)
                throw new ArgumentNullException("notify");

            _notifyRepository.Insert(notify);
        }

        /// <summary>
        /// Updates the notify user
        /// </summary>
        /// <param name="task">Notify</param>
        public virtual void UpdateNotify(Notify notify)
        {
            if (notify == null)
                throw new ArgumentNullException("notify");

            _notifyRepository.Update(notify);
        }
        #endregion
    }
}
