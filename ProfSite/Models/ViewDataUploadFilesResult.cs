using System;
using System.Collections.Generic;
using System.Security.Cryptography;

namespace ProfSite.Models
{

    public class UploadFilesResults
    {
        public List<ViewDataUploadFilesResult> files { get; set; }
        public int queuedcontentid { get; set; }
    }

    public class ViewDataUploadFilesResult
    {
        public string url { get; set; }
        public string thumbnail_url { get; set; }
        public string name { get; set; }
        public string type { get; set; }
        public int size { get; set; }
        public string delete_url { get; set; }
        public string delete_type { get; set; }
        public int attachmentId { get; set; }
    }

   
}