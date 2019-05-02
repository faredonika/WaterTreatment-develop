using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using WaterTreatment.Web.Services.Interface;

namespace WaterTreatment.Web.Services
{
    public class FileStorage : IFileStorage
    {
        public Guid Add(Stream data)
        {
            var id = Guid.NewGuid();

            var path = Path.Combine(RootDir, id.ToString("N"));
            using (var file = File.Create(path))
            {
                data.CopyTo(file);
            }

            return id;
        }

        public void Update(Guid id, Stream data)
        {
            var path = Path.Combine(RootDir, id.ToString("N"));
            using (var file = File.OpenWrite(path))
            {
                data.CopyTo(file);
            }
        }

        public void Remove(Guid id)
        {
            var path = Path.Combine(RootDir, id.ToString("N"));
            if (File.Exists(path))
            {
                File.Delete(path);
            }
        }

        public Stream Get(Guid id)
        {
            var path = Path.Combine(RootDir, id.ToString("N"));
            if (!File.Exists(path))
            {
                throw new InvalidOperationException("Invalid Id");
            }

            var data = new MemoryStream();
            using (var file = File.OpenRead(path))
            {
                file.CopyTo(data);
            }
            data.Position = 0;

            return data;
        }

        public string RootDir { get { return HttpContext.Current.Server.MapPath("~/Content/Uploads"); } }
    }
}