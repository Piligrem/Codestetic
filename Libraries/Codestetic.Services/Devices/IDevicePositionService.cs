using System;
using System.Collections.Generic;
using Codestetic.Web.Core.Domain.GPS;
using Codestetic.Web.Core.Domain.Users;

namespace Codestetic.Web.Services.Devices
{
    public interface IDevicePositionService
    {
        IList<DevicePosition> GetAllDeviceLastPositionByUser(User user);
        IList<DevicePosition> GetAllDeviceLastPositionByUserId(long userId);
        DevicePosition GetDeviceLastPositionByDeviceId(long userId, long deviceId);

        DevicePosition GetPositionByDeviceId(long deviceId);
    }
}
