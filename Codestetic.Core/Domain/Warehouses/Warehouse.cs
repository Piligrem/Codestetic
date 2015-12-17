using Codestetic.Core.Domain.Employees;
using System.Runtime.Serialization;

namespace Codestetic.Core.Domain.Warehouses
{
    [DataContract]
    public class Warehouse : BaseEntity
    {
        #region Properties
        /// <summary>
        /// Gets or sets the name
        /// </summary>
        [DataMember]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the responsible employee 
        /// </summary>
        public int EmployeeId { get; set; }
        #endregion Properties

        #region Navigation properties
        public virtual Employee Employee { get; set; }
        #endregion Navigation properties
    }
}
