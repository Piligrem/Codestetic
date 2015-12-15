using System;
using System.Collections.Generic;
using Codestetic.Web.Core.Domain.Devices;
using Codestetic.Web.Core.Domain.Tracker;

namespace Codestetic.Web.Services.Tracker
{
    public interface ITrackService
    {
        IList<Track> GetTrackByDevice(Device device, DateTime? fromDate = null, DateTime? toDate = null);
        IList<Track> GetTrackByDeviceId(long deviceId, DateTime? fromDate = null, DateTime? toDate = null);
        Track GetTrackById(long id);
        DateTime GetMinDateTrack(long deviceId);
        DateTime GetMaxDateTrack(long deviceId);
    }
}
