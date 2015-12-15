
using System.Collections.Generic;
using Codestetic.Core.Domain.Employees;

namespace Codestetic.Core.Domain.Security
{
    /// <summary>
    /// Represents a permission record
    /// </summary>
    public class PermissionRecord : BaseEntity
    {
        private ICollection<EmployeeRole> _employeeRoles;

        /// <summary>
        /// Gets or sets the permission name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the permission system name
        /// </summary>
        public string SystemName { get; set; }
        
        /// <summary>
        /// Gets or sets the permission category
        /// </summary>
        public string Category { get; set; }
        
        /// <summary>
        /// Gets or sets discount usage history
        /// </summary>
        public virtual ICollection<EmployeeRole> EmployeeRoles
        {
            get { return _employeeRoles ?? (_employeeRoles = new List<EmployeeRole>()); }
            protected set { _employeeRoles = value; }
        }   
    }
}
