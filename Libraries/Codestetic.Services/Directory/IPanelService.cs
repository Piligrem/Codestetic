using System.Collections.Generic;
using Codestetic.Web.Core.Domain.Directory;

namespace Codestetic.Web.Services.Directory
{
    /// <summary>
    /// Country service interface
    /// </summary>
    public partial interface IPanelService
    {
        IList<Panel> GetPanel(string name, bool showHidden = false);
    }
}