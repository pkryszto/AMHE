using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AMHE
{
    class Vertice
    {
        public String name;
        List<Edge> edges;

        public Vertice(String n)
        {
            name = n;
            edges = new List<Edge>();
        }

        public void addEdge(Edge edge)
        {
            edges.Add(edge);
        }

        public void printPoint()
        {
            Console.WriteLine("Point name:" + name);
            Console.WriteLine("Connections:");
            foreach (var edge in edges) edge.printEdge();
        }

    }
}
