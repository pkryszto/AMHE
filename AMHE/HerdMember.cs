using System;
using System.Collections.Generic;

namespace AMHE
{
    class HerdMember
    {
        private static readonly double distancePenalty = 9999;
        private static readonly double capacityPenalty = Math.Log(0,000001);
        private static Random random = new Random();
        private List<Vertice> vertices;
        private double[,] distanceTable;
        private double[,] capacityTable;
        private int target;
        public Solution solution;

        public HerdMember(List<Vertice> ver, double[,] dist, double[,] cap, Path oldPath, int trg)
        {
            vertices = ver;
            target = trg;
            distanceTable = Graph.copyTable(dist, vertices.Count);
            capacityTable = Graph.copyTable(cap, vertices.Count);
            Path shortPath = drawNewPath(oldPath);
            Path distancePath = Path.mergePaths(shortPath, findBestPath(shortPath, distanceTable, true));
            Path capacityPath = findBestPath(distancePath, capacityTable, false);
            solution = new Solution(distancePath, capacityPath);
        }

        public HerdMember(List<Vertice> ver, double[,] dist, double[,] cap, int src, int trg)
        {
            vertices = ver;
            target = trg;
            distanceTable = Graph.copyTable(dist, vertices.Count);
            capacityTable = Graph.copyTable(cap, vertices.Count);

            Dijkstra dijkstra = new Dijkstra(vertices, distanceTable, src, target, true);
            Path distancePath = dijkstra.findPath();
            Path capacityPath = findBestPath(distancePath, capacityTable, false);
            solution = new Solution(distancePath, capacityPath);
        }


        private Path findBestPath(Path currentPath, double[,] table, bool isDistance)
        {
            modifyTable(table, currentPath, isDistance);
            int src;

            if (isDistance) src = vertices.FindIndex(x => x.name == currentPath.getLast().name);
            else src = vertices.FindIndex(x => x.name == currentPath.getFirst().name);

            Dijkstra dijkstra = new Dijkstra(vertices, table, src, target, isDistance);
            return dijkstra.findPath();
        }

        public Path drawNewPath(Path oldPath)
        {
            Tuple<int, int> newPoint = drawNewPoint(oldPath);
            List<Vertice> path = new List<Vertice>();
            Vertice temp = oldPath.getFirst();

            int i = 1;

            while(!temp.Equals(vertices[newPoint.Item1]))
            {
                path.Add(temp);
                temp = oldPath.points[i];
                i++;
            }

            path.Add(vertices[newPoint.Item1]);
            path.Add(vertices[newPoint.Item2]);

            Path toReturn = new Path(path);
            return toReturn;
        }

        private Tuple<int,int> drawNewPoint(Path oldPath)
        {
            int point;
            int position;
            int counter;
            while(true)
            {
                point = random.Next(oldPath.points.Count-2);
                position = vertices.FindIndex(x => x.name == oldPath.points[point].name);
                counter = 0;
                for (int i = 0; i < vertices.Count; i++) if (distanceTable[position, i] != 0) counter++;
                if (counter > 2) break;
            }

            int[] drawTable; 

            int next = vertices.FindIndex(x => x.name == oldPath.points[point + 1].name);
            int prior;
            if (point == 0)
            {
                drawTable = new int[counter - 1];
                prior = next;
            }
            else
            {
                drawTable = new int[counter - 2];
                prior = vertices.FindIndex(x => x.name == oldPath.points[point - 1].name);
            }
            
            int j = 0;

            for(int i = 0; i < vertices.Count; i++)
            {
                if(distanceTable[position, i] != 0 && i != prior && i != next)
                {
                    drawTable[j] = i;
                    j++;
                }
            }

            Tuple<int, int> toReturn = new Tuple<int, int>(position, drawTable[random.Next(counter - 2)]);
            return toReturn;

        }

        private void modifyTable(double[,] table, Path path, bool isDistance)
        {
            double penalty;
            if (isDistance) penalty = distancePenalty;
            else penalty = capacityPenalty;

            for(int i = 0; i < path.points.Count-1; i++)
            {
                int a = vertices.FindIndex(x => x.name == path.points[i].name);
                int b = vertices.FindIndex(x => x.name == path.points[i+1].name);
                table[a, b] = penalty;
                table[b, a] = penalty;
            }
        }

    }
}
