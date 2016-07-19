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
using System.Threading;

namespace SPade.Grading
{
    public class Grader
    {
        private string ans, error, subOut, solOut;
        private List<String> subList = new List<String>();
        private string[] output;
        private bool programFailed = false;
        private decimal testCasePassed;
        private int exitcode, assgnId;
        private int noOfTestCase;
        private ProcessStartInfo procInfo, compileInfo;
        private Process proc, compile;
        private List<object> testcases = new List<object>();
        private List<string> answers = new List<string>();
        public string filePath, fileName, assignmentTitle;
        XmlNode docNode, bodyNode, solutionsNode;
        XmlDocument slnDoc = new XmlDocument();

        //Lecturer use this
        public Grader(string filePath, string fileName, string assignmentTitle)
        {
            this.filePath = filePath;
            this.fileName = fileName;
            this.assignmentTitle = assignmentTitle;
        }//end of constructor

        //student use this
        public Grader(string filePath, string fileName, int assgnId)
        {
            this.filePath = filePath;
            this.fileName = fileName;
            this.assgnId = assgnId;
        }//end of overloaded constructor

        public decimal grade()
        {
            //compile java program
            compileInfo = new ProcessStartInfo("C:/Program Files/Java/jdk1.8.0_91/bin/javac.exe", filePath + "/" + fileName + "/src/" + fileName.ToLower() + "/" + fileName + ".java");

            compileInfo.CreateNoWindow = true;
            compileInfo.UseShellExecute = false;
            compile = Process.Start(compileInfo);

            compile.WaitForExit();//compilation process ends

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
            {
                testCaseFile.Load(HttpContext.Current.Server.MapPath(@"~/TestCase/" + assgnId + "testcase.xml"));
                XmlNodeList testcaseList = testCaseFile.SelectNodes("/body/testcase");

                foreach (XmlNode testcase in testcaseList)
                {
                    noOfTestCase++;
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
                    }
                    else
                    {
                        //program given fail if an error was encountered
                        programFailed = true;
                        sw.Close();
                        proc.WaitForExit();
                        break; //break out of loop
                    }//check if error

                    if (programFailed == false) //method only run if no error/have proper error handling in submitted program
                    {
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
                    }//end of check
                    proc.WaitForExit();
                }//end of test case loop

                //read output 
                if (programFailed == false)
                {
                    return (testCasePassed / noOfTestCase);
                }
                else
                {
                    return 0;
                }
            }
            catch (Exception e) //when exception occures means failed to retrieve testcase, in turn means program does not take in inputs
            {//start catch, application does not accept input
                proc = Process.Start(procInfo);

                if (!proc.WaitForExit(10000))
                {
                    return 0;//fail program if program failed to produce feedback after 10 seconds
                }

                proc.WaitForExit();

                //read output and error
                error = proc.StandardError.ReadToEnd();
                exitcode = proc.ExitCode; //0 means success 1 means failure

                //get output from submission

                ans = proc.StandardOutput.ReadToEnd();

                //get the output from solution
                // get the output from solution
                solutionFile.Load(HttpContext.Current.Server.MapPath(@"~/Solutions/" + assgnId + "solution.xml"));
                solOut = solutionFile.SelectSingleNode("/body/solution").InnerText;

                if (exitcode == 0 && error.Equals("")) //if submission properly ran and produced desired outcome
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
                    return 0; //debug
                }
            }//end of catch
        }//end of grade method

        public bool RunLecturerSolution()
        {
            bool isRun = false;

            //method to run lecturer solution. 
            compileInfo = new ProcessStartInfo("C:/Program Files/Java/jdk1.8.0_91/bin/javac.exe", filePath + "/" + fileName + "/src/" + fileName.ToLower() + "/" + fileName + ".java");

            compileInfo.CreateNoWindow = true;
            compileInfo.UseShellExecute = false;
            compile = Process.Start(compileInfo);

            compile.WaitForExit();//compilation process ends

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
            {
                //load test case
                testCaseFile.Load(HttpContext.Current.Server.MapPath(@"~/TestCase/" + assignmentTitle + ".xml"));
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
                        subOut += proc.StandardOutput.ReadLine();
                        subList.Add(subOut); //add to list of answers 
                    }
                    else
                    {
                        //program given fail if an error was encountered
                        programFailed = true;
                        sw.Close();
                        proc.WaitForExit();
                        break; //break out of loop
                    }//check if error

                    proc.WaitForExit();
                }

                //create the solution file 
                if (programFailed == false)
                {
                    docNode = slnDoc.CreateXmlDeclaration("1.0", "UTF-8", null);
                    slnDoc.AppendChild(docNode);

                    bodyNode = slnDoc.CreateElement("body");
                    slnDoc.AppendChild(bodyNode);

                    //loop through the answers and append into the xml file 
                    foreach (string s in subList)
                    {
                        solutionsNode = slnDoc.CreateElement("solution");
                        solutionsNode.AppendChild(slnDoc.CreateTextNode(s));
                        bodyNode.AppendChild(solutionsNode);
                    }

                    //save the XML file
                    var fP = HttpContext.Current.Server.MapPath(@"~/Solutions/" + assignmentTitle + ".xml");
                    slnDoc.Save(fP);

                    isRun = true;
                }
            }
            catch (Exception ex)
            {

            }

            return isRun;

        }
    }//end of class
}