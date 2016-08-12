using System.Collections.Generic;
using System.Xml;

namespace SPade.ViewModels.Lecturer
{
    public class ViewTestCaseViewModel
    {
        public List<TestCase> testcases { get; set; }
        //public XmlDocument testCaseFile { get; set; }
    }

    public class TestCase
    {
        public List<string> inputs { get; set; }
    }
}