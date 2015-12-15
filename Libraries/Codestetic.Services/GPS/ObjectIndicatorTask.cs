using System;
using System.Net;
using System.Linq;
using Codestetic.Web.Core;
using Codestetic.Web.Services.Tasks;
using Codestetic.Web.Core.Domain.Common;
using Codestetic.Web.Core.Data;
using Codestetic.Web.Core.Domain.GPS;
using Codestetic.Web.Services.SignalR;
using Codestetic.Web.Services.Helpers;
using Codestetic.Web.Services.Localization;
using System.Data.Objects;
using System.Data.Entity;
using System.Collections.Generic;
using Codestetic.Web.Services.Devices;

namespace Codestetic.Web.Services.GPS
{
    public partial class ObjectIndicatorTask : ITask
    {
        #region Fields
        private readonly IDeviceIndicatorService deviceIndicatorService;
        private readonly IRepository<ObjectIndicator> objectIndicatorRepository;
        #endregion Fields

        #region Convert function
        #endregion Convert function

        #region Constructors
        public ObjectIndicatorTask(
            IDeviceIndicatorService deviceIndicatorService,
            IRepository<ObjectIndicator> objectIndicatorRepository)
        {
            this.deviceIndicatorService = deviceIndicatorService;
            this.objectIndicatorRepository = objectIndicatorRepository;
        }
        #endregion Constructors

        public void Execute()
        {
            var nowDate = DateTime.UtcNow.Date;
            var objectsIndicator = objectIndicatorRepository.Table
                .Where(q => DbFunctions.TruncateTime(q.SessionDateUtc).Value == nowDate).ToList()
                .GroupBy(q => q.UserId)
                .Select(q => new { UserId = q.Key, ObjectsIndicator = q.OrderBy(oq => oq.Id).ToList<ObjectIndicator>() }).ToList();

            foreach (var objectIndicator in objectsIndicator)
            {
                var devicesIndicator = new List<DeviceIndicatorIcon>();
                objectIndicator.ObjectsIndicator.ForEach(x => devicesIndicator.Add(deviceIndicatorService.GetDeviceIndicator(x)));
                NotificationHub.SendIndicatorToUser(objectIndicator.UserId, devicesIndicator);
            }
        }

        #region Private methods
        #endregion Private methods
    }
}
