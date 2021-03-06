using System;
using System.Collections.Generic;
using System.Linq;
using Codestetic.Web.Core.Domain.Cms;
using Codestetic.Web.Core.Plugins;
using Codestetic.Web.Services.Configuration;

namespace Codestetic.Web.Services.Cms
{
    /// <summary>
    /// Widget service
    /// </summary>
    public partial class WidgetService : IWidgetService
    {

        #region Fields

        private readonly IPluginFinder _pluginFinder;
        private readonly WidgetSettings _widgetSettings;
        private readonly ISettingService _settingService;

        #endregion
        
        #region Ctor

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="pluginFinder">Plugin finder</param>
        /// <param name="widgetSettings">Widget settings</param>
		/// <param name="pluginService">Plugin service</param>
		public WidgetService(IPluginFinder pluginFinder, WidgetSettings widgetSettings, ISettingService settingService)
        {
            this._pluginFinder = pluginFinder;
            this._widgetSettings = widgetSettings;
			this._settingService = settingService;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Load active widgets
        /// </summary>
		/// <param name="storeId">Load records allows only in specified store; pass 0 to load all records</param>
        /// <returns>Widgets</returns>
		public virtual IList<IWidgetPlugin> LoadActiveWidgets()
        {
            return LoadAllWidgets()
                   .Where(x => _widgetSettings.ActiveWidgetSystemNames.Contains(x.PluginDescriptor.SystemName, StringComparer.InvariantCultureIgnoreCase))
                   .ToList();
        }

        /// <summary>
        /// Load active widgets
        /// </summary>
        /// <param name="widgetZone">Widget zone</param>
		/// <param name="storeId">Load records allows only in specified store; pass 0 to load all records</param>
        /// <returns>Widgets</returns>
		public virtual IList<IWidgetPlugin> LoadActiveWidgetsByWidgetZone(string widgetZone)
        {
            if (String.IsNullOrWhiteSpace(widgetZone))
                return new List<IWidgetPlugin>();

            return LoadActiveWidgets()
                   .Where(x => x.GetWidgetZones().Contains(widgetZone, StringComparer.InvariantCultureIgnoreCase))
                   .ToList();
        }

        /// <summary>
        /// Load widget by system name
        /// </summary>
         /// <param name="systemName">System name</param>
        /// <returns>Found widget</returns>
        public virtual IWidgetPlugin LoadWidgetBySystemName(string systemName)
        {
            var descriptor = _pluginFinder.GetPluginDescriptorBySystemName<IWidgetPlugin>(systemName);
            if (descriptor != null)
                return descriptor.Instance<IWidgetPlugin>();

            return null;
        }

        /// <summary>
        /// Load all widgets
        /// </summary>
		/// <param name="storeId">Load records allows only in specified store; pass 0 to load all records</param>
        /// <returns>Widgets</returns>
		public virtual IList<IWidgetPlugin> LoadAllWidgets()
        {
			return _pluginFinder
				.GetPlugins<IWidgetPlugin>()
				.ToList();
        }
        #endregion
    }
}
