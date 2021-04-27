using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Text;

namespace insecure_bank_net.Bean
{
    [Serializable]
    public class FileUntrusted
    {
        private string username;

        public FileUntrusted(string username)
        {
            this.username = username;
        }
        
        [OnDeserializing]
        internal void OnDeserializingMethod(StreamingContext context)
        {
            Debug.WriteLine("Constructor parent");

            var output = new StringBuilder();
            var error = new StringBuilder();
            var startInfo = new ProcessStartInfo
            {
                FileName = RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ? "cmd" : "sh",
                Arguments = RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ? "/c date /T" : "-c date",
                CreateNoWindow = true,
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true
            };
            var proc = new Process {StartInfo = startInfo, EnableRaisingEvents = true};
            proc.OutputDataReceived += (sender, e) => output.Append(e.Data).Append("\n");
            proc.ErrorDataReceived += (sender, e) => error.Append(e.Data).Append("\n");
            if (!proc.Start())
            {
                throw new Exception("Process " + startInfo.FileName + " failed to start");
            }

            proc.BeginOutputReadLine();
            proc.BeginErrorReadLine();
            proc.WaitForExit();
            Debug.WriteLine("Here is the standard output of the command:\n");
            Debug.WriteLine(output.ToString());
            Debug.WriteLine("Here is the standard error of the command (if any):\n");
            Debug.WriteLine(error.ToString());
            Debug.WriteLine("Process exitValue: " + proc.ExitCode);
        }

        public override string ToString()
        {
            return $"This is: {username}";
        }
    }
}