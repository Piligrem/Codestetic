using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.Entity.Spatial;
using System.Text;
using System.Web.Script.Serialization;
using System.Collections.Concurrent;
using System.IO;
using Newtonsoft.Json;
using Codestetic.Web.Core.Domain.GPS;
using Codestetic.Web.Services.SignalR;
using Codestetic.Web.Core.Data;
using Codestetic.Web.Services.GPS;
using Codestetic.Web.Services.Devices;

namespace Codestetic.Web.Controllers
{
    public class TestInfoController : BaseController
    {
        private readonly SqlDependencyService<SignalR_> objRepo = new SqlDependencyService<SignalR_>();
        private readonly IRepository<SignalR_> signalRRepository;
        private readonly IDevicePositionService devicePositionService;

        public TestInfoController(
            IRepository<SignalR_> signalRRepository,
            IDevicePositionService devicePositionService)
        {
            this.signalRRepository = signalRRepository;
            this.devicePositionService = devicePositionService;
            
        }

        // GET: TestInfo
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public JsonResult GetDate()
        {
            return Json(new
            {
                data = objRepo.GetData(),
                success = true
            });
        }
    }
}