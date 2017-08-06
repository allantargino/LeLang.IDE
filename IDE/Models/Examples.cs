using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IDE
{
    public class Examples
    {
        public IEnumerable<string> GetExample(string file)
        {
            var path = Settings.ExamplesDirectory;
            var filePath = $"{path}/{file}";
            if (File.Exists(filePath))
            {
                return File.ReadLines(filePath);
            }
            else
            {
                throw new FileNotFoundException(file);
            }
        }
    }
}
