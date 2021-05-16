using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AMHE
{
    class Globals
    {
        public static Stopwatch stopWatch = new Stopwatch();
        public static TimeSpan ts = new TimeSpan();
        public static string elapsedTime;
        public static void printElapsedTimeResetWatch(TimeSpan ts, string activity)
        {
            Globals.elapsedTime = String.Format("{0:00}:{1:00}:{2:00}", ts.Hours, ts.Minutes, ts.Seconds);
            Console.WriteLine(activity + " - finished in time: " + Globals.elapsedTime + " (hours:minutes:seconds)");
            Globals.stopWatch.Reset();
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            string edgePath = "C://Users//user//Desktop//dane//przykladowe_dane//gen_50_d_2.xml";
            string parametersPath = "C://Users//user//Desktop//dane//przykladowe_dane//gen_50_d_2_0_4_1.txt";

            Graph graph = new Graph();
            Globals.stopWatch.Start();
            graph.readEdges(edgePath);
            Globals.stopWatch.Stop();
            Globals.printElapsedTimeResetWatch(Globals.stopWatch.Elapsed, "Loaded edges");

            Globals.stopWatch.Start();
            graph.readParameters(parametersPath);
            Globals.stopWatch.Stop();
            Globals.printElapsedTimeResetWatch(Globals.stopWatch.Elapsed, "Loaded parameters");


            Globals.stopWatch.Start();
            double[,] distanceTable = graph.createDistanceTable();
            double[,] capacityTable = graph.createCapacityTable();
            Globals.stopWatch.Stop();
            Globals.printElapsedTimeResetWatch(Globals.stopWatch.Elapsed, "Created distance and capacity tables");


            Globals.stopWatch.Start();
            HerdMember member = new HerdMember(graph.vertices, distanceTable, capacityTable, graph.sourceNumber, graph.targetNumber);
            Globals.stopWatch.Stop();
            Globals.printElapsedTimeResetWatch(Globals.stopWatch.Elapsed, "Found first solution");
            member.solution.printSolution();

            Globals.stopWatch.Start();
            Population population = new Population(graph, 10, 15, 1000, 1000000000, 10, "roulette");
            population.runAlgorithm();
            Globals.stopWatch.Stop();
            Globals.printElapsedTimeResetWatch(Globals.stopWatch.Elapsed, "Finished algorithm");
            population.bestMember.solution.printSolution();
            
            Console.ReadKey();
        }
    }
}
