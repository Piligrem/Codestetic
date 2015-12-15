using System;
using System.Collections.Generic;
using Codestetic.Web.Core.Domain.GPS;
using Codestetic.Web.Core.Domain.Users;

namespace Codestetic.Web.Services.Devices
{
    public interface IDeviceIndicatorService
    {
        IList<DeviceIndicator> GetAllDeviceIndicatorByUser(User user);
        DeviceIndicatorIcon GetDeviceIndicator(ObjectIndicator objectIndicator);
        //IList<DeviceIndicatorIcon> GetAllDevicesIndicatorByUser(User user);
        IList<DeviceIndicator> GetAllDeviceIndicatorByUserId(long userId);
        DeviceIndicator GetDeviceIndicatorByDeviceId(long deviceId);
    }
}
