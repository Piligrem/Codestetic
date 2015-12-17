using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Codestetic.Core.Domain.Document
{
    public interface IDocumentDetails
    {
        int            DocumentId     { get; set; }
        int            DocumentTypeId { get; set; }
        IList<Details> Details        { get; set; }
    }

    [DataContract]
    public class Details
    {
        [DataMember]   
        public Type   Type  { get; set; }
    
        [DataMember]      
        public string Name  { get; set; }
    
        [DataMember]       
        public object Value { get; set; } 
    }
}
