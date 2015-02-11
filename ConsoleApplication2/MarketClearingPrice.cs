using System;
using System.Collections.Generic;
using System.Text;

namespace MarketClearingPrice
{
    /// <summary>
    /// Given a set of n buyers who want to each buy one of a set of n products, a market clearing price
    /// is a price assigned to each one of the products so that each buyer experiences equal satisfaction with
    /// the value of her purchase (i.e. maximize the difference between price paid and the value of the product
    /// for each buyer). The market clearing price is found by constructing a bipartite graph between the buyers
    /// and the products and finding a price vector that allows for a maximum matching.
    /// </summary>
    public class MarketClearingPrice
    {
        static void Main(string[] args)
        {
            // Example 1: randomized valuation matrix
            MarketClearingGraph mcg0 = new MarketClearingGraph(4, 8);
            Console.WriteLine("Randomized Matrix: ");
            mcg0.MarketClearingAlgorithm();
            mcg0.PrintOriginalValuationMatrix();
            mcg0.PrintAdjustedValuationMatrix();
            mcg0.PrintPriceVector();
            Console.WriteLine("Is the calcualted price vector valid?");
            Console.WriteLine(mcg0.CheckValidPriceVector(mcg0.GetPriceVector()));
            Console.WriteLine("-------");

            // Example 2: sample 3x3 valuation matrix
            int[,] vm = InitializeMatrix1();
            MarketClearingGraph mcg1 = new MarketClearingGraph(vm);
            Console.WriteLine("Sample 3 x 3 Valuation Matrix: ");
            mcg1.MarketClearingAlgorithm();
            mcg1.PrintOriginalValuationMatrix();
            mcg1.PrintAdjustedValuationMatrix();
            mcg1.PrintPriceVector();
            Console.WriteLine("Is the calculated price vector valid?");
            Console.WriteLine(mcg1.CheckValidPriceVector(mcg1.GetPriceVector()));
            Console.WriteLine("-------");           

            // Example 2: sample 5x5 valuation matrix
            vm = InitializeMatrix2();
            MarketClearingGraph mcg2 = new MarketClearingGraph(vm);
            Console.WriteLine("Sample 5 x 5 Valuation Matrix: ");
            mcg2.MarketClearingAlgorithm();
            mcg2.PrintOriginalValuationMatrix();
            mcg2.PrintAdjustedValuationMatrix();
            mcg2.PrintPriceVector();

            Console.WriteLine("Is the calculated price vector valid?");
            Console.WriteLine(mcg2.CheckValidPriceVector(mcg2.GetPriceVector()));
            Console.WriteLine("-------");

            Console.ReadLine();
        }

        static int[,] InitializeMatrix1()
        {
            int[,] vm = new int[3, 3];
            vm[0, 0] = 6;
            vm[0, 1] = 5;
            vm[0, 2] = 2;

            vm[1, 0] = 7;
            vm[1, 1] = 6;
            vm[1, 2] = 3;

            vm[2, 0] = 6;
            vm[2, 1] = 7;
            vm[2, 2] = 6;

            return vm;
        }

        static int[,] InitializeMatrix2()
        {
            int[,] vm = new int[5, 5];
            vm[0, 0] = 5;
            vm[0, 1] = 4;
            vm[0, 2] = 2;
            vm[0, 3] = 0;
            vm[0, 4] = 0;

            vm[1, 0] = 3;
            vm[1, 1] = 5;
            vm[1, 2] = 4;
            vm[1, 3] = 3;
            vm[1, 4] = 0;

            vm[2, 0] = 6;
            vm[2, 1] = 1;
            vm[2, 2] = 2;
            vm[2, 3] = 3;
            vm[2, 4] = 0;

            vm[3, 0] = 1;
            vm[3, 1] = 7;
            vm[3, 2] = 8;
            vm[3, 3] = 3;
            vm[3, 4] = 0;

            vm[4, 0] = 1;
            vm[4, 1] = 2;
            vm[4, 2] = 0;
            vm[4, 3] = 3;
            vm[4, 4] = 0;

            return vm;
        }

    }
}
