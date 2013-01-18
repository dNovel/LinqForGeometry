using System;
using System.Collections.Generic;
using System.Linq;

namespace SimpleLinq
{
    class BirthdayLinq
    {
        static int Main(string[] args)
        {
            // Creating a integer typed list
            List<int> IntList = new List<int>();

            // Filling a list container with data
            IntList.Add(10111);
            IntList.Add(1);
            IntList.Add(0);
            IntList.Add(-203);
            IntList.Add(10011);
            IntList.Add(1010111);

            /** This is the Linq Query
             *  Basicly it means:
             *  'Select all values from the container where the value is bigger than 0'
             *  Nothing has been executed until now
             */
            var intListQry = from val in IntList where val > 0 select val;

            Console.Write("Birthday: ");

            /**
             * Let's execute the query now against the container
             * Have a look at the output
             * The two values that are smaller than 0 are not selected
             */
            foreach (int value in intListQry)
            {
                Console.Write(value + " ");
            }

            Console.WriteLine("That's it.");
            return 0;
        }
    }
}
