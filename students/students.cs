using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace students
{
    internal class students
    {
        public string name;
        public int age;
        public string studentId;
        public double score;

        public void displayInfo()
        {
            Console.WriteLine("Name: " + name);
            Console.WriteLine("Age: " + age);
            Console.WriteLine("Student ID: " + studentId);
            Console.WriteLine("Score: " + score);

        }
        public void CheckStatus()
        {
            if (score >= 75)
            {
                Console.WriteLine("Status: Passed");
            }
            else
            {
                Console.WriteLine("Status: Failed");
            }
        }
    }
}
