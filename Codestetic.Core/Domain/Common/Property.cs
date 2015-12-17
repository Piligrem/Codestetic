using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Codestetic.Core.Domain.Common
{
    [DataContract]
    class Property : BaseEntity
    {
        #region Properties
        /// <summary>
        /// Gets or sets name property
        /// </summary>
        [DataMember]
        public string Name { get; set; }
        public string Description { get; set; }
        #endregion Properties
    }
}
