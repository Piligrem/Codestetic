using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Codestetic.Core.Domain.Catalog
{
    [DataContract]
    class ProductGroup : BaseEntity
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

        [DataMember]
        public string Description { get; set; }
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
        #endregion
    }
}
