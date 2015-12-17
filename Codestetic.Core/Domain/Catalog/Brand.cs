using System.Collections.Generic;
using System.Runtime.Serialization;
using Codestetic.Core.Domain.Customers;

namespace Codestetic.Core.Domain.Catalog
{
    [DataContract]
    public class Brand : BaseEntity
    {
        #region Fields
        private ICollection<Customer> _customer;
        #endregion Fields

        #region Properties
        /// <summary>
        /// Gets or sets name
        /// </summary>
        [DataMember]
        public string Name { get; set; }
        [DataMember]
        public string Url { get; set; }
        #endregion Properties

        #region Navigation properties
        public virtual ICollection<Customer> Customers
        {
            get { return _customer ?? (_customer = new List<Customer>()); }
            protected set { _customer = value; }
        }
        #endregion
    }
}
