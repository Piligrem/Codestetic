using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.Entity.Spatial;
using System.Text;
using System.Web.Script.Serialization;
using System.Collections.Concurrent;

using Codestetic.Web.Services.SignalR;
using Codestetic.Web.Core;
using Codestetic.Web.Core.Data;
using System.Threading.Tasks;
using Codestetic.Web.Services.GPS;
using Codestetic.Web.Services.Logging;
using Codestetic.Web.Core.Logging;
using Codestetic.Web.Core.Domain.Logging;
using System.IO;
using Newtonsoft.Json;
using System.Diagnostics;
using Codestetic.Web.Services.Devices;

namespace Codestetic.Web.Controllers
{
    public class DataReceiverController : BaseController
    {
        #region Fields
        private readonly ILogger logger;
        private readonly IWorkContext workContext;
        private readonly IUserActivityService userActivityService;
        private readonly IDeviceService deviceService;
        #endregion Fields

        #region Constructors
        public DataReceiverController(
            ILogger logger, IWorkContext workContext,
            IDeviceService deviceService,
            IUserActivityService userActivityService)
        {
            this.logger = logger;
            this.workContext = workContext;
            this.userActivityService = userActivityService;
            this.deviceService = deviceService;
        }
        #endregion Constructors

        #region Methods
        
        [HttpPost]
        public JsonResult NowLocation()
        {
            Stream req = Request.InputStream;
            req.Seek(0, System.IO.SeekOrigin.Begin);
            string json = new StreamReader(req).ReadToEnd();
            dynamic data = JsonConvert.DeserializeObject(json);
            //Debug.WriteLine("IMEI: {0}, DateOnUtc: {1}, Longitude: {2}, Latitude: {3}", (string)data.IMEI, (DateTime)data.DateOnUtc, (double)data.Longitude, (double)data.Latitude);

            return Json(deviceService.SaveLocation(
                (string)data.IMEI, 
                (DateTime)data.DateOnUtc,
                (double)data.Latitude,
                (double)data.Longitude,
                (double)data.Altitude,
                (double)data.Angle, 
                (double)data.Speed, 
                (bool)data.Alarm));
        }
        #endregion Methods
    }
}
