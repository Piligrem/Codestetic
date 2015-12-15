using System;
using System.Net;
using System.Linq;
using System.Data.Entity;
using Codestetic.Web.Core;
using Codestetic.Web.Services.Tasks;
using Codestetic.Web.Core.Domain.Common;
using Codestetic.Web.Core.Data;
using Codestetic.Web.Core.Domain.GPS;
using Codestetic.Web.Services.SignalR;
using System.Collections.Generic;

namespace Codestetic.Web.Services.GPS
{
    public partial class ObjectPositionTask : ITask
    {
        #region Fields
        private readonly IRepository<ObjectPosition> objectPositionRepository;
        #endregion Fields

        #region Constructors
        public ObjectPositionTask(IRepository<ObjectPosition> objectPositionRepository)
        {
            this.objectPositionRepository = objectPositionRepository;
        }
        #endregion Constructors

        public void Execute()
        {
            var nowDate = DateTime.UtcNow.Date;
            var objectsPosition = objectPositionRepository.Table
                .Where(q => DbFunctions.TruncateTime(q.SessionDateUtc).Value == nowDate)
                .GroupBy(q => q.UserId)
                .Select(q => new { UserId = q.Key, ObjectPosition = q.OrderBy(oq => oq.TimestampOnUtc).ToList() }).ToList();
            foreach (var objectPosition in objectsPosition)
            {
                var devicesPosition = new List<ObjectPosition>();
                objectPosition.ObjectPosition.ForEach(x => devicesPosition.Add(x));

                NotificationHub.SendPositionToUser(objectPosition.UserId, devicesPosition);
            }
        }
    }
}
