using System;

namespace insecure_bank_net.Bean
{
    [Serializable]
    public class FileUntrustedValid
    {
        private string username;

        public FileUntrustedValid(string username)
        {
            this.username = username;
        }
    }
}