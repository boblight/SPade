using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;
using System.Text;
using System.IO;
using System.Xml;
using SPade.ViewModels.Student;
using System.Timers;

namespace SPade.Grading
{
    public class Grader
    {
        private string ans, error, subOut, solOut;
        private List<String> subList = new List<String>();
        private string[] output;
        private bool programFailed = false;
        private decimal testCaseFailed, testCasePassed;
        private int exitcode, noOfTestCase = 2/*hardcoded*/, assgnId;
        private ProcessStartInfo procInfo, compileInfo;
        private Process proc, compile;
        private List<object> testcases = new List<object>();
        public string filePath, fileName;
        public Timer timer;

        //Lecturer use this
        public Grader(string filePath)
        {
            this.filePath = filePath;
        }//end of constructor

        //student use this
        public Grader(string filePath, string fileName, int assgnId)
        {
            this.filePath = filePath;
            this.fileName = fileName;
            this.assgnId = assgnId;
        }//end of overloaded constructor

        public Decimal grade()
        {
            timer = new Timer(10000);

            //compile java program
            compileInfo = new ProcessStartInfo("C:/Program Files/Java/jdk1.8.0_91/bin/javac.exe", filePath + "/" + fileName + "/src/" + fileName.ToLower() + "/" + fileName + ".java");

            compileInfo.CreateNoWindow = true;
            compileInfo.UseShellExecute = false;
            compile = Process.Start(compileInfo);

            compile.WaitForExit();

            //////////////debugger for error
            int temp = compile.ExitCode;
            if (temp == 1)
            {
                return 2;
            }
            ///////////////////////////////////

            //run program with Java
            procInfo = new ProcessStartInfo("java", "-cp " + filePath + "/" + fileName + "/src " + fileName.ToLower() + "." + fileName);
            procInfo.CreateNoWindow = true;
            procInfo.UseShellExecute = false;

            //redirect standard output and error
            procInfo.RedirectStandardError = true;
            procInfo.RedirectStandardOutput = true;
            procInfo.RedirectStandardInput = true;

            //load test cases if any
            XmlDocument testCaseFile = new XmlDocument();
            XmlDocument solutionFile = new XmlDocument();

            try
            {//start try*/
                testCaseFile.Load(HttpContext.Current.Server.MapPath(@"~/TestCase/" + assgnId + "testcase.xml"));
                XmlNodeList testcaseList = testCaseFile.SelectNodes("/body/testcase");

                foreach (XmlNode testcase in testcaseList)
                {
                    proc = Process.Start(procInfo);
                    subOut = "";

                    System.IO.StreamWriter sw = proc.StandardInput;

                    foreach (XmlNode input in testcase.ChildNodes)
                    {
                        sw.WriteLine(input.InnerText);
                        sw.Flush();
                        subOut += proc.StandardOutput.ReadLine() + input.InnerText;
                    }//end of inputs
                    //check if there is another error thrown by program
                    error = proc.StandardError.ReadToEnd();

                    if (error.Equals(""))
                    {
                        //add output to list of outputs if there is no error
                        subOut += proc.StandardOutput.ReadLine();
                        //subList.Add(subOut);
                    }
                    else
                    {
                        //program given fail if an error was encountered
                        programFailed = true;
                        sw.Close();
                        proc.WaitForExit();
                        break; //break out of loop
                    }//check if error

                    //get the output from solution
                    solutionFile.Load(HttpContext.Current.Server.MapPath(@"~/Solutions/" + assgnId + "solution.xml"));
                    XmlNodeList solutions = solutionFile.SelectNodes("/body/solution");

                    //loop through all the solutions to find matching
                    foreach (XmlNode solution in solutions)
                    {
                        if (subOut.Equals(solution.InnerText))
                        {
                            testCasePassed++;
                        }
                    }//end of foreach loop

                    proc.WaitForExit();
                }//loop through all test cases

                //read output 
                if (programFailed == false)
                {
                    return (testCasePassed / noOfTestCase);
                }
                else
                {
                    return 0;
                }
            }//end of try
            catch (Exception e)
            {//start catch, application does not accept input
                proc = Process.Start(procInfo);

                //check if program freezes
                foreach (ProcessThread thread in proc.Threads)
                {
                    if (thread.ThreadState == ThreadState.Wait
                        && thread.WaitReason == ThreadWaitReason.UserRequest)
                    {
                        proc.Kill();
                        return 0;
                    }
                }

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
                output = File.ReadAllLines(HttpContext.Current.Server.MapPath(@"~/Solutions/" + assgnId + "solution.txt"));

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
            }//end of catch

        }//end of grade method
    }//end of class
}