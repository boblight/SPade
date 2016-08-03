using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Remoting;
using System.Security;
using System.Security.Permissions;
using System.Security.Policy;
using System.Web;
using System.Xml;

namespace SPade.Grading
{
    public class Sandboxer : MarshalByRefObject
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

        const string pathToUntrusted = @"C:/Users/tongliang/Documents/Visual Studio 2015/Projects/Grade/Grade/bin/Debug";
        //const string pathToUntrusted = @"~/Grading";
        const string untrustedAssembly = "SPade.Grading";
        const string untrustedClass = "SPade.Grading.SandboxGrader";
        const string entryPoint = "SandboxGrading"; //method name
        private static string[] parameters = new string[4];

        public Sandboxer()
        {

        }

        //constructor
        public Sandboxer(string filePath, string fileName, int assgnId, string language)
        {
            this.filePath = filePath;
            this.fileName = fileName;
            this.assgnId = assgnId;
            this.language = language;

            setup();
        }

        public void setup()
        {
            //compile the program
            if (language == "Java")
            {
                //compile java program
                compileInfo = new ProcessStartInfo("C:/Program Files/Java/jdk1.8.0_91/bin/javac.exe", fileName + ".java");

                compileInfo.CreateNoWindow = true;
                compileInfo.UseShellExecute = false;
                compileInfo.WorkingDirectory = filePath + "/" + fileName.ToLower();
                compile = Process.Start(compileInfo);

                compile.WaitForExit();//compilation process ends

                parameters[0] = "java " + "-cp " + filePath + " " + fileName.ToLower() + "." + fileName;
            }
            else if (language == "C#")
            {
                //compile java program
                compileInfo = new ProcessStartInfo("C:/Program Files/Java/jdk1.8.0_91/bin/javac.exe", fileName + ".java");

                compileInfo.CreateNoWindow = true;
                compileInfo.UseShellExecute = false;
                compileInfo.WorkingDirectory = filePath + "/" + fileName.ToLower();
                compile = Process.Start(compileInfo);

                compile.WaitForExit();//compilation process ends

                parameters[0] = filePath + "/" + fileName.ToLower() + "/" + fileName + ".exe";
            }

            parameters[1] = HttpContext.Current.Server.MapPath(@"~/TestCase/" + assgnId + "testcase.xml");
            parameters[2] = HttpContext.Current.Server.MapPath(@"~/Solutions/" + assgnId + "solution.xml");
            parameters[3] = filePath + "/output.txt";
        }//end of setup

        public decimal runSandboxedGrading()
        {
            //Code taken from: https://msdn.microsoft.com/en-us/library/bb763046(v=vs.110).aspx

            //Setting the AppDomainSetup. It is very important to set the ApplicationBase to a folder 
            //other than the one in which the sandboxer resides.
            AppDomainSetup adSetup = new AppDomainSetup();
            adSetup.ApplicationBase = Path.GetFullPath(pathToUntrusted);

            //Setting the permissions for the AppDomain. We give the permission to execute and to 
            //read/discover the location where the untrusted code is loaded.
            PermissionSet permSet = new PermissionSet(PermissionState.Unrestricted);
            permSet.AddPermission(new SecurityPermission(SecurityPermissionFlag.Execution));

            //We want the sandboxer assembly's strong name, so that we can add it to the full trust list.
            StrongName fullTrustAssembly = typeof(Sandboxer).Assembly.Evidence.GetHostEvidence<StrongName>();

            //Now we have everything we need to create the AppDomain, so let's create it.
            AppDomain newDomain = AppDomain.CreateDomain("Sandbox", null, adSetup, permSet, fullTrustAssembly);
            //newDomain.SetData("assgnId", assgnId);
            //newDomain.SetData("executionPath", parameters[0]);

            //Use CreateInstanceFrom to load an instance of the Sandboxer class into the
            //new AppDomain.
            ObjectHandle handle = Activator.CreateInstanceFrom(
            newDomain, typeof(Sandboxer).Assembly.ManifestModule.FullyQualifiedName,
                typeof(Sandboxer).FullName
            );

            //Unwrap the new domain instance into a reference in this domain and use it to execute the 
            //untrusted code.
            Sandboxer newDomainInstance = (Sandboxer)handle.Unwrap();
            //newDomainInstance.assgnId = (int)AppDomain.CurrentDomain.GetData("assgnId");
            //newDomainInstance.pathToExecutable = (string)AppDomain.CurrentDomain.GetData("executionPath");
            return newDomainInstance.ExecuteUntrusedGrading();
            //return newDomainInstance.ExecuteUntrustedCode(untrustedAssembly, untrustedClass, entryPoint, parameters);
        }

        public decimal ExecuteUntrusedGrading()
        {
            proc = new Process();
            procInfo = new ProcessStartInfo("java", "-cp C:/Users/tongliang/Documents/FYP/projectfiles/SPade/SPade/SPade/Submissions/p1111111JavaPayIncrement4 " + "sampleqns1.SampleQns1");

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
                testCaseFile.Load("C:/Users/tongliang/Documents/FYP/projectfiles/SPade/SPade/SPade/TestCase/4testcase.xml");
                //testCaseFile.Load(HttpContext.Current.Server.MapPath(@"~/TestCase/" + 4 + "testcase.xml"));
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
                        return 2; //program fails to run
                    }//check if error

                    //get the output from solution
                    solutionFile.Load(HttpContext.Current.Server.MapPath(@"~/Solutions/" + 4 + "solution.xml"));
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

                //else
                //{
                //    //terminate sandboxie if program fails
                //    Process terminateSandbox = new Process();
                //    terminateSandbox.StartInfo = new ProcessStartInfo("\"C:/Program Files/Sandboxie/Start.exe\"  /terminate_all");
                //    terminateSandbox.StartInfo.CreateNoWindow = true;
                //    terminateSandbox.StartInfo.UseShellExecute = false;
                //    terminateSandbox.Start();
                //    terminateSandbox.WaitForExit();

                //    return 2; //program failure
                //}

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
        }

        public decimal ExecuteUntrustedCode(string assemblyName, string typeName, string entryPoint, Object[] parameters)
        {
            //code taken from: https://msdn.microsoft.com/en-us/library/bb763046(v=vs.110).aspx

            //Load the MethodInfo for a method in the new Assembly. This might be a method you know, or 
            //you can use Assembly.EntryPoint to get to the main function in an executable.
            MethodInfo target = Assembly.Load(assemblyName).GetType(typeName).GetMethod(entryPoint);
            try
            {
                //Now invoke the method.
                //return (decimal)
                return (decimal) target.Invoke(this, parameters);
            }
            catch (Exception ex)
            {
                // When we print informations from a SecurityException extra information can be printed if we are 
                //calling it with a full-trust stack.
                (new PermissionSet(PermissionState.Unrestricted)).Assert();
                //Console.WriteLine("SecurityException caught:\n{0}", ex.ToString());
                File.AppendAllText("C:/Users/tongliang/Desktop/SandboxDebuggingLog.txt", "Security exception" + ex.ToString());
                CodeAccessPermission.RevertAssert();
                //Console.ReadLine();
            }
            return 100;
        }
    }//end of sandboxer
}