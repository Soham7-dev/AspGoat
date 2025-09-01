using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;

namespace AspGoat.Models
{
    public class DeserializationDemo : SafeMessage
    {
        public string? cmd { get; set; }

        [OnDeserialized] // A magic method
        internal void Command(StreamingContext context)
        {
            string shell, args;

            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                shell = "cmd.exe";
                args = $"/c {cmd}";
            }
            else
            {
                shell = "/bin/sh";
                args = $"-c \"{cmd}\"";
            }

            Process.Start(shell, args);
        }
    }
}