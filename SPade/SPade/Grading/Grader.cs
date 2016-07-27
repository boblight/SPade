using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Web;
using System.IO;
using System.Xml;

namespace SPade.Grading
{
    public class Grader
    {
        private string ans, error, subOut, solOut;
        private List<String> subList = new List<String>();
        private bool programFailed = false;
        private decimal testCasePassed;
        private int exitcode, assgnId;
        private int noOfTestCase;
        private ProcessStartInfo procInfo, compileInfo;
        private Process proc, compile;
        private List<object> testcases = new List<object>();
        private List<string> answers = new List<string>();
        public string filePath, fileName, assignmentTitle, language, pathToExecutable;
        private bool isRun = false, isTestCasePresnt = false;
        private string[] arguments;
        XmlNode docNode, bodyNode, solutionsNode;
        XmlDocument slnDoc = new XmlDocument();

        //Lecturer use this
        public Grader(string filePath, string fileName, string assignmentTitle, string language, bool isTestCasePresent)
        {
            this.filePath = filePath;
            //fileName is the subfolder that contains the solution
            this.fileName = fileName;
            //this is just the assignment title
            this.assignmentTitle = assignmentTitle;
            this.language = language;
            this.isTestCasePresnt = isTestCasePresent;

        }//end of constructor

        //student use this
        public Grader(string filePath, string fileName, int assgnId, string language)
        {
            this.filePath = filePath;
            this.fileName = fileName;
            this.assgnId = assgnId;
            this.language = language;
        }//end of overloaded constructor

        public void processForJava()
        {
            //compile java program
            compileInfo = new ProcessStartInfo("C:/Program Files/Java/jdk1.8.0_91/bin/javac.exe", fileName + ".java");

            compileInfo.CreateNoWindow = true;
            compileInfo.UseShellExecute = false;
            compileInfo.WorkingDirectory = filePath + "/" + fileName.ToLower();
            compile = Process.Start(compileInfo);

            compile.WaitForExit();//compilation process ends

            //run program with Java
            //pathToExecutable = "java -cp " + filePath + "/" + fileName + "/src " + fileName.ToLower() + "." + fileName;
            //procInfo = new ProcessStartInfo("\"C:/Program Files/Sandboxie/Start.exe\"", "C:/Users/tongliang/Documents/Visual Studio 2015/Projects/Grade/Grade/bin/Debug/Grade.exe");
            //procInfo = new ProcessStartInfo("\"C:/Program Files/Sandboxie/Start.exe\"", "/hide_window " + "java -cp " + filePath + "/" + fileName + "/src " + fileName.ToLower() + "." + fileName);
            procInfo = new ProcessStartInfo("java", "-cp " + filePath + " " + fileName.ToLower() + "." + fileName);
        }//end of processForJava

        public void processForCS()
        {
            //compile c# program
            compileInfo = new ProcessStartInfo("C:/Windows/Microsoft.NET/Framework64/v4.0.30319/csc.exe", "Program.cs");
            compileInfo.CreateNoWindow = true;
            compileInfo.UseShellExecute = false;
            compileInfo.WorkingDirectory = filePath + "/" + fileName.ToLower();
            compile = Process.Start(compileInfo);

            compile.WaitForExit();//compilation process ends

            //pathToExecutable = filePath + "/" + fileName + "/" + fileName + "/bin/Debug/" + fileName + ".exe";
            //procInfo = new ProcessStartInfo("\"C:/Program Files/Sandboxie/Start.exe\"", HttpContext.Current.Server.MapPath(@"~/Grading/Grade.exe"));
            //procInfo = new ProcessStartInfo("\"C:/Program Files/Sandboxie/Start.exe\" ", "/hide_window " + filePath + "/" + fileName + "/" + fileName + "/bin/Debug/" + fileName + ".exe");
            //procInfo = new ProcessStartInfo(filePath + "/" + fileName + "/" + fileName + "/Program.exe");
            procInfo = new ProcessStartInfo(filePath + "/" + fileName.ToLower() + "/" + fileName + ".exe");
        }//end of processForCS

        public decimal grade()
        {
            //decide which process to use
            switch (language)
            {
                case "Java":
                    processForJava();
                    break;
                case "C#":
                    processForCS();
                    break;
                default:
                    processForJava();
                    break;
            }

            ////create a new file for grading executable to append output to
            //File.AppendAllText(filePath + "/output.txt", "");

            ////arguments to be passed into the grading executable
            ////1st string is path to submission
            ////2nd string is path to test case
            ////3rd string is path to solution
            ////4th string is path to output
            //arguments = new string[4];
            //arguments[0] = pathToExecutable;
            //arguments[1] = HttpContext.Current.Server.MapPath(@"~/TestCase/" + assgnId + "testcase.xml");
            //arguments[2] = HttpContext.Current.Server.MapPath(@"~/Solutions/" + assgnId + "solution.xml");
            //arguments[3] = filePath + "/output.txt";

            ////File.AppendAllText("C:/Users/tongliang/Desktop/viewarguments.txt", arguments[0] + " " + arguments[1] + " " + arguments[2] + " " + arguments[3]);

            //procInfo.CreateNoWindow = true;
            //procInfo.UseShellExecute = false;
            //procInfo.Arguments = arguments[0] + " " + arguments[1] + " " + arguments[2] + " " + arguments[3];

            ////run sandbox grading
            //proc = Process.Start(procInfo);

            //proc.WaitForExit(10000);

            ////once Grading process is done, retrive output and return to controller
            //return Decimal.Parse(File.ReadAllText(filePath + "/output.txt"));



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
                    List<string> inputs = new List<string>();
                    noOfTestCase++;
                    proc = Process.Start(procInfo);
                    subOut = "";

                    System.IO.StreamWriter sw = proc.StandardInput;

                    foreach (XmlNode input in testcase.ChildNodes)
                    {
                        inputs.Add(input.InnerText);
                        sw.WriteLine(input.InnerText);
                        sw.Flush();
                    }//end of inputs

                    //check if there is another error thrown by program
                    error = proc.StandardError.ReadToEnd();

                    if (error.Equals(""))
                    {
                        //scan through all lines of standard output to retrieve anything
                        string checkEmpty;
                        do
                        {
                            checkEmpty = proc.StandardOutput.ReadLine();
                            subOut += checkEmpty;
                        } while (checkEmpty != null);

                        foreach (string input in inputs)
                        {
                            subOut += input;
                        }
                    }
                    else
                    {
                        //program given fail if an error was encountered
                        programFailed = true;
                        sw.Close();
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
                    }//end of checkW

                    if (!proc.WaitForExit(10000))
                    {
                        proc.Kill();
                    }
                    //proc.WaitForExit();
                }//end of test case loop

                //read output 
                if (programFailed == false)
                {
                    return (testCasePassed / noOfTestCase);
                }
                else
                {
                    //error logging
                    //to be deleted at final product
                    File.AppendAllText("C:/Users/tongliang/Desktop/error.txt", error);

                    //terminate sandboxie if program fails
                    Process terminateSandbox = new Process();
                    terminateSandbox.StartInfo = new ProcessStartInfo("\"C:/Program Files/Sandboxie/Start.exe\"  /terminate_all");
                    terminateSandbox.StartInfo.CreateNoWindow = true;
                    terminateSandbox.StartInfo.UseShellExecute = false;
                    terminateSandbox.Start();
                    terminateSandbox.WaitForExit();

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

        public int RunLecturerSolution()
        {
            switch (language)
            {
                case "Java":
                    processForJava();
                    break;
                case "C#":
                    processForCS();
                    break;
                default:
                    processForJava();
                    break;
            }

            procInfo.CreateNoWindow = true;
            procInfo.UseShellExecute = false;

            //redirect standard output and error
            procInfo.RedirectStandardError = true;
            procInfo.RedirectStandardOutput = true;
            procInfo.RedirectStandardInput = true;

            if (isTestCasePresnt == true)
            {
                return RunWithTestCase();
            }
            else
            {
                return RunWithoutTestCase();
            }
        }//

        public int RunWithTestCase()
        {
            //load test cases if any
            XmlDocument testCaseFile = new XmlDocument();
            XmlDocument solutionFile = new XmlDocument();

            try
            {
                //load test case
                testCaseFile.Load(HttpContext.Current.Server.MapPath(@"~/TestCase/" + assignmentTitle + ".xml"));
                XmlNodeList testcaseList = testCaseFile.SelectNodes("/body/testcase");

                //create part of the solution file first
                docNode = slnDoc.CreateXmlDeclaration("1.0", "UTF-8", null);
                slnDoc.AppendChild(docNode);
                bodyNode = slnDoc.CreateElement("body");
                slnDoc.AppendChild(bodyNode);

                foreach (XmlNode testcase in testcaseList)
                {
                    List<string> inputs = new List<string>();
                    // noOfTestCase++;
                    proc = Process.Start(procInfo);
                    subOut = "";

                    System.IO.StreamWriter sw = proc.StandardInput;

                    foreach (XmlNode input in testcase.ChildNodes)
                    {
                        inputs.Add(input.InnerText);
                        sw.WriteLine(input.InnerText);
                        sw.Flush();
                    }//end of inputs

                    //check if there is another error thrown by program
                    error = proc.StandardError.ReadToEnd();

                    if (error.Equals(""))
                    {
                        //scan through all lines of standard output to retrieve anything
                        string checkEmpty;
                        do
                        {
                            checkEmpty = proc.StandardOutput.ReadLine();
                            subOut += checkEmpty;

                        } while (checkEmpty != null);

                        foreach (string input in inputs)
                        {
                            subOut += input;
                        }

                        solutionsNode = slnDoc.CreateElement("solution");
                        solutionsNode.AppendChild(slnDoc.CreateTextNode(subOut));
                        bodyNode.AppendChild(solutionsNode);
                    }
                    else
                    {
                        //program given fail if an error was encountered
                        programFailed = true;
                        sw.Close();
                        break; //break out of loop

                    }
                    if (!proc.WaitForExit(10000))
                    {
                        proc.Kill();
                        return 4;
                    }
                    //proc.WaitForExit();
                }

                //create the solution file 
                if (programFailed == false)
                {
                    //save the XML file
                    var fP = HttpContext.Current.Server.MapPath(@"~/Solutions/" + assignmentTitle + ".xml");
                    slnDoc.Save(fP);

                    //solution has run successfully
                    return 1;
                }
                else
                {
                    return 3;
                }
            }
            catch (Exception ex)
            {
                //cannot load xml
                return 2;
            }
        }

        private int RunWithoutTestCase()
        {
            try
            {
                //create part of the solution file first
                docNode = slnDoc.CreateXmlDeclaration("1.0", "UTF-8", null);
                slnDoc.AppendChild(docNode);
                bodyNode = slnDoc.CreateElement("body");
                slnDoc.AppendChild(bodyNode);

                //start the process 
                proc = Process.Start(procInfo);

                error = proc.StandardError.ReadToEnd();

                //read the output from the program
                if (error.Equals(""))
                {

                    subOut = proc.StandardOutput.ReadToEnd();

                    solutionsNode = slnDoc.CreateElement("solution");
                    solutionsNode.AppendChild(slnDoc.CreateTextNode(subOut));
                    bodyNode.AppendChild(solutionsNode);
                }
                else
                {
                    //program given fail if an error was encountered
                    programFailed = true;
                }
                proc.WaitForExit();

                //store into the solutions file if no error
                if (programFailed == false)
                {
                    //save the XML file
                    var fP = HttpContext.Current.Server.MapPath(@"~/Solutions/" + assignmentTitle + ".xml");
                    slnDoc.Save(fP);

                    //solution has run successfully
                    return 1;
                }
            }
            catch (Exception e)
            {
                File.AppendAllText("C:/Users/tongliang/Desktop/Exception.txt", e.Message);
            }
            return 1;
        }

    }//end of class
}