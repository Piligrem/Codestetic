using System.Runtime.Serialization;

namespace Specter.Web.Core.Domain.Common
{
    [DataContract]
    public partial class GenericAttribute : BaseEntity
    {
        /// <summary>
        /// Gets or sets the entity identifier
        /// </summary>
        [DataMember]
        public long EntityId { get; set; }

        /// <summary>
        /// Gets or sets the key group
        /// </summary>
        [DataMember]
        public string KeyGroup { get; set; }

        /// <summary>
        /// Gets or sets the key
        /// </summary>
        [DataMember]
        public string Key { get; set; }

        /// <summary>
        /// Gets or sets the value
        /// </summary>
        [DataMember]
        public string Value { get; set; }
    }
}
