using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSharpDeductionMachine
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Enter number 1: ");
            int i = Int32.Parse(Console.ReadLine());
            Console.WriteLine("Enter number 1: ");
            int j = Int32.Parse(Console.ReadLine());
            Console.WriteLine("Deduction of both number is: " + (i - j));
        }
    }
}
