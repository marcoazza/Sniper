using System;
namespace WiFiLoc_App
{
    public class Logger
    {
        public static void log(String lines)
        {

            // Write the string to a file.append mode is enabled so that the log
            // lines get appended to  test.txt than wiping content and writing the log
            string rp = Environment.CurrentDirectory.ToString();

            System.IO.StreamWriter file = new System.IO.StreamWriter(rp + "\\log.txt", true);
            file.WriteLine(lines);

            file.Close();

        }
    }
}