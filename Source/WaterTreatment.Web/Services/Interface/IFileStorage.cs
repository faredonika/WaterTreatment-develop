using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace WaterTreatment.Web.Services.Interface
{
    public interface IFileStorage
    {
        Guid Add(Stream data);
        void Update(Guid id, Stream data);
        void Remove(Guid id);
        Stream Get(Guid id);
    }
}