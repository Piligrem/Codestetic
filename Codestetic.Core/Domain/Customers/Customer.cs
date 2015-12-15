using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using Codestetic.Core.Domain.Common;

namespace Codestetic.Core.Domain.Customers
{
    [DataContract]
    public partial class Customer : BaseEntity
    {
        /// <summary>
        /// Gets or sets Name
        /// </summary>
        [DataMember]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets BrandId
        /// </summary>
        [DataMember]
        public int BrandId { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the customer is active
        /// </summary>
        public bool Active { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the customer has been deleted
        /// </summary>
        public bool Deleted { get; set; }

        /// <summary>
        /// Gets or sets the date and time of entity creation
        /// </summary>
        public DateTime CreatedOnUtc { get; set; }

    }
}
