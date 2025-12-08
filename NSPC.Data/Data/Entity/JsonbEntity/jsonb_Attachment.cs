using System;

namespace NSPC.Data
{
    public class jsonb_Attachment
    {
        public Guid Id { get; set; }
        public string DocType { get; set; }
        public string FileName { get; set; }
        public string FileType { get; set; }
        public string FilePath { get; set; }
        public long FileSize { get; set; }
        public string Description { get; set; }
    }
}