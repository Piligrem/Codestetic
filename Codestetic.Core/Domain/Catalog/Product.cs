using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Codestetic.Core.Domain.Localization;
using Codestetic.Core.Domain.Security;
using Codestetic.Core.Domain.Common;
using Codestetic.Core.Domain.Customers;

namespace Codestetic.Core.Domain.Catalog
{
    /// <summary>
    /// Represents a product
    /// </summary>
    [DataContract]
	public partial class Product : BaseEntity, ILocalizedEntity, IAclSupported
    {
        private ICollection<Property> _productProperty;
        private ICollection<ProductGroup> _productGroup;
        private ICollection<ProductPicture> _productPictures;

        /// <summary>
        /// Gets or sets the name
        /// </summary>
        [DataMember]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the short description
        /// </summary>
        [DataMember]
        public string ShortDescription { get; set; }

        /// <summary>
        /// Gets or sets the full description
        /// </summary>
        [DataMember]
        public string FullDescription { get; set; }

        /// <summary>
        /// Gets or sets the admin comment
        /// </summary>
        public string AdminComment { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the entity is published
        /// </summary>
        public bool Published { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the entity has been deleted
        /// </summary>
        public bool Deleted { get; set; }

        /// <summary>
        /// Gets or sets the date and time of product creation
        /// </summary>
        public DateTime CreatedOnUtc { get; set; }

        /// <summary>
        /// Gets or sets the date and time of product update
        /// </summary>
        public DateTime UpdatedOnUtc { get; set; }


        /// <summary>
        /// Gets or sets the customer
        /// </summary>
        [DataMember]
        public Customer CustomerId { get; set; }

        /// <summary>
        /// Gets or sets the brand
        /// </summary>
        [DataMember]
        public Brand BrandId { get; set; }


        /// <summary>
        /// Gets or sets the warranty period
        /// </summary>
        [DataMember]
        public WarrantyPeriod warrantyPeriod { get; set; }

        #region Navigation properties
        /// <summary>
        /// Gets or sets the collection of ProductCategory
        /// </summary>
        [DataMember]
        public virtual ICollection<ProductGroup> ProductGroups
        {
            get { return _productGroup ?? (_productGroup = new List<ProductGroup>()); }
            protected set { _productGroup = value; }
        }

        /// <summary>
        /// Gets or sets the collection of ProductPicture
        /// </summary>
        public virtual ICollection<ProductPicture> ProductPictures
        {
            get { return _productPictures ?? (_productPictures = new List<ProductPicture>()); }
            protected set { _productPictures = value; }
        }
        #endregion
    }
}

