using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace Codestetic.Core.Domain.Warehouse
{
    [DataContract]
    class Warehouse : BaseEntity
    {

        /// <summary>
        /// Gets or sets the name
        /// </summary>
        [DataMember]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the responsible employee 
        /// </summary>
        public int EmployeeId { get; set; }
    }
}
