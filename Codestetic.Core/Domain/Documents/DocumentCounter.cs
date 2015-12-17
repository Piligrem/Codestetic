using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Codestetic.Core.Domain.Documents
{
    public class DocumentCounter: BaseEntity
    {
        /// <summary>
        /// Gets or sets value DocumentDetails
        /// </summary>
        public virtual Guid DocumentTypeId { get; set; }

        /// <summary>
        /// Gets or sets value last document number
        /// </summary>
        public int LastCount
        {
            get
            {
                int count = GetLastCount(this.DocumentTypeId);
                //To do
                return count;
            }
            set 
            {
                LastCount = (int)value; 
            }

        }

    }
}
