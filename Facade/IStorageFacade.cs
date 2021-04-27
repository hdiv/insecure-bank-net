using System.IO;

namespace insecure_bank_net.Facade
{
    public interface IStorageFacade
    {
        bool Exists(string file);

        byte[] Load(string image);

        void Save(Stream inputStream, string name);
    }
}