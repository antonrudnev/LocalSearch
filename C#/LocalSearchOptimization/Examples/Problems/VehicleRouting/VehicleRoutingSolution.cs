using System;
using System.Collections.Generic;
using System.Linq;
using LocalSearchOptimization.Components;
using LocalSearchOptimization.Examples.Structures.Permutation;
using System.Text;

namespace LocalSearchOptimization.Examples.Problems.VehicleRouting
{
    public class VehicleRoutingSolution : IPermutation
    {
        private VehicleRoutingProblem vehicleRoutingProblem;
        private double[] transportationCost;

        public double CostValue { get; private set; }
        public int IterationNumber { get; set; }
        public double TimeInSeconds { get; set; }
        public bool IsCurrentBest { get; set; }
        public bool IsFinal { get; set; }

        public List<int> Order { get; }

        public string InstanceTag { get; set; }

        public string OperatorTag { get; }

        public double[] X { get; }
        public double[] Y { get; }

        public double MaxWidth { get; set; }
        public double MaxHeight { get; set; }

        public int NumberOfCustomers { get => vehicleRoutingProblem.NumberOfCustomers; }
        public int NumberOfVehicles { get => vehicleRoutingProblem.NumberOfVehicles; }
        public int VehicleCapacity { get => vehicleRoutingProblem.VehicleCapacity; }

        public double LowerBoundGap { get => (CostValue / vehicleRoutingProblem.LowerBound - 1) * 100; }

        public VehicleRoutingSolution(VehicleRoutingProblem vehicleRoutingProblem) : this(vehicleRoutingProblem, Enumerable.Range(0, vehicleRoutingProblem.NumberOfCustomers).ToList(), "init")
        {

        }

        private VehicleRoutingSolution(VehicleRoutingProblem vehicleRoutingProblem, List<int> order, string operatorName)
        {
            this.vehicleRoutingProblem = vehicleRoutingProblem;
            X = vehicleRoutingProblem.X;
            Y = vehicleRoutingProblem.Y;
            MaxWidth = vehicleRoutingProblem.MaxX;
            MaxHeight = vehicleRoutingProblem.MaxY;
            Order = order;
            OperatorTag = operatorName;
            transportationCost = new double[vehicleRoutingProblem.NumberOfVehicles];
            DecodeSolution(vehicleRoutingProblem);
        }

        public IPermutation FetchPermutation(List<int> order, string operatorName)
        {
            return new VehicleRoutingSolution(vehicleRoutingProblem, order, operatorName);
        }

        public ISolution Shuffle(int seed)
        {
            Random random = new Random(seed);
            return new VehicleRoutingSolution(vehicleRoutingProblem, Order.OrderBy(x => random.Next()).ToList(), "shuffle");
        }

        public ISolution Transcode()
        {
            return this;
        }

        private void DecodeSolution(VehicleRoutingProblem vehicleRoutingProblem)
        {
            for (int i = 0; i < NumberOfVehicles; i++)
            {
                int currentCustomer = Order[Math.Min((i + 1) * VehicleCapacity, NumberOfCustomers) - 1];
                int nextCustomer = Order[i * VehicleCapacity];
                double cost = vehicleRoutingProblem.Distance[currentCustomer, nextCustomer];
                for (int j = i * VehicleCapacity; j < Math.Min((i + 1) * VehicleCapacity, NumberOfCustomers) - 1; j++)
                {
                    currentCustomer = Order[j];
                    nextCustomer = Order[j + 1];
                    cost += vehicleRoutingProblem.Distance[currentCustomer, nextCustomer];
                }
                transportationCost[i] = cost;
            }
            CostValue = transportationCost.Sum();
        }

        public override string ToString()
        {
            StringBuilder builder = new StringBuilder();
            for (int i = 0; i < NumberOfVehicles; i++)
            {
                builder.Append(i).Append(": ");
                for (int j = i * VehicleCapacity; j < Math.Min((i + 1) * VehicleCapacity, NumberOfCustomers); j++)
                {
                    builder.Append(j);
                    if (j < Math.Min((i + 1) * VehicleCapacity, NumberOfCustomers) - 1)
                    {
                        builder.Append("->");
                    }
                }
                if (i < NumberOfVehicles - 1)
                {
                    builder.AppendLine();
                }
            }
            return builder.ToString();
        }
    }
}