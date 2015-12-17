using System.Runtime.Serialization;
using Codestetic.Core.Domain.Catalog;

namespace Codestetic.Core.Domain.Employees
{
    [DataContract]
    class CategoryRepair : BaseEntity
    {
        /// <summary>
        /// Gets or sets the name
        /// </summary>
        [DataMember]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the brand
        /// </summary>
        [DataMember]
        public int BrandId { get; set; }

        #region Navigation properties
        public Brand Brand { get; set; }
        #endregion Navigation properties
    }
}
