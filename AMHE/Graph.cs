using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;

namespace AMHE
{
    class Graph
    {
        public List<Vertice> vertices;
        public List<Edge> edges;
        public double capacity;
        public string source;
        public string target;
        public int sourceNumber;
        public int targetNumber;
        double Lx;
        double Ly;

        public Graph()
        {
            vertices = new List<Vertice>();
            edges = new List<Edge>();
        }

        private Vertice findVertice(string name)
        {
            foreach(var v in vertices)
            {
                if (v.name.Equals(name)) return v;
            }
            return null;
        }

        private Vertice addVertice(string name)
        {
            Vertice toAdd = new Vertice(name);
            vertices.Add(toAdd);
            return toAdd;
        }

        private Edge findEdge(string name)
        {
            foreach (var v in edges)
            {
                if (v.name.Equals(name)) return v;
            }
            return null;
        }

        private void addEdge(string name, string v1, string v2)
        {
            Vertice first = findVertice(v1);
            if (first == null) first = addVertice(v1);
            Vertice second = findVertice(v2);
            if (second == null) second = addVertice(v2);

            Edge firstEdge = new Edge(name, first, second);
            edges.Add(firstEdge);

            first.addEdge(firstEdge);
        }

        public void readEdges(string path)
        {
            string edgeName;
            string v1;
            string v2;

            XmlDocument doc = new XmlDocument();
            doc.Load(path);
            XmlNodeList nodes = doc.SelectNodes("network/networkStructure/links/link");
            foreach(XmlNode node in nodes)
            {
                edgeName = node.Attributes[0].Value;
                v1 = node.ChildNodes[0].InnerText;
                v2 = node.ChildNodes[1].InnerText;
                addEdge(edgeName, v1, v2);
            }
        }

        public void readParameters(string path)
        {
            try
            {
                using (var sr = new StreamReader(path))
                {
                    //Skip current load
                    sr.ReadLine();
                    //Read capacity
                    var line = sr.ReadLine();
                    capacity = Double.Parse(line.Split(',')[1]);

                    //Read traffic on edges
                    while(true)
                    {
                        line = sr.ReadLine();
                        if (line.Contains("source,")) break;

                        var edgeParams = line.Split(',');
                        Edge edge = findEdge(edgeParams[0]);
                        edge.setCapacity(Double.Parse(edgeParams[1]));
                    }

                    //Read source
                    source = line.Split(',')[1];
                    sourceNumber = getIndexOfVertice(source);
                    //Read target
                    line = sr.ReadLine();
                    target = line.Split(',')[1];
                    targetNumber = getIndexOfVertice(target);

                    //Skip value and S
                    sr.ReadLine();
                    sr.ReadLine();

                    //Load Lx and Ly
                    line = sr.ReadLine();
                    Lx = Double.Parse(line.Split(',')[1].Replace('.', ','));
                    line = sr.ReadLine();
                    Ly = Double.Parse(line.Split(',')[1].Replace('.', ','));
                }
            }
            catch (IOException e)
            {
                Console.WriteLine("The file could not be read:");
                Console.WriteLine(e.Message);
            }
        }

        public void printGraph()
        {
            Console.WriteLine("Source: " + source);
            Console.WriteLine("Target: " + target);
            Console.WriteLine("Capacity: " + capacity);
            Console.WriteLine("Lx: " + Lx);
            Console.WriteLine("Ly: " + Ly);
            Console.WriteLine("Vertices:");
            foreach (var v in vertices) v.printPoint();
        }

        public int getIndexOfVertice(string name)
        {
            return vertices.FindIndex(x => x.name == name);
        }

        public double[,] createDistanceTable()
        {
            double[,] table = new double[vertices.Count, vertices.Count];

            foreach (Edge v in edges)
            {
                if (v.capacity / capacity > Lx) continue;
                int x = getIndexOfVertice(v.firstPoint.name);
                int y = getIndexOfVertice(v.secondPoint.name);
                table[x, y] = v.length;
                table[y, x] = v.length;
            }

            return table;
        }

        public double[,] createCapacityTable()
        {
            double[,] table = new double[vertices.Count, vertices.Count];
            
            for(int i = 0; i < vertices.Count; i++)
            {
                for(int j = 0; j < vertices.Count; j++)
                {
                    table[i, j] = -1;
                }
            }

            foreach (Edge v in edges)
            {
                if (v.capacity / capacity > Ly) continue;
                int x = getIndexOfVertice(v.firstPoint.name);
                int y = getIndexOfVertice(v.secondPoint.name);
                table[x, y] = (-1)*calculateCapacity(v.capacity);
                table[y, x] = (-1) * calculateCapacity(v.capacity);
            }

            return table;
        }

        private double calculateCapacity(double val)
        {
            return Math.Log(1 - val / capacity);
        }

        public static void printTable(double[,] table, int size)
        {
            for(int i = 0; i < size; i++)
            {
                for (int j = 0; j < size; j++) Console.Write(table[i, j] + " ");
                Console.WriteLine();
            }   
        }

        public static double[,] copyTable(double[,] table, int size)
        {
            double[,] newTable = new double[size,size];
            for (int i = 0; i < size; i++)
            {
                for (int j = 0; j < size; j++) newTable[i, j] = table[i, j];
            }

            return newTable;
        }
    }
}
