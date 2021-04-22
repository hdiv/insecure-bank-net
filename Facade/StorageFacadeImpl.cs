using System.IO;
using Microsoft.AspNetCore.Hosting;

namespace insecure_bank_net.Facade
{
    public class StorageFacadeImpl : IStorageFacade
    {
        private readonly IWebHostEnvironment env;

        public StorageFacadeImpl(IWebHostEnvironment env)
        {
            this.env = env;
        }

        public bool Exists(string file)
        {
            return File.Exists(env.WebRootPath + file);
        }

        public byte[] Load(string image)
        {
            return File.ReadAllBytes(env.WebRootPath + image);
        }

        public void Save(Stream inputStream, string name)
        {
            using var fileStream = File.Create(env.WebRootPath + name);
            inputStream.CopyTo(fileStream);
        }

    }
}