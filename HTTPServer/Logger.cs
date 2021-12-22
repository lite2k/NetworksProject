using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace HTTPServer
{
    class Logger
    {
        public static void LogException(Exception ex)
        {
            FileStream fs = new FileStream("log.txt", FileMode.OpenOrCreate, FileAccess.Write);
            StreamWriter sr = new StreamWriter(fs);
            // TODO: Create log file named log.txt to log exception details in it
            //Datetime:
            //message:
            // for each exception write its details associated with datetime 
            sr.WriteLine(DateTime.Now.ToString());
            sr.WriteLine(ex.ToString());
            sr.Close();
            fs.Close();
        }
    }
}
