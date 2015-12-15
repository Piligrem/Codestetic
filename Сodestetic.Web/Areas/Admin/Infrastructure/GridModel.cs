using System.Collections;
using System.Collections.Generic;

namespace Codestetic.Web.Areas.Admin.Infrastructure
{
    public class GridModel<T> : IGridModel
    {
        public GridModel() { }

        public GridModel(IEnumerable<T> data)
        {
            this.Data = data;
        }

        public object Aggregates { get; set; }

        public IEnumerable<T> Data { get; set; }

        IEnumerable IGridModel.Data
        {
            get { return this.Data; }
        }

        public int Total { get; set; }
    }
}
