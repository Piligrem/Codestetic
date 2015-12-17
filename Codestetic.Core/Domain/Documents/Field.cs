using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Codestetic.Core.Domain.Documents
{
    public class Field
    {
        #region Properties
        public string Name { get; set; }
        public Type Type { get; set; }
        public object Value { get; set; }
        #endregion Properties
    }
}
