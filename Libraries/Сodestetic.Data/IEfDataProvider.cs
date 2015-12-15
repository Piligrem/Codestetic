using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using Codestetic.Web.Core.Data;

namespace Codestetic.Web.Data
{
    public interface IEfDataProvider : IDataProvider
    {
        /// <summary>
        /// Get connection factory
        /// </summary>
        /// <returns>Connection factory</returns>
        IDbConnectionFactory GetConnectionFactory();

    }
}
