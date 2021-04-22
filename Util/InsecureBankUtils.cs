using System;
using System.IO;

namespace insecure_bank_net.Util
{
    public static class InsecureBankUtils
    {
        public static double Round(double value, int places)
        {
            if (places < 0) { throw new Exception("places has to be a positive integer"); }

            var factor = Math.Pow(10, places);
            value = value * factor;
            var tmp = Math.Round(value);
            return (double) tmp / factor;
        }
        
        public static string GetFileExtension(string fullName)
        {
            return Path.GetExtension(fullName);
        }

        public static string GetNameWithoutExtension(string file)
        {
            return Path.GetFileNameWithoutExtension(file);
        }
    }
}