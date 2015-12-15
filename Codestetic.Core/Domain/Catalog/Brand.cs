using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Codestetic.Core.Domain.Catalog
{
    [DataContract]
    class Brand : BaseEntity
    {
        /// <summary>
        /// Gets or sets name
        /// </summary>
        [DataMember]
        public DateTime Name { get; set; }

    }
}
