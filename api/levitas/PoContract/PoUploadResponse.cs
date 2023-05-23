using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace levitas.PoContract
{
    public class PoUploadResponse
    {
        public string FileName { get; set; }
        public string uniqueId { get; set; }
        public DateTime uploadedDate { get; set; } = DateTime.Now;
        public string error { get; set; } 
        public PoUploadStatus PoUploadStatus { get; set; }

    }
}