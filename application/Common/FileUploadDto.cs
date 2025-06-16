using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace application.Common
{
    public class FileUploadDto
    {
        public Stream FileStream { get; }
        public string FileName { get; }
        public string ContentType { get; }

        public FileUploadDto(
            Stream fileStream,
            string fileName,
            string contentType
        )
        {
            FileStream = fileStream;
            FileName = fileName;
            ContentType = contentType;
        }
    }
}