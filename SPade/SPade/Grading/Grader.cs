using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;
using System.Text;
using System.IO;
using System.Xml;

namespace SPade.Grading
{
    public class Grader
    {
        private string ans, error, subOut, solOut;
        private string[] output;
        private int exitcode, testCaseFailed, noOfTestCase;
        private ProcessStartInfo procInfo;
        private Process proc;
        private List<object> testcases = new List<object>();

        public Grader()
        {

        }//end of constructor

        public double grade(string filePath, int assgnId)
        {
            procInfo = new ProcessStartInfo("java.exe", "-jar " + HttpContext.Current.Server.MapPath(@"~/App_Data/Submissions/" + filePath));
            procInfo.CreateNoWindow = true;
            procInfo.UseShellExecute = false;

            //redirect standard output and error
            procInfo.RedirectStandardError = true;
            procInfo.RedirectStandardOutput = true;
            procInfo.RedirectStandardInput = true;

            proc = Process.Start(procInfo);

            //read test cases
            using (XmlReader reader = XmlReader.Create(HttpContext.Current.Server.MapPath(@"C:/Users/tongliang/Documents/FYP/projectfiles/SPade/SPade/SPade/App_Data/TestCase/" + assgnId + "testcase.xml")))
            {
                while (reader.Read())
                {
                    if (reader.Name == "input")
                    {
                        testcases.Add(reader.ReadString());
                    }
                }
            }//end of xml reader

            System.IO.StreamWriter sw = proc.StandardInput;
            foreach (object testcase in testcases)
            {
                noOfTestCase++;

                if (testcase != null)
                {
                    try
                    {
                        sw.WriteLine(testcase);
                    }
                    catch (Exception e)
                    {
                        testCaseFailed++;
                        break;
                    }
                    finally
                    {
                        sw.Flush();
                    }
                }
            }//end of input

            sw.Close();

            proc.WaitForExit();

            //read output and error
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

            foreach (string s in output)
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