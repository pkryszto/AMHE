using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AMHE
{
    class Population
    {
        private static readonly int MAX_THREADS = 4;
        private static readonly Object semaphore = new object();

        public HerdMember bestMember;
        private static Random random = new Random();
        private int mi;
        private int lambda;
        public List<HerdMember> members;
        private int iterations;
        private double minValue;
        private double maxTime;
        private string selection;


        List<Vertice> vertices;
        double[,] distanceTable;
        double[,] capacityTable;
        int source;
        int target;

        public Population(Graph graph, int m, int l, int iter, double val, double time, string sel)
        {
            mi = m;
            lambda = l;
            iterations = iter;
            minValue = val;
            maxTime = time;

            distanceTable = graph.createDistanceTable();
            capacityTable = graph.createCapacityTable();
            source = graph.sourceNumber;
            target = graph.targetNumber;
            vertices = graph.vertices;
            selection = sel;
            HerdMember member = new HerdMember(graph.vertices, distanceTable, capacityTable, source, target);
            bestMember = member;
            members = new List<HerdMember>();
            members.Add(member);
        }

        public void firstGeneration()
        {
            for(int i = 1; i < mi; i++)
            {
                HerdMember member = members[0];
                createMember(member, members);
            }
            bestMember = chooseBest(members);
        }

        private void createMember(List<HerdMember> fromList, List<HerdMember> outList)
        {
            int number = random.Next(fromList.Count);
            HerdMember member = new HerdMember(vertices, distanceTable, capacityTable, fromList[number].solution.capacityPath, target);

            System.Threading.Monitor.Enter(semaphore);
            outList.Add(member);
            System.Threading.Monitor.Exit(semaphore);
        }

        private void createMember(HerdMember member, List<HerdMember> outList)
        {
            HerdMember toAdd = new HerdMember(vertices, distanceTable, capacityTable, member.solution.capacityPath, target);
            outList.Add(toAdd);
        }

        private void createMembers(List<HerdMember> fromList, List<HerdMember> outList, int elements)
        {
            for (int i = 0; i < elements; i++) createMember(fromList, outList);
        }

        private List<HerdMember> generateNewPopulationMultiThreads()
        {
            List<HerdMember> outList = new List<HerdMember>();
            int[] table = splitNumber(lambda);

            List<System.Threading.Thread> tasks = new List<System.Threading.Thread>();
            for (int i = 0; i < MAX_THREADS; i++)
            {
                System.Threading.Monitor.Enter(semaphore);
                int size = table[i];
                System.Threading.Thread thread = new System.Threading.Thread(delegate () {
                    createMembers(members, outList, size);
                });
                System.Threading.Monitor.Exit(semaphore);

                tasks.Add(thread);
                thread.Start();
            }

            foreach (var temp in tasks)
            {
                temp.Join();
            }

            return outList;
        }

        private List<HerdMember> generateNewPopulation()
        {
            List<HerdMember> outList = new List<HerdMember>();
            for (int i = 0; i < lambda; i++) createMember(members, outList);

            return outList;
        }

        private HerdMember chooseBest(List<HerdMember> list)
        {
            HerdMember toReturn = list.First();
            foreach (var v in list) if (v.solution.finalValue > toReturn.solution.finalValue) toReturn = v;
            return toReturn;
        }

        private List<HerdMember> mergePopulations(List<HerdMember> first, List<HerdMember> second)
        {
            List<HerdMember> toReturn = new List<HerdMember>();
            toReturn.AddRange(first);
            toReturn.AddRange(second);
            return toReturn;
        }

        private void rouletteSelection()
        {
            double sum = 0;
            foreach (var v in members) sum += v.solution.finalValue;
            List<HerdMember> newMembers = new List<HerdMember>();
            for(int i = 0; i < mi; i++)
            {
                HerdMember toAdd = rouletteOnePoint(sum);
                newMembers.Add(toAdd);
            }
            members = newMembers;
        }

        private HerdMember rouletteOnePoint(double sum)
        {
            double currentSum = 0;
            double prob = random.NextDouble();
            for(int i = 0; i < members.Count; i++)
            {
                currentSum = members[i].solution.finalValue / sum;
                if (prob <= currentSum) return members[i];
            }
            return members.Last();
        }

        private void tresholdSelection(int size)
        {
            List<HerdMember> newMembers = new List<HerdMember>();
            members = members.OrderByDescending(x => x.solution.finalValue).ToList();
            for(int i = 0; i < mi; i++)
            {
                newMembers.Add(members[random.Next(size)]);
            }
            members = newMembers;
        }

        private void duelSelection(int size)
        {
            List<HerdMember> newMembers = new List<HerdMember>();
            for (int i = 0; i < mi; i++) newMembers.Add(duel(size));
            members = newMembers;
        }

        private HerdMember duel(int size)
        {
            List<HerdMember> duelists = new List<HerdMember>();
            for(int i = 0; i < size; i++)
            {
                duelists.Add(members[random.Next(members.Count)]);
            }
            return chooseBest(duelists);
        }
        
        private void doSelection(int size = 2)
        {
            if (selection.Equals("duel")) duelSelection(size);
            else if (selection.Equals("treshold")) tresholdSelection(size);
            else rouletteSelection();
        }
        
        private void oneIteration()
        {
            List<HerdMember> newPopulation = generateNewPopulationMultiThreads();
            members = mergePopulations(members, newPopulation);
            HerdMember bestChild = chooseBest(newPopulation);
            if (bestChild.solution.finalValue > bestMember.solution.finalValue) bestMember = bestChild;
            doSelection(2);
        }

        public void runAlgorithm()
        {
            firstGeneration();
            for(int i = 1; i < iterations; i++)
            {
                oneIteration();
                if (bestMember.solution.finalValue >= minValue) return;
            }
        }
        
        private int[] splitNumber(int number)
        {
            int[] table = new int[MAX_THREADS];
            for(int i = 0; i < MAX_THREADS; i++) table[i] = number / MAX_THREADS;
            int newNumber = number % MAX_THREADS;
            for (int i = 0; i < newNumber; i++) table[i] = table[i] + 1;
            return table;
        }

    }
}
