using System;
using System.Collections.Generic;
using System.Linq;
using Codestetic.Web.Core.Caching;
using Codestetic.Web.Core.Data;
using Codestetic.Web.Core.Domain.Directory;
using Codestetic.Web.Core.Events;

namespace Codestetic.Web.Services.Directory
{
    /// <summary>
    /// Country service
    /// </summary>
    public partial class PanelService : IPanelService
    {
        #region Constants
        private const string PANELS_ALL_KEY = "specter.panel.all-{0}";
        private const string PANELS_PATTERN_KEY = "specter.panel.";
        #endregion
        
        #region Fields
        private readonly IRepository<Panel> _panelRepository;
        private readonly IEventPublisher _eventPublisher;
        private readonly ICacheManager _cacheManager;
        #endregion Fields

        #region Constructors
        public PanelService(ICacheManager cacheManager,
            IRepository<Panel> panelRepository,
            IEventPublisher eventPublisher)
        {
            _cacheManager = cacheManager;
            _panelRepository = panelRepository;
            _eventPublisher = eventPublisher;
        }
        #endregion Constructors

        #region Methods
        public virtual IList<Panel> GetPanel(string name, bool showHidden = false)
        {
            string key = string.Format(PANELS_ALL_KEY, showHidden);
            return _cacheManager.Get(key, () =>
            {
                var query = from c in _panelRepository.Table
                            orderby c.DisplayOrder, c.Name
                            where showHidden
                            select c;
                var panel = query.ToList();
                return panel;
            });
        }
        #endregion Methods
    }
}