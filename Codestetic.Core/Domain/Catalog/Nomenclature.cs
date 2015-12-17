using System.Collections.Generic;
using System.Runtime.Serialization;
using Codestetic.Core.Domain.Localization;
using Codestetic.Core.Domain.Security;

namespace Codestetic.Core.Domain.Catalog
{
    /// <summary>
    /// Represents a nomenclature - main product(repairable equipment) 
    /// </summary>
    [DataContract]
	public partial class Nomenclature : BaseEntity, ILocalizedEntity, IAclSupported, ISoftDeletable
    {
        #region Fields
        private ICollection<Product> _product;
        #endregion Fields

        #region Properties
        /// <summary>
        /// Gets or sets the name
        /// </summary>
        [DataMember]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the brand id
        /// </summary>
        [DataMember]
        public int BrandId { get; set; }

        /// <summary>
        /// Gets or sets the description
        /// </summary>
        [DataMember]
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the entity has been deleted
        /// </summary>
        public bool Deleted { get; set; }
        #endregion Properties

        #region Navigation properties
        /// <summary>
        /// Gets or sets the product
        /// </summary>
        public virtual ICollection<Product> Products
        {
            get { return _product ?? (_product = new List<Product>()); }
            protected set { _product = value; }
        }
        public virtual Brand Brand { get; set; }
        #endregion Navigation properties
    }
   
}
