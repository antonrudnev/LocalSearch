using System;
using System.IO;
using System.Linq;

namespace LocalSearchOptimization.Examples.Problems.VehicleRouting
{
    public class VehicleRoutingProblem
    {
        public double[] X { get; }
        public double[] Y { get; }
        public double[,] Distance { get; private set; }

        public double MinX { get; set; }
        public double MinY { get; set; }
        public double MaxX { get; set; }
        public double MaxY { get; set; }

        public int Dimension { get => NumberOfCustomers; }

        public int NumberOfCustomers { get; }
        public int NumberOfVehicles { get; }
        public int VehicleCapacity { get; private set; }

        public double LowerBound { get; private set; }

        public VehicleRoutingProblem(int numberOfCustomers, int numberOfVehicles, int seed = 3)
        {
            NumberOfCustomers = numberOfCustomers;
            NumberOfVehicles = numberOfVehicles;
            X = new double[NumberOfCustomers];
            Y = new double[NumberOfCustomers];
            Random random = new Random(seed);
            for (int i = 0; i < NumberOfCustomers; i++)
            {
                X[i] = random.NextDouble();
                Y[i] = random.NextDouble();
            }
            CalculateMetrics();
        }

        public VehicleRoutingProblem(int numberOfCustomers, int numberOfVehicles, double[] x, double[] y)
        {
            if (x.Length != numberOfCustomers || y.Length != numberOfCustomers) throw new ArgumentException("Arrays x and y don't match to the number of customers.");
            NumberOfCustomers = numberOfCustomers;
            NumberOfVehicles = numberOfVehicles;
            X = x;
            Y = y;
            CalculateMetrics();
        }

        private void CalculateMetrics()
        {
            Distance = new double[NumberOfCustomers, NumberOfCustomers];
            for (int i = 0; i < NumberOfCustomers - 1; i++)
                for (int j = i + 1; j < NumberOfCustomers; j++)
                {
                    Distance[i, j] = Math.Sqrt(Math.Pow(X[i] - X[j], 2) + Math.Pow(Y[i] - Y[j], 2));
                    Distance[j, i] = Distance[i, j];
                }

            MinX = X.Min();
            MinY = Y.Min();
            MaxX = X.Max();
            MaxY = Y.Max();

            VehicleCapacity = (int)Math.Ceiling((double)NumberOfCustomers / NumberOfVehicles);

            double lb1 = 0.7080 * Math.Sqrt(NumberOfCustomers) + 0.522;
            double lb2 = 0.7078 * Math.Sqrt(NumberOfCustomers) + 0.551;
            LowerBound = Math.Max(lb1, lb2);
        }

        public void Save(string fileName)
        {
            using (StreamWriter file = new StreamWriter(File.Create(fileName)))
            {
                file.WriteLine(NumberOfCustomers);
                file.WriteLine(NumberOfVehicles);
                for (int i = 0; i < NumberOfCustomers; i++)
                {
                    file.WriteLine("{0} {1}", X[i], Y[i]);
                }
            }
        }

        public static VehicleRoutingProblem Load(string fileName)
        {
            using (StreamReader file = new StreamReader(File.Open(fileName, FileMode.Open)))
            {
                int numberOfCustomers = int.Parse(file.ReadLine());
                int numberOfVehicles = int.Parse(file.ReadLine());
                double[] x = new double[numberOfCustomers];
                double[] y = new double[numberOfCustomers];
                for (int i = 0; i < numberOfCustomers; i++)
                {
                    string[] xy = file.ReadLine().Split(' ');
                    x[i] = double.Parse(xy[0]);
                    y[i] = double.Parse(xy[1]);
                }
                return new VehicleRoutingProblem(numberOfCustomers, numberOfVehicles, x, y);
            }
        }
    }
}
