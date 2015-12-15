using System.Collections;

namespace Codestetic.Web.Areas.Admin.Infrastructure
{
    public interface IGridModel
    {
        object Aggregates { get; }
        IEnumerable Data { get; }
        int Total { get; }
    }
}
