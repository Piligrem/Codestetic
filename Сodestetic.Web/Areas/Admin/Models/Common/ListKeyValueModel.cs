using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Codestetic.Web.Areas.Admin.Models.Common
{
    public class ListParametersModel
    {
        #region Fields
        private IDictionary<string, object> entries;
        #endregion Fields

        #region Constructors
        public ListParametersModel()
        {
            entries = new Dictionary<string, object>();
        }
        #endregion Constructors

        #region Properties
        public IList<KeyValueModel> Items { get; set; }
        #endregion Properties

        #region Methods
        public void Add(string parameter, object value)
        {
            entries.Add(parameter, value);
        }

        public object this[string parameter] { get { return entries[parameter]; } }
        #endregion Methods

        #region Private methods
        #endregion Private methods
    }
}