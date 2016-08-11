using System;
using System.Diagnostics;

namespace SPade.Grading
{
    public class Compiler
    {
        //the purpose of this class is to compile the programmes for respective programming languages
        //this file is modifiable to allow for more programming languages to be added

        private ProcessStartInfo compileInfo;
        private Process compile;
        private string pathToExecutable, filePath, fileName;

        //additional cases can be added to allow more programming languages to be marked
        public Compiler(string language, string filePath, string fileName)
        {
            this.filePath = filePath;
            this.fileName = fileName;
            switch (language)
            {
                case "Java":
                    try
                    {
                        //compile java program
                        compileInfo = new ProcessStartInfo("C:/Program Files/Java/jdk1.8.0_91/bin/javac.exe", fileName + ".java");

                        compileInfo.CreateNoWindow = true;
                        compileInfo.UseShellExecute = false;
                        compileInfo.WorkingDirectory = filePath + "/" + fileName.ToLower();
                        compile = Process.Start(compileInfo);

                        compile.WaitForExit(5000);//compilation process ends
                        
                        //procInfo = new ProcessStartInfo("java", "-cp " + filePath + " " + fileName.ToLower() + "." + fileName);
                    }
                    catch (Exception e)
                    {
                        pathToExecutable = "error";//signal that compilation error
                        break;
                    }
                    pathToExecutable = "-cp " + filePath + " " + fileName.ToLower() + "." + fileName;
                    break;
                case "C#":
                    try
                    {
                        //compile c# program
                        compileInfo = new ProcessStartInfo("C:/Windows/Microsoft.NET/Framework64/v4.0.30319/csc.exe", fileName + ".cs");
                        compileInfo.CreateNoWindow = true;
                        compileInfo.UseShellExecute = false;
                        compileInfo.WorkingDirectory = filePath + "/" + fileName.ToLower();
                        compile = Process.Start(compileInfo);

                        compile.WaitForExit();//compilation process ends

                        //procInfo = new ProcessStartInfo(filePath + "/" + fileName.ToLower() + "/" + fileName + ".exe");
                    }
                    catch (Exception e)
                    {
                        pathToExecutable = "error";//signal that compilation error
                        break;
                    }
                    pathToExecutable = filePath + "\\" + fileName.ToLower() + "\\" + fileName + ".exe";
                    break;
                case "Python":
                    pathToExecutable = filePath + "\\" + fileName.ToLower() + "\\" + fileName + ".py";
                    break;
                default:
                    //should never reach default
                    pathToExecutable = "";
                    break;
            }
        }

        public string getExePath()
        {
            return pathToExecutable;
        }
    }//end of class
}