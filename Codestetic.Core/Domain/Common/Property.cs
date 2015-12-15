using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Codestetic.Core.Domain.Common
{
    [DataContract]
    class Property : BaseEntity
    {

        /// <summary>
        /// Gets or sets name property
        /// </summary>
        [DataMember]
        public DateTime Name { get; set; }
    }
}
