using Codestetic.Web.Core.Domain.Security;
using System.Collections.Generic;

namespace Codestetic.Web.Services.Security
{
    public interface IPermissionProvider
    {
        IEnumerable<PermissionRecord> GetPermissions();
        IEnumerable<DefaultPermissionRecord> GetDefaultPermissions();
    }
}
