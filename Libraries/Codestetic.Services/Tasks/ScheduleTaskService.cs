using System;
using System.Collections.Generic;
using System.Linq;
using Codestetic.Web.Core.Data;
using Codestetic.Web.Core.Domain.Tasks;
using Codestetic.Web.Core.Caching;
using Codestetic.Web.Core.Logging;

namespace Codestetic.Web.Services.Tasks
{
    /// <summary>
    /// Task service
    /// </summary>
    public partial class ScheduleTaskService : IScheduleTaskService
    {
        #region Constants
        #endregion

        #region Fields
        private readonly IRepository<ScheduleTask> _taskRepository;
        #endregion

        #region Constructors
        public ScheduleTaskService(IRepository<ScheduleTask> taskRepository)
        {
            this._taskRepository = taskRepository;
        }
        #endregion Constructors

        #region Methods
        public virtual void DeleteTask(ScheduleTask task)
        {
            if (task == null)
                throw new ArgumentNullException("task");

            _taskRepository.Delete(task);
        }
        public virtual ScheduleTask GetTaskById(long taskId)
        {
            if (taskId == 0)
                return null;

            return _taskRepository.GetById(taskId);
        }
        public virtual ScheduleTask GetTaskByType(string type)
        {
            if (String.IsNullOrWhiteSpace(type))
                return null;

            var query = _taskRepository.Table;
            query = query.Where(st => st.Type == type);
            query = query.OrderByDescending(t => t.Id);

            var task = query.FirstOrDefault();
            return task;
        }
        public virtual IList<ScheduleTask> GetAllTasks(bool showHidden = false)
        {
            var query = _taskRepository.Table;

            if (!showHidden)
            {
                query = query.Where(t => t.Enabled);
            }
            query = query.OrderByDescending(t => t.Seconds);

            var tasks = query.ToList();
            return tasks;
        }
        public virtual void InsertTask(ScheduleTask task)
        {
            if (task == null)
                throw new ArgumentNullException("task");

            _taskRepository.Insert(task);
        }
        public virtual void UpdateTask(ScheduleTask task)
        {
            if (task == null)
                throw new ArgumentNullException("task");

            _taskRepository.Update(task);
        }
        #endregion
    }
}
