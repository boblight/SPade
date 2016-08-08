using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Web;
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
        private bool isTestCasePresnt = false;
        XmlNode docNode, bodyNode, solutionsNode;
        XmlDocument slnDoc = new XmlDocument();
        Compiler c;

        //Lecturer use this
        public Grader(string filePath, string fileName, string assignmentTitle, string language, bool isTestCasePresent)
        {
            c = new Compiler(language, filePath, fileName);

            //fileName is the subfolder that contains the solution
            //this is just the assignment title
            this.assignmentTitle = assignmentTitle;
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
            //pathToExecutable = "java " + "-cp " + filePath + " " + fileName.ToLower() + "." + fileName;
            //procInfo = new ProcessStartInfo("\"C:/Program Files/Sandboxie/Start.exe\"", "java " + "-cp " + filePath + " " + fileName.ToLower() + "." + fileName);
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

            //pathToExecutable = filePath + "/" + fileName.ToLower() + "/" + fileName + ".exe";
            procInfo = new ProcessStartInfo("\"C:/Program Files/Sandboxie/Start.exe\"", fileName + ".exe");
            procInfo.WorkingDirectory = filePath + "/" + fileName.ToLower() + "/";

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
                    File.AppendAllText("C:/Users/tongliang/Desktop/debugger.txt", "TEST");
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
                        return 2; //program fails to run
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

                    if (!proc.WaitForExit(10000))
                    {
                        proc.Kill();
                        return 3;//infinite loop
                    }
                }//end of test case loop

                return (testCasePassed / noOfTestCase); //return results
            }
            catch (Exception e) //when exception occures means failed to retrieve testcase, in turn means program does not take in inputs
            {//start catch, application does not accept input
                try
                {
                    proc = Process.Start(procInfo);
                }
                catch (Exception exc)
                {
                    return 2; //return error code if program failed to run
                }

                if (!proc.WaitForExit(10000))
                {
                    return 3; //return error code if program failed to produce feedback after 10 seconds
                }

                proc.WaitForExit();

                //read output and error
                error = proc.StandardError.ReadToEnd();
                exitcode = proc.ExitCode; //0 means success 1 means failure

                //get output from submission

                ans = proc.StandardOutput.ReadToEnd();

                //get the output from solution
                //get the output from solution
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
                    return 2;//program failure
                }
            }//end of catch
        }//end of grade method

        public int RunLecturerSolution()
        {
            //switch (language)
            //{
            //    case "Java":
            //        processForJava();
            //        break;
            //    case "C#":
            //        processForCS();
            //        break;
            //    default:
            //        processForJava();
            //        break;
            //}

            if (language == "Java")
            {
                proc = new Process();
                procInfo = new ProcessStartInfo("C:/Program Files/Java/jdk1.8.0_91/bin/java.exe", c.getExePath());
            }
            else if (language == "C#")
            {
                proc = new Process();
                procInfo = new ProcessStartInfo(c.getExePath());
            }
            else
            {
                return 5;
            }

            procInfo.CreateNoWindow = true;
            procInfo.UseShellExecute = false;

            //redirect standard output and error
            procInfo.RedirectStandardError = true;
            procInfo.RedirectStandardOutput = true;
            procInfo.RedirectStandardInput = true;

            //run the appropirate method based off a testcase is present or not
            if (isTestCasePresnt == true)
            {
                return RunWithTestCase();
            }
            else
            {
                return RunWithoutTestCase();
            }
        }

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

        public int RunWithoutTestCase()
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
                    exitcode = 3;
                }
                proc.WaitForExit();

                //store into the solutions file if no error
                if (programFailed == false)
                {
                    //save the XML file
                    var fP = HttpContext.Current.Server.MapPath(@"~/Solutions/" + assignmentTitle + ".xml");
                    slnDoc.Save(fP);

                    //solution has run successfully
                    exitcode = 1;

                }
            }
            catch (Exception e)
            {
                //File.AppendAllText("C:/Users/tongliang/Desktop/Exception.txt", e.Message);

                //any exception
                exitcode = 3;

            }
            return exitcode;
        }

    }//end of class
}