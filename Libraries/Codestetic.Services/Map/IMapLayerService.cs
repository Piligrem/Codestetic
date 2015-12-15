using System;
using System.Collections.Generic;
using Codestetic.Web.Core.Domain.Map;

namespace Codestetic.Web.Services.Map
{
    public interface IMapLayerService
    {
        IList<GroupMapLayer> GetGroupMapLayers();
        IList<MapLayer> GetMapLayers();
        IList<MapLayer> GetMapLayersByGroup(GroupMapLayer group);
    }
}
