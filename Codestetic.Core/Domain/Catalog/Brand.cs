using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Codestetic.Core.Domain.Customers;

namespace Codestetic.Core.Domain.Catalog
{
    [DataContract]
    class Brand : BaseEntity
    {

        private ICollection<Customer> _customer;
        /// <summary>
        /// Gets or sets name
        /// </summary>
        [DataMember]
        public DateTime Name { get; set; }
     
        #region Navigation properties
        public virtual ICollection<Customer> Customers
        {
            get { return _customer ?? (_customer = new List<Customer>()); }
            protected set { _customer = value; }
        }
        #endregion
    }
}
