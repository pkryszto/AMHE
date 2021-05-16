using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AMHE
{
    class Edge
    {
        public string name;
        public Vertice firstPoint;
        public Vertice secondPoint;
        public double capacity;
        public double length;

        public Edge(string nam, Vertice first, Vertice second)
        {
            name = nam;
            firstPoint = first;
            secondPoint = second;
            length = 1;
        }

        public void printEdge()
        {
            Console.WriteLine("Edge between " + firstPoint.name + " and " + secondPoint.name);
            Console.WriteLine("Length: " + length + " Capacity: " + capacity);
        }

        public double calculateCapacity()
        {
            return Math.Log(1 - capacity);
        }

        public void setCapacity(double cap)
        {
            capacity = cap;
        }
    }
}
