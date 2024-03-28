using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace revitProtec.Classes
{
    internal class Global
    {
        internal static Assembly ExecutingAssembly = Assembly.GetExecutingAssembly();
        internal static string ExecutingPath = Path.GetDirectoryName(ExecutingAssembly.Location);
        internal static string TempPath = Environment.GetEnvironmentVariable("TMP", EnvironmentVariableTarget.User);
        internal static string RevitVersion { get; set; }
        internal static string Version = ExecutingAssembly.GetName().Version.ToString();
    }
}
