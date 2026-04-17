using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace students
{
    internal class Program
    {
        static void Main(string[] args)
        {
            students s1 = new students();
            s1.name = "Andi";
            s1.age = 20;
            s1.studentId = "S001";
            s1.score = 85;
            s1.displayInfo();
            s1.CheckStatus();
            Console.WriteLine();

            students s2 = new students();
            s2.name = "Budi";
            s2.age = 21;
            s2.studentId = "S002";
            s2.score = 60;

            s2.displayInfo();
            s2.CheckStatus();

        }
    }
}
