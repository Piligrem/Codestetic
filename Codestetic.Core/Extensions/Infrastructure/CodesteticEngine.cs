using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using Autofac;
using Codestetic.Core.Configuration;
using Codestetic.Core.Data;
using Codestetic.Core.Infrastructure.DependencyManagement;
using Codestetic.Core.Plugins;

namespace Codestetic.Core.Infrastructure
{
    public class CodesteticEngine : IEngine
    {
        #region Fields

        private ContainerManager _containerManager;

        #endregion

        #region Ctor

        /// <summary>
		/// Creates an instance of the content engine using default settings and configuration.
		/// </summary>
		public CodesteticEngine() 
            : this(EventBroker.Instance, new ContainerConfigurer())
		{
		}

		public CodesteticEngine(EventBroker broker, ContainerConfigurer configurer)
		{
            var config = ConfigurationManager.GetSection("CodesteticConfig") as CodesteticConfig;
            InitializeContainer(configurer, broker, config);
		}
        
        #endregion

        #region Utilities

        private void RunStartupTasks()
        {
            var typeFinder = _containerManager.Resolve<ITypeFinder>();
            var startUpTaskTypes = typeFinder.FindClassesOfType<IStartupTask>();
            var startUpTasks = new List<IStartupTask>();

            foreach (var startUpTaskType in startUpTaskTypes)
            {
                if (PluginManager.IsActivePluginAssembly(startUpTaskType.Assembly))
                {
                    startUpTasks.Add((IStartupTask)Activator.CreateInstance(startUpTaskType));
                }
            }
            
            //sort
            foreach (var startUpTask in startUpTasks.OrderBy(st => st.Order))
            {
                startUpTask.Execute();
            }
        }

        private void InitializeContainer(ContainerConfigurer configurer, EventBroker broker, CodesteticConfig config)
        {
            var builder = new ContainerBuilder();

            _containerManager = new ContainerManager(builder.Build());
            configurer.Configure(this, _containerManager, broker, config);
        }

        #endregion

        #region Methods
        
        /// <summary>
        /// Initialize components and plugins in the sm environment.
        /// </summary>
        /// <param name="config">Config</param>
        public void Initialize(CodesteticConfig config)
        {
            bool databaseInstalled = DataSettingsHelper.DatabaseIsInstalled();
            if (databaseInstalled)
            {
                //startup tasks
                RunStartupTasks();
            }
        }

        public T Resolve<T>(string name = null) where T : class
		{
            if (name.HasValue())
            {
                return ContainerManager.ResolveNamed<T>(name);
            }
            return ContainerManager.Resolve<T>();
		}

        public object Resolve(Type type, string name = null)
        {
            if (name.HasValue())
            {
                return ContainerManager.ResolveNamed(name, type);
            }
            return ContainerManager.Resolve(type);
        }

        public Array ResolveAll(Type serviceType)
        {
            throw new NotImplementedException();
        }

        public T[] ResolveAll<T>()
        {
            return ContainerManager.ResolveAll<T>();
        }

		#endregion

        #region Properties

        public IContainer Container
        {
            get { return _containerManager.Container; }
        }

        public ContainerManager ContainerManager
        {
            get { return _containerManager; }
        }

        #endregion
    }
}
