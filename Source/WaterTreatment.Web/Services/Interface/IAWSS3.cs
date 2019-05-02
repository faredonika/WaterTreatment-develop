using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;


namespace WaterTreatment.Web.Services.Interface
{
    public interface IAWSS3
    {
        string CreateBucket();
        Guid UploadFile(Stream data);
        Stream GetFile(Guid id);
        void Update(Guid id, Stream data);
        void Remove(Guid id);
    }
}