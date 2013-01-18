using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace StructDistribution
{
    class StructDistribution
    {
        static int Main(string[] args)
        {
            A a1 = new A();
            B b1 = new B();

            a1.aM = 10111;
            b1.bM = 1010111;

            Console.WriteLine("a1 a: " + a1.aM);
            Console.WriteLine("b1 b: " + b1.bM);

            return 0;
        }
    }
}
