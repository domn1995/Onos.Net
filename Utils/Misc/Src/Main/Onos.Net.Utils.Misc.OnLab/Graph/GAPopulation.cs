using System;
using System.Collections.Generic;

namespace Onos.Net.Utils.Misc.OnLab.Graph
{
    /// <summary>
    /// Represents a population of GAOrganisms. This class can be used to run
    /// a genetic algorithm on the population and return the fittest solutions.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class GAPopulation<T> : List<T> where T : IGAOrganism
    {
        private Random r = new Random();
        private IComparer<T> comparer = new GAComparer();

        /// <summary>
        /// Steps the population through one generation. The 75%
        /// least fit organisms are killed off and replaced with
        /// the children of the 25% and some "random" newcomers.
        /// </summary>
        public void Step()
        {
            Sort(comparer);
            int maxSize = Count;

            for (int i = Count - 1; i > maxSize / 4; --i)
            {
                RemoveAt(i);
            }

            foreach (T org in this)
            {
                // Get random number, either 0 (don't mutate) or 1 (mutate).
                if (r.Next(0, 2) == 1)
                {
                    org.Mutate();
                }
            }

            while (Count < maxSize * 4 / 5)
            {
                T org1 = this[r.Next(Count)];
                T org2 = this[r.Next(Count)];
                Add((T)org1.CrossWith(org2));
            }

            while (Count < maxSize)
            {
                T org1 = this[r.Next(Count)];
                Add((T)org1.GetRandom());
            }
        }

        /// <summary>
        /// Runs the genetic algorithm for the specified number of iterations,
        /// and returns a sample of the resulting population of solutions.
        /// </summary>
        /// <param name="generations">The number of generations to run the GA for.</param>
        /// <param name="populationSize">The population size of the GA.</param>
        /// <param name="sample">The number of solutions to ask for.</param>
        /// <param name="template">The template GAOrganism to seed the population with.</param>
        /// <returns>A new list containing sample number of organisms.</returns>
        public IList<T> RunGA(int generations, int populationSize, int sample, T template)
        {
            for (int i = 0; i < populationSize; ++i)
            {
                Add((T)template.GetRandom());
            }

            for (int i = 0; i < generations; ++i)
            {
                Step();
            }

            for (int i = Count - 1; i >= sample; --i)
            {
                RemoveAt(i);
            }

            return new List<T>(this);
        }

        private class GAComparer : IComparer<T>
        {
            public int Compare(T org1, T org2) => org1.Fitness.CompareTo(org2.Fitness);
        }
    }
}