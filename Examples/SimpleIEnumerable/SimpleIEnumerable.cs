using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SimpleIEnumerable
{
    class Program
    {
        static void Main(string[] args)
        {
            DataContainer dc = new DataContainer();
            dc.InsertData(1, 2, 3);
            dc.InsertData(4, 5, 6);
            dc.InsertData(7, 8, 9);

            var result = dc.EnXEvenNumber();

            foreach (Scontainer scontainer in result) {
                Console.WriteLine(scontainer.X);
                Console.WriteLine(scontainer.Y);
                Console.WriteLine(scontainer.Z);
            }
        }
    }
}
