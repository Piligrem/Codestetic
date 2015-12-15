using System;
using System.Linq;
using System.Collections.Generic;

using Codestetic.Web.Core.Data;
using Codestetic.Web.Core.Domain.GPS;
using Codestetic.Web.Core.Domain.Users;
using Codestetic.Web.Services.Configuration;

namespace Codestetic.Web.Services.Devices
{
    public partial class DevicePositionService : IDevicePositionService
    {
        #region Constants
        //private const string USERROLES_ALL_KEY = "Specter.userrole.all-{0}";
        //private const string USERROLES_PATTERN_KEY = "Specter.userrole.";
        #endregion Constants

        #region Fields
        private readonly IRepository<User> _userRepository;
        private readonly IRepository<UserRole> _roleRepository;
        private readonly IRepository<DevicePosition> _devicePositionRepository;
        //private readonly IRepository<Session> _userSessionRepository;
        //private readonly ICacheManager _cacheManager;
        //private readonly IEventPublisher _eventPublisher;
        private readonly ISettingService _settingService;
        #endregion Fields

        #region Constructors
        public DevicePositionService(IRepository<DevicePosition> devicePositionRepository,
            //ICacheManager cacheManager,
            //IRepository<Session> userSessionRepository,
            ISettingService settingService,
            //IEventPublisher eventPublisher,
            IRepository<User> userRepository,
            IRepository<UserRole> roleRepository)
        {
            //this._cacheManager = cacheManager;
            this._devicePositionRepository = devicePositionRepository;
            this._userRepository = userRepository;
            this._roleRepository = roleRepository;
            //this._userSessionRepository = userSessionRepository;
            this._settingService = settingService;
            //this._eventPublisher = eventPublisher;
        }
        #endregion Constructors

        #region Methods
        public IList<DevicePosition> GetAllDeviceLastPositionByUser(User user)
        {
            if (user == null)
                return null;

            var query = from device in user.Devices
                        select device;

            return query.Select(d => d.DevicePosition).ToList();
        }
        public IList<DevicePosition> GetAllDeviceLastPositionByUserId(long userId)
        {
            if (userId == 0)
                return null;

            var user = _userRepository.GetById(userId);
            var query = from device in user.Devices
                        select device;

            return query.Select(d => d.DevicePosition).ToList();
        }
        public DevicePosition GetDeviceLastPositionByDeviceId(long userId, long deviceId)
        {
            if (deviceId == 0 || userId == 0)
                return null;

            var users = _userRepository.Table;
            var query = from user in users
                        where user.Id == userId
                        from device in user.Devices
                        where device.Id == deviceId
                        select device;

            //if (_settingService.GetSettingByKey<bool>("Visualize.OnlyValid"))
            //    query = query.Where(p => p.Valid);

            return query.FirstOrDefault().DevicePosition;
        }

        public DevicePosition GetPositionByDeviceId(long deviceId)
        {
            if (deviceId == 0)
                return null;

            var query = _devicePositionRepository.Table;
            query = query.Where(d => d.Id == deviceId);

            return query.FirstOrDefault();
        }
        #endregion Methods
    }
}
