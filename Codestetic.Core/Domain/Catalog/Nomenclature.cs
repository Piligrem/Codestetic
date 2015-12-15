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
    /// Represents a nomenclature - main product(repairable equipment) 
    /// </summary>
    [DataContract]
	public partial class Nomenclature : BaseEntity, ILocalizedEntity, IAclSupported
{
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
    }
}
