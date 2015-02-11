using System;
using System.Collections.Generic;
using System.Text;

namespace MarketClearingPrice
{
    /// <summary>
    /// A MarketClearingGraph keeps track of all the data structures that are needed to maintain a bipartite graph
    /// of buyers and products as well as a price vector.
    /// </summary>
    public class MarketClearingGraph
    {
        /// <summary>
        /// The number of buyers and products in the valuation matrix.
        /// </summary>
        private int numberOfBuyers;

        /// <summary>
        /// The maximum value each valuation can take.
        /// </summary>
        private int maxValuation;

        /// <summary>
        /// A square matrix that holds how much each buyer values each product.
        /// </summary>
        private int[,] originalValuationMatrix;

        /// <summary>
        /// The valuation matrix, adjusted for the price vector (the price of each product is subtracted from each buyer's valuation).
        /// </summary>
        private int[,] adjustedValuationMatrix;

        /// <summary>
        /// A vector containing the price of each product.
        /// </summary>
        private int[] priceVector;

        /// <summary>
        /// An adjacency list implementation of a bipartite graph. It is composed of an array of integer lists
        /// where each element of the array represents a buyer and each element of an integer list represents the products
        /// that the buyer values the most. A perfect matching in this graph means that an optimal price vector has been found
        /// such that each buyer gets the product they value the most.
        /// </summary>
        private List<int>[] bipartiteGraph;

        /// <summary>
        /// Creates and initializes a MarketClearingGraph with the given number of buyers and randomizes the valuationMatrix
        /// according to the given maximum value of each valuation
        /// </summary>
        /// <param name="numberOfBuyers">The number of buyers and products in the valuation matrix.</param>
        /// <param name="maxValuation">The maximum value each valuation can take.</param>
        public MarketClearingGraph(int numberOfBuyers, int maxValuation)
        {
            this.numberOfBuyers = numberOfBuyers;
            this.maxValuation = maxValuation;

            priceVector = new int[numberOfBuyers];
            originalValuationMatrix = new int[numberOfBuyers, numberOfBuyers];
            adjustedValuationMatrix = new int[numberOfBuyers, numberOfBuyers];
            bipartiteGraph = new List<int>[numberOfBuyers];

            Random r = new Random();
            for (int i = 0; i < numberOfBuyers; i++)
            {
                priceVector[i] = 0;
                bipartiteGraph[i] = new List<int>();
                for (int j = 0; j < numberOfBuyers; j++)
                {
                    originalValuationMatrix[i, j] = r.Next(0, maxValuation + 1);
                }
            }
        }

        /// <summary>
        /// Creates and initializes a MarketClearingGraph with the given valuation matrix. 
        /// </summary>
        /// <param name="valuationMatrix"> A square matrix that holds information on how much each buyer values each product.</param>
        public MarketClearingGraph(int[,] valuationMatrix)
        {
            this.numberOfBuyers = valuationMatrix.GetLength(0);
            priceVector = new int[this.numberOfBuyers];
            this.originalValuationMatrix = valuationMatrix;
            adjustedValuationMatrix = new int[numberOfBuyers, numberOfBuyers];
            bipartiteGraph = new List<int>[this.numberOfBuyers];
        }

        /// <summary>
        /// Runs the market clearing price algorithm on a given MarketClearingGraph to obtain a final price vector.
        /// </summary>
        public void MarketClearingAlgorithm()
        {
            while (true)
            {
                AdjustValuationMatrix();
                ClearBipartiteGraph();
                AddMaxValuesToBipartiteGraph();
                var violatedHallSet = HasViolatedHallSet();
                if (violatedHallSet.Count != 0)
                {
                    foreach (int set in violatedHallSet)
                    {
                        IncreasePrice(set);
                    }
                    NormalizePriceVector();
                }
                else
                {
                    break;
                }
            }
        }
        /// <summary>
        /// Clears the adjacency list representation of the bipartite graph.
        /// </summary>
        public void ClearBipartiteGraph()
        {
            for (int i = 0; i < numberOfBuyers; i++)
            {
                bipartiteGraph[i] = new List<int>();
            }
        }

        /// <summary>
        /// For each buyer, find the product(s) she values the most according to the valuation matrix and add those
        /// to the bipartite graph.
        /// </summary>
        public void AddMaxValuesToBipartiteGraph()
        {
            // for each buyer, find the product(s) she values most and add them to the adjacency list 
            for (int i = 0; i < numberOfBuyers; i++)
            {
                int maxPrice = 0;
                List<int> products = new List<int>();

                // do a first pass to find the maximum price, then a second pass to find all products that are valued that price
                for (int j = 0; j < numberOfBuyers; j++)
                {
                    if (maxPrice < adjustedValuationMatrix[i, j])
                    {
                        maxPrice = adjustedValuationMatrix[i, j];
                    }
                }
                for (int j = 0; j < numberOfBuyers; j++)
                {
                    if (adjustedValuationMatrix[i, j] == maxPrice)
                    {
                        bipartiteGraph[i].Add(j);
                    }
                }
            }
        }

        /// <summary>
        /// Check the bipartite graph to see if any of its elements violates Hall's Condition (that is, check to see if
        /// the number of neighbors of any set A is smaller than the cardinality of set A itself).
        /// </summary>
        /// <returns>Returns an empty set if Hall's Condition is not violated anywhere in the bipartite graph, else returns
        /// the set of products that violates Hall's Condition.</returns>
        public HashSet<int> HasViolatedHallSet()
        {
            // generate all possible permutations of the set of buyers
            List<int[]> permutations = GeneratePermutations();

            // check each permutation to see if it violates Hall's Condition
            foreach (int[] permutation in permutations)
            {
                Array.Sort(permutation);
                int arrayPointer = 0;

                // find all distinct neighbors of products represented in this permutation
                HashSet<int> neighbors = new HashSet<int>();
                for (int i = 0; i < numberOfBuyers; i++)
                {
                    if (arrayPointer < permutation.Length && i == permutation[arrayPointer])
                    {
                        arrayPointer = arrayPointer + 1;
                        foreach (int seller in bipartiteGraph[i])
                        {
                            neighbors.Add(seller);
                        }
                    }
                }
                int numberOfElements = permutation.Length;
                int numberOfNeighbors = neighbors.Count;
               
                if (numberOfNeighbors < numberOfElements)
                {
                    return neighbors;
                }
            }
            return new HashSet<int>();
        }

        /// <summary>
        /// Increase the price of the product that has violated Hall's Condition.
        /// </summary>
        /// <param name="violatedHallSet"></param>
        public void IncreasePrice(int violatedHallSet)
        {
            priceVector[violatedHallSet] = priceVector[violatedHallSet] + 1;
        }


        /// <summary>
        /// Normalize the price vector so that the lowest price is 0 and all other prices are decreased accordingly.
        /// </summary>
        public void NormalizePriceVector()
        {
            int lowestPrice = int.MaxValue;
            for (int i = 0; i < numberOfBuyers; i++)
            {
                if (priceVector[i] < lowestPrice)
                {
                    lowestPrice = priceVector[i];
                }
            }
            for (int i = 0; i < numberOfBuyers; i++)
            {
                priceVector[i] = priceVector[i] - lowestPrice;
            }
        }

        /// <summary>
        /// Adjust the valuation matrix accordingly by decrementing each buyers' valuation of the given product.
        /// </summary>
        public void AdjustValuationMatrix()
        {
            for (int i = 0; i < numberOfBuyers; i++)
            {
                for (int j = 0; j < numberOfBuyers; j++)
                {
                    adjustedValuationMatrix[i, j] = originalValuationMatrix[i, j] - priceVector[j];
                }
            }
        }

        /// <summary>
        /// Checks if the given price vector will produce a perfect matching.
        /// </summary>
        /// <param name="priceVector"></param>
        /// <returns></returns>
        public bool CheckValidPriceVector(int[] priceVector)
        {
            if (priceVector.Length != numberOfBuyers)
            {
                return false;
            }
            else
            {
                SetPriceVector(priceVector);
                AdjustValuationMatrix();
                ClearBipartiteGraph();
                AddMaxValuesToBipartiteGraph();
                if (HasViolatedHallSet().Count == 0)
                {
                    return true;
                }
                else
                {
                    return false;
                } 
            }
        }

        /// <summary>
        /// Gets the price vector.
        /// </summary>
        public int[] GetPriceVector()
        {
            return priceVector;
        }

        /// <summary>
        /// Sets the price vector.
        /// </summary>
        /// <param name="pv"></param>
        private void SetPriceVector(int[] pv)
        {
            priceVector = pv;
        }

        /// <summary>
        /// Generates all possible subsets of a sequential integer array.
        /// </summary>
        /// <returns>A list of integer arrays that represents all possible permutations of a given set.</returns>
        private List<int[]> GeneratePermutations()
        {
            List<int[]> subsets = new List<int[]>();

            int[] originalArray = new int[numberOfBuyers];
            for (int i = 0; i < numberOfBuyers; i++)
            {
                originalArray[i] = i;
            }

            // reference: http://stackoverflow.com/questions/2650629/finding-all-subsets-of-a-setsentence-using-c-sharp
            for (int i = 0; i < originalArray.Length; i++)
            {
                int subsetCount = subsets.Count;
                subsets.Add(new int[] { originalArray[i]});
                for (int j = 0; j < subsetCount; j++)
                {
                    int[] newSubset = new int[subsets[j].Length + 1];
                    subsets[j].CopyTo(newSubset, 0);
                    newSubset[newSubset.Length - 1] = originalArray[i];
                    subsets.Add(newSubset);
                }
            }
            return subsets;
        }

        #region Printing Methods
        /// <summary>
        /// Prints out the adjusted valuation matrix to console.
        /// </summary>
        public void PrintAdjustedValuationMatrix()
        {
            Console.WriteLine("Adjusted Valuation Matrix: ");
            for (int i = 0; i < numberOfBuyers; i++)
            {
                for (int j = 0; j < numberOfBuyers; j++)
                {
                    Console.Write(adjustedValuationMatrix[i, j] + " ");
                }
                Console.WriteLine("");
            }
            Console.WriteLine("");
        }

        /// <summary>
        /// Prints out the original valuation matrix to console.
        /// </summary>
        public void PrintOriginalValuationMatrix()
        {
            Console.WriteLine("Original Valuation Matrix: ");
            for (int i = 0; i < numberOfBuyers; i++)
            {
                for (int j = 0; j < numberOfBuyers; j++)
                {
                    Console.Write(originalValuationMatrix[i, j] + " ");
                }
                Console.WriteLine("");
            }
            Console.WriteLine("");
        }

        /// <summary>
        /// Prints out the price vector to console.
        /// </summary>
        public void PrintPriceVector()
        {
            Console.WriteLine("Price Vector: ");
            for (int i = 0; i < numberOfBuyers; i++)
            {
                Console.Write(priceVector[i] + " ");
            }
            Console.WriteLine("");
            Console.WriteLine("");
        }

        #endregion
    }
}
