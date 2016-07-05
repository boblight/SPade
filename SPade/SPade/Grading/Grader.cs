using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;
using System.Text;
using System.IO;

namespace SPade.Grading
{
    public class Grader
    {
        private string ans, error, subOut, solOut;
        private string[] output;
        private int exitcode;
        private ProcessStartInfo procInfo;
        private Process proc;

        public Grader()
        {

        }//end of constructor

        public double grade(string filePath, int assgnId)
        {
            procInfo = new ProcessStartInfo("java.exe", "-jar " + HttpContext.Current.Server.MapPath(@filePath));
            procInfo.CreateNoWindow = true;
            procInfo.UseShellExecute = false;

            //redirect standard output and error
            procInfo.RedirectStandardError = true;
            procInfo.RedirectStandardOutput = true;
            procInfo.RedirectStandardInput = true;

            proc = Process.Start(procInfo);

            /*
            System.IO.StreamWriter sw = proc.StandardInput;
            sw.WriteLine(5);
            sw.Flush();
            sw.WriteLine(5);
            sw.Flush();
            sw.Close();
            */

            proc.WaitForExit();

            //read output and error
            //ans = proc.StandardOutput.ReadToEnd();
            error = proc.StandardError.ReadToEnd();
            exitcode = proc.ExitCode; //0 means success 1 means failure

            //get output from submission
            do
            {
                subOut = proc.StandardOutput.ReadLine();
                ans += subOut;
            } while (subOut != null);

            //get the output from solution
            output = File.ReadAllLines(HttpContext.Current.Server.MapPath(@"~/App_Data/Solutions/" + assgnId + "solution.txt"));

            foreach(string s in output)
            {
                solOut += s;
            }

            if (exitcode == 0)
            {
                if (ans.Equals(solOut))
                {
                    return 1;
                }
                else
                {
                    return 0;
                }
            }
            else //means fail
            {
                return 2; //debug
            }
        }//end of grade method

    }//end of class
}