using System;
using System.ComponentModel.DataAnnotations;
using Codestetic.Web.Framework;
using Codestetic.Web.Framework.Mvc;

namespace Codestetic.Web.Areas.Admin.Models.Common
{
    public class MaintenanceModel : ModelBase
    {
        public MaintenanceModel()
        {
            DeleteGuests = new DeleteGuestsModel();
            DeleteExportedFiles = new DeleteExportedFilesModel();
            DeleteImageCache = new DeleteImageCacheModel();
        }

        public DeleteGuestsModel DeleteGuests { get; set; }
        public DeleteExportedFilesModel DeleteExportedFiles { get; set; }

        public DeleteImageCacheModel DeleteImageCache { get; set; }

        [ResourceDisplayName("Admin.System.Maintenance.SqlQuery")]
        public string SqlQuery { get; set; }


        #region Nested classes

        public class DeleteGuestsModel : ModelBase
        {
            [ResourceDisplayName("Admin.System.Maintenance.DeleteGuests.StartDate")]
            public DateTime? StartDate { get; set; }

            [ResourceDisplayName("Admin.System.Maintenance.DeleteGuests.EndDate")]
            public DateTime? EndDate { get; set; }

            public int? NumberOfDeletedUsers { get; set; }
        }

        public class DeleteExportedFilesModel : ModelBase
        {
            [ResourceDisplayName("Admin.System.Maintenance.DeleteExportedFiles.StartDate")]
            public DateTime? StartDate { get; set; }

            [ResourceDisplayName("Admin.System.Maintenance.DeleteExportedFiles.EndDate")]
            public DateTime? EndDate { get; set; }

            public int? NumberOfDeletedFiles { get; set; }
        }

        public class DeleteImageCacheModel : ModelBase
        {
            [ResourceDisplayName("Admin.System.Maintenance.DeleteImageCache.FileCount")]
            public long FileCount { get; set; }

            [ResourceDisplayName("Admin.System.Maintenance.DeleteImageCache.TotalSize")]
            public string TotalSize { get; set; }
        }
        #endregion
    }
}
