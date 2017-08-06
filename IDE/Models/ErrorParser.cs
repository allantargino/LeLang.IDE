using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IDE
{
    public class ErrorParser
    {
        private string _file;

        public ErrorParser(string file)
        {
            if (string.IsNullOrEmpty(file)) throw new ArgumentNullException(nameof(file));

            if (File.Exists(file))
                _file = file;
            else
                throw new FileNotFoundException(file);
        }

        public IEnumerable<Error> GetErrors()
        {
            var errorList = new List<Error>();
            var content = File.ReadAllLines(_file);
            foreach (string line in content)
            {
                var fields = line.Split(';');
                errorList.Add(new Error()
                {
                    Category = (ErrorCategory) Enum.Parse(typeof(ErrorCategory), fields[0]),
                    Code = int.Parse(fields[1]),
                    Message = fields[2],
                    Line = int.Parse(fields[3])
                });
            }
            return errorList;
        }
    }
}