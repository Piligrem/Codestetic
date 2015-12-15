using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Codestetic.Core.Domain.Common
{
    [DataContract]
    class ObjectProperty
    {
        /// <summary>
        /// Gets or sets objectId 
        /// </summary>
        
        [DataMember]
        public int ObjectId { get; set; }

        /// <summary>
        /// Gets or sets object type 
        /// </summary>
        
        [DataMember]
        public int ObjectTypeId { get; set; }

        /// <summary>
        /// Gets or sets propertyId 
        /// </summary>
        
        [DataMember]
        public int PropertyId { get; set; }

        /// <summary>
        /// Gets or sets value of property 
        /// </summary>
        
        [DataMember]
        public string Value { get; set; }
    }
}
