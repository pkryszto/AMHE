using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AMHE
{
    class Dijkstra
    {
        private List<Vertice> vertices;
        private double[,] edges;
        private int source;
        private int target;
        private bool isDistance;

        public Dijkstra(List<Vertice> ver, double[,] edgs, int src, int trg, bool dist)
        {
            vertices = ver;
            edges = edgs;
            source = src;
            target = trg;
            isDistance = dist;
        }

        int minDistance(double[] dist,
                    bool[] sptSet)
        {
            double min = Double.MaxValue;
            int min_index = -1;

            for (int v = 0; v < vertices.Count; v++)
                if (sptSet[v] == false && dist[v] <= min)
                {
                    min = dist[v];
                    min_index = v;
                }

            return min_index;
        }
        
        public Path findPath()
        {
            int noConnection;
            if (isDistance) noConnection = 0;
            else noConnection = -1;

            double[] dist = new double[vertices.Count]; 
            int[] lastPoint = new int[vertices.Count]; 
            
            bool[] sptSet = new bool[vertices.Count];
            
            for (int i = 0; i < vertices.Count; i++)
            {
                dist[i] = int.MaxValue;
                sptSet[i] = false;
                lastPoint[i] = -1;
            }

            dist[source] = 0;
            
            for (int count = 0; count < vertices.Count - 1; count++)
            {
                int u = minDistance(dist, sptSet);
                
                sptSet[u] = true;
                
                for (int v = 0; v < vertices.Count; v++)
                {
                    if (!sptSet[v] && edges[u, v] != noConnection && dist[u] != int.MaxValue && dist[u] + edges[u, v] < dist[v])
                    {
                        dist[v] = dist[u] + edges[u, v];
                        lastPoint[v] = u;
                    }
                }

                if (u == target) break;
            }

            List<Vertice> path = getPath(lastPoint);

            if (isDistance) return new Path(path, dist[target], 0);
            else return new Path(path, 0, (-1) * dist[target]);
        }

        private List<Vertice> getPath(int[] points)
        {
            List<Vertice> toReturn = new List<Vertice>();
            int vertice = target;

            while(vertice != -1)
            {
                toReturn.Insert(0, vertices[vertice]);
                vertice = points[vertice];
            }

            return toReturn;
        }

    }
}
