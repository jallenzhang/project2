using System;
using System.Collections.Generic;
using System.Text;

namespace EricUtility
{
    public class ArrayUtility
    {
        /// <summary>
        /// Choisis un élément dans la liste parmis tous les minimums !!
        /// </summary>
        /// <typeparam name="T">Le type d'objet du Array</typeparam>
        /// <param name="values">L'array</param>
        /// <returns>La position de la valeur choisie dans le array</returns>
        public static int GetRandomMin<T>(T[] values) where T : IComparable<T>
        {
            T min = values[0];
            List<int> possibles = new List<int>() { 0 };
            for (int i = 1; i < 16; ++i)
            {
                if (values[i].CompareTo(min) < 0)
                {
                    min = values[i];
                    possibles.Clear();
                }

                if (values[i].CompareTo(min) == 0)
                    possibles.Add(i);
            }
            return possibles[Hasard.RandomWithMax(possibles.Count - 1)];
        }
        /// <summary>
        /// Choisis un élément dans la liste parmis tous les maximums
        /// </summary>
        /// <typeparam name="T">Le type d'objet du Array</typeparam>
        /// <param name="values">L'array</param>
        /// <returns>La position de la valeur choisie dans le array</returns>
        public static int GetRandomMax<T>(T[] values) where T : IComparable<T>
        {
            T max = values[0];
            List<int> possibles = new List<int>() { 0 };
            for (int i = 1; i < 16; ++i)
            {
                if (values[i].CompareTo(max) > 0)
                {
                    max = values[i];
                    possibles.Clear();
                }

                if (values[i].CompareTo(max) == 0)
                    possibles.Add(i);
            }
            return possibles[Hasard.RandomWithMax(possibles.Count - 1)];
        }
    }
}
