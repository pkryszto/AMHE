using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AMHE
{
    class Solution
    {
        public static readonly double pathEpsilon = 1;
        public static readonly double capacityEpsilon = 1000;
        public Path distancePath;
        public Path capacityPath;
        public double finalValue;

        public Solution(Path dist, Path cap)
        {
            distancePath = dist;
            capacityPath = cap;
            finalValue = pathEpsilon / distancePath.length + capacityEpsilon * Math.Pow(Math.E, capacityPath.capacity);
        }

        public void printSolution()
        {
            Console.WriteLine("DISTANCE PATH:");
            distancePath.printPath();
            Console.WriteLine("CAPACITY PATH:");
            capacityPath.printPath();
            Console.WriteLine("FINAL VALUE: " + finalValue);
        }

    }
}
