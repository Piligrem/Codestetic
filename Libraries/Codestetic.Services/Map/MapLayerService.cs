using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Codestetic.Web.Core.Caching;
using Codestetic.Web.Core.Data;
using Codestetic.Web.Core.Domain.Common;
using Codestetic.Web.Core.Domain.Map;
using Codestetic.Web.Core.Events;
using Codestetic.Web.Services.Configuration;
using Codestetic.Web.Services.Logging;

namespace Codestetic.Web.Services.Map
{
    public class MapLayerService : IMapLayerService
    {
        #region Constants
        private const string GROUPMAPLAYER_ALL_KEY = "specter.groupmaplayer.all";
        private const string MAPLAYER_ALL_KEY = "specter.maplayer.all";
        private const string MAPLAYER_GROUP_KEY = "specter.maplayer.group-{0}";
        #endregion Constants

        #region Fields
        private readonly IRepository<GroupMapLayer> _groupMapLayerRepository;
        private readonly IRepository<MapLayer> _mapLayerRepository;
        private readonly ICacheManager _cacheManager;
        private readonly IUserActivityService _userActivityService;
        private readonly IEventPublisher _eventPublisher;
        private readonly CommonSettings _commonSettings;
        private readonly ISettingService _settingService;
        #endregion Fields

        #region Constructors
        public MapLayerService(IRepository<GroupMapLayer> groupMapLayerRepository,
            IRepository<MapLayer> mapLayerRepository, ICacheManager cacheManager,
            IEventPublisher eventPublisher, IUserActivityService userActivityService, 
            CommonSettings commonSettings, ISettingService settingService)
        {
            this._groupMapLayerRepository = groupMapLayerRepository;
            this._mapLayerRepository = mapLayerRepository;
            this._cacheManager = cacheManager;
            this._eventPublisher = eventPublisher;
            this._userActivityService = userActivityService;
            this._commonSettings = commonSettings;
            this._settingService = settingService;
        }
        #endregion Constructors

        #region Methods
        public IList<GroupMapLayer> GetGroupMapLayers()
        {
            return _cacheManager.Get(GROUPMAPLAYER_ALL_KEY, () =>
            {
                var query = _groupMapLayerRepository.Table;
                query = query.OrderBy(g => g.Order);
                return query.ToList();
            });
        }
        public IList<MapLayer> GetMapLayersByGroup(GroupMapLayer group)
        {
            var key = string.Format(MAPLAYER_GROUP_KEY, group.Name);
            return _cacheManager.Get(key, () =>
            {
                var query = _mapLayerRepository.Table;
                query = query.Where(l => l.GroupMapLayerId == group.Id).OrderBy(l => l.Order);
                return query.ToList();
            });
        }
        public IList<MapLayer> GetMapLayers()
        {
            return _cacheManager.Get(MAPLAYER_ALL_KEY, () =>
            {
                var query = _mapLayerRepository.Table;
                return query.ToList();
            });
        }
        #endregion Methods
    }
}
