﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Specter.Web.Core.Domain.Logging
{
    public class Performance : BaseEntity
    {
        public string TaskName { get; set; }
        public TimeSpan Runtime { get; set; }
        public long UserId { get; set; }
        public DateTime CreatedOnUtc { get; set; }
    }
}
