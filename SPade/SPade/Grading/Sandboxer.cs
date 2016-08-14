using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Runtime.Remoting;
using System.Security;
using System.Security.Permissions;
using System.Security.Policy;
using System.Web.Hosting;

namespace SPade.Grading
{
    public class Sandboxer : MarshalByRefObject
    {
        private List<String> subList = new List<String>();
        private int assgnId;
        private List<object> testcases = new List<object>();
        private List<string> answers = new List<string>();
        public string filePath, fileName, assignmentTitle, language, pathToExecutable;
        private Compiler c;

        //replace with your own machine name
        //const string pathToUntrusted = "C:/Users/tongliang/Documents/Visual Studio 2015/Projects/Grade/Grade/bin/Debug/";
        //const string pathToUntrusted = "C:/Users/tongliang/Documents/FYP/projectfiles/SPade/SPade/SPade/Grading";
        //const string pathToUntrusted = "E:/School/Y3/SDP/SPade-MVC/SPade/SPade/Grading";
        const string pathToUntrusted = "C:/inetpub/wwwroot/Grading";
        const string untrustedAssembly = "Grade";
        const string untrustedClass = "Grade.Program";
        string entryPoint; //method name
        private static string[] parameters = new string[4];

        public Sandboxer()
        {
        }

        //constructor submission
        public Sandboxer(string filePath, string fileName, int assgnId, string language)
        {
            c = new Compiler(language, filePath, fileName);
            this.assgnId = assgnId;
            entryPoint = "Grading";

            // HttpContext.Current = ctx;

            parameters[0] = c.getExePath();
            parameters[1] = HostingEnvironment.MapPath(@"~/TestCase/" + assgnId + "testcase.xml");
            parameters[2] = HostingEnvironment.MapPath(@"~/Solutions/" + assgnId + "solution.xml");
            parameters[3] = language;
        }

        //constructor for lecturer's adding assignment
        public Sandboxer(string filePath, string fileName, string assignmentTitle, string language, bool isTestCasePresent)
        {
            c = new Compiler(language, filePath, fileName);
            entryPoint = "createSolution";

            parameters[0] = c.getExePath();
            parameters[1] = language;
            parameters[2] = HostingEnvironment.MapPath(@"~/Solutions/" + assignmentTitle + ".xml");
            if (isTestCasePresent == true)
            {
                parameters[3] = HostingEnvironment.MapPath(@"~/TestCase/" + assignmentTitle + ".xml");
            }
            else
            {
                parameters[3] = "";
            }
        }

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
            //permSet.AddPermission(new System.Security.Permissions.FileIOPermission(System.Security.Permissions.FileIOPermissionAccess.Read, "C:/Users/tongliang/Documents/FYP/projectfiles/SPade/SPade/SPade/TestCase"));
            permSet.AddPermission(new SecurityPermission(SecurityPermissionFlag.Execution));

            //We want the sandboxer assembly's strong name, so that we can add it to the full trust list.
            StrongName fullTrustAssembly = typeof(Sandboxer).Assembly.Evidence.GetHostEvidence<StrongName>();

            //Now we have everything we need to create the AppDomain, so let's create it.
            AppDomain newDomain = AppDomain.CreateDomain("Sandbox", null, adSetup, permSet, fullTrustAssembly);
            newDomain.SetData("param1", parameters[0]);
            newDomain.SetData("param2", parameters[1]);
            newDomain.SetData("param3", parameters[2]);
            newDomain.SetData("param4", parameters[3]);
            newDomain.SetData("entryPoint", entryPoint);

            //Use CreateInstanceFrom to load an instance of the Sandboxer class into the
            //new AppDomain.
            ObjectHandle handle = Activator.CreateInstanceFrom(
            newDomain, typeof(Sandboxer).Assembly.ManifestModule.FullyQualifiedName,
                typeof(Sandboxer).FullName
            );

            //Unwrap the new domain instance into a reference in this domain and use it to execute the 
            //untrusted code.
            Sandboxer newDomainInstance = (Sandboxer)handle.Unwrap();

            //retrieve the parameters stored on previous instance of Sandboxer class
            parameters[0] = newDomain.GetData("param1").ToString();
            parameters[1] = newDomain.GetData("param2").ToString();
            parameters[2] = newDomain.GetData("param3").ToString();
            parameters[3] = newDomain.GetData("param4").ToString();
            this.entryPoint = newDomain.GetData("entryPoint").ToString();

            return newDomainInstance.ExecuteUntrustedCode(untrustedAssembly, untrustedClass, entryPoint, parameters);
        }

        public decimal ExecuteUntrustedCode(string assemblyName, string typeName, string entryPoint, string[] parameters)
        {
            //code taken from: https://msdn.microsoft.com/en-us/library/bb763046(v=vs.110).aspx

            //Load the MethodInfo for a method in the new Assembly. This might be a method you know, or 
            //you can use Assembly.EntryPoint to get to the main function in an executable.
            MethodInfo target = Assembly.Load(assemblyName).GetType(typeName).GetMethod(entryPoint);

            try
            {
                //Now invoke the method.
                return (decimal)target.Invoke(null, new Object[] { parameters });
            }
            catch (Exception ex)
            {
                // When we print informations from a SecurityException extra information can be printed if we are 
                //calling it with a full-trust stack.
                (new PermissionSet(PermissionState.Unrestricted)).Assert();
                //Console.WriteLine("SecurityException caught:\n{0}", ex.ToString());
                CodeAccessPermission.RevertAssert();
                return 2;
            }
        }
    }//end of sandboxer
}