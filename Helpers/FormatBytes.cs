using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NitroHttp.Helpers
{
    public static class FormatBytes
    {
        public static string Format(long bytes)
        {
            return bytes switch
            {
                < 1024 => $"{bytes}B",
                < 1024 * 1024 => $"{bytes / 1024.0:F1}KB",
                _ => $"{bytes / (1024.0 * 1024.0):F1}MB"
            };
        }
    }
}
