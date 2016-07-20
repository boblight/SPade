using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSharpSumMachine
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Welcome to sum machine.\nPlease enter 1st number: ");
            int i = Int32.Parse(Console.ReadLine());
            Console.WriteLine("Please enter 2nd number: ");
            int j = Int32.Parse(Console.ReadLine());
            Console.WriteLine("Sum of 2 numbers is: " + (i + j));
            Console.ReadKey();
        }//end of main method
    }
}
