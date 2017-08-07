using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IDE
{
    public static class LeCompiler
    {

        public static Process process;

        static LeCompiler()
        {
            process = new Process();
        }

        public static void Compile(string input)
        {

            if (File.Exists(Settings.ErrorFile))
                File.Delete(Settings.ErrorFile);
            if (File.Exists(Settings.OutputTempFile))
                File.Delete(Settings.OutputTempFile);

            //Save input
            File.WriteAllText(Settings.InputTempFile, input);

            //Call compiler
            var args = $"-File {Settings.LeLangCompileScript} {Settings.InputTempFile} {Settings.OutputTempFile}";

            process.StartInfo.WorkingDirectory = Settings.LeLangHome;
            process.StartInfo.FileName = Settings.PowerShell;
            process.StartInfo.Arguments = args;
            process.Start();

            //Wait the process finishes
        }

        public static int GetProcessStatus()
        {
            try
            {
               return process.ExitCode;
            }
            catch (Exception)
            {
                return -1;
            }
        }

        public static CompilationResult GetCompilationResults()
        {
            //Check for errors
            var errorFile = new FileInfo(Settings.ErrorFile);
            //Check for success
            var outputFile = new FileInfo(Settings.OutputTempFile);

            if (errorFile.Exists)
                return CompilationResult.Error;
            else if (outputFile.Exists)
                return CompilationResult.Success;
            else
                throw new FileNotFoundException($"Neither {Settings.ErrorFile} or {Settings.OutputTempFile} were found. Errors should happed during the Le Compilation.");
        }
    }
}
