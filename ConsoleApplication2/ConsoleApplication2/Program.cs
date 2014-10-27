using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApplication2
{
    class Program
    {
        static void Main(string[] args)
        {
            //string pathfile = @"\\Gti\worklists\wo0000 GBGTest2.gwl";
            string pathfile = @"\\Bioreader\worklists\blah";


            string[] replace = pathfile.Split(new string[] { @"\" }, StringSplitOptions.None);

            string again = string.Empty;
            bool start = false;

            foreach (string s in replace)
            {
                if (s.Contains("worklists"))
                {
                    start = true;
                }
                if (start)
                {
                    again += @"/" + s;
                }
                
            }

            pathfile = (@"C:/Documents and Settings/Administrator/Desktop") + again;

            Console.WriteLine(pathfile);


        }
    }
}
