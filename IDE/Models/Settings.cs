using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IDE
{
    public static class Settings
    {
        public static string PowerShell = @"C:\Windows\System32\WindowsPowerShell\v1.0\powershell.exe";

        public static string LeLangHome = @"C:\Repos\LeLang\src\";

        public static string LeLangCompileScript = @".\compile_le.ps1";

        public static string ExamplesDirectory = @"C:\Repos\LeLang\examples\";

        public static string InputTempFile = @"C:\Repos\LeLang\examples\temp.le";

        public static string ErrorFile = @"C:\Repos\LeLang\examples\errors.csv";

        public static string OutputTempFile = @"C:\Repos\LeLang\examples\temp.c";
    }
}
