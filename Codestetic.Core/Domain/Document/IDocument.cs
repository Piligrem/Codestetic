using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Codestetic.Core.Domain.Document
{
    public interface IDocument
    {
        DateTime Date         { get; set; }
        int      Number       { get; set; }
        object   Region       { get; set; }  // Gets or sets a value Region (City, Provance)
        Guid     AuthorId     { get; set; }
        string   Comment      { get; set; }
        DateTime UpdatedOnUtc { get; set; }  // Gets or sets a value last update document in format UTC
        bool     Active       { get; set; }  // Gets or sets a value indicating whether the entity has been deleted
    }
    
    public class Document : BaseEntity, IDocumentDetails
    {
    }
}
