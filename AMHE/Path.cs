using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AMHE
{
    class Path
    {
        public List<Vertice> points;
        public double length = 0;
        public double capacity = 0;

        public Path()
        {
            points = new List<Vertice>();
        }

        public Path(List<Vertice> pts)
        {
            points = new List<Vertice>();
            foreach (var v in pts) points.Add(v);
            length = pts.Count - 1;
        }

        public Path(List<Vertice> pts, Vertice newPoint)
        {
            points = new List<Vertice>();
            foreach (var v in pts)
            {
                points.Add(v);
            }

            points.Add(newPoint);
        }

        public Path(List<Vertice> pts, double len, double cap)
        {
            points = pts;
            length = len;
            capacity = cap;
        }

        public void addPoint(Vertice point)
        {
            points.Add(point);
        }
        
        public Vertice getFirst()
        {
            return points.First();
        }

        public Vertice getLast()
        {
            return points.Last();
        }

        public static Path mergePaths(Path first, Path second)
        {
            List<Vertice> points = new List<Vertice>();
            points.AddRange(first.points);
            points.AddRange(second.points.GetRange(1, second.points.Count - 1));
            return new Path(points, first.length + second.length, first.capacity + second.capacity);
        }

        public void printPath()
        {
            for (int i = 0; i < points.Count - 1; i++) Console.Write(points[i].name + "->");
            Console.WriteLine(points.Last().name);
            Console.WriteLine("LENGTH: " + length + " CAPACITY: " + capacity);
        }

    }
}
