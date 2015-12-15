using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Codestetic.Core.Domain.Catalog 
{
    /// <summary>
    /// Represents a product
    /// </summary>
    [DataContract]
    class WarrantyPeriod : BaseEntity
    {
        /// <summary>
        /// Gets or sets the date and time of warranty period for product
        /// </summary>
        [DataMember]
        public DateTime WarrantyPeriod { get; set; }

        /// <summary>
        /// Gets or sets tag working day of warranty period
        /// </summary>
        [DataMember]
        public bool WorkingDay { get; set; }
    }
}
