using System;
using System.Diagnostics;

using Specter.Web.Core.Data;
using Specter.Web.Core.Domain.Logging;
using Specter.Web.Core.Domain.Common;
using Specter.Web.Core;

namespace Specter.Web.Services.Logging
{
    public partial class PerfLoggerService : IPerfLoggerService
    {
        #region Constants
        #endregion Constants

        #region Fields
        private readonly IRepository<Performance> performanceRepository;
        private readonly CommonSettings commonSettings;
        private string taskName;
        private Stopwatch stopwatch;
        private long userId;
        #endregion Fields

        #region Constructors
        public PerfLoggerService(IRepository<Performance> performanceRepository,
            CommonSettings commonSettings,
            IWorkContext workContext)
        {
            this.performanceRepository = performanceRepository;
            this.commonSettings = commonSettings;

            userId = workContext.CurrentUser.Id;
            taskName = GetTaskName();
            stopwatch = new Stopwatch();
            stopwatch.Start();
        }
        #endregion Constructors

        #region Properties
        #endregion Properties

        #region Methods
        public void Save()
        {
            stopwatch.Stop();
            performanceRepository.Insert(new Performance() 
            { 
                TaskName = taskName, 
                Runtime = new TimeSpan(stopwatch.ElapsedTicks),
                UserId = userId
            });
        }

        public string GetTaskName()
        {
            var frame = new StackFrame(1, false);
            return frame.GetMethod().DeclaringType.FullName;
        }
        #endregion Methods
    }
}
