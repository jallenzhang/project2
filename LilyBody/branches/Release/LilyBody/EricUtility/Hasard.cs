using System;
using System.Collections.Generic;
using System.Text;

namespace EricUtility
{
    /// <summary>
    /// Classe permettant de générer certaines valeurs de manière aléatoire
    /// </summary>
    public static class Hasard
    {
        // Objet utilisé pour le random
        private static Random m_Random = new Random();

        /// <summary>
        /// Calcul un nombre aléatoire entre 0 et Max
        /// </summary>
        /// <param name="max">Maximum que peut atteindre le random</param>
        /// <returns>Retourne un nombre aléatoire entre 0 et Max, ou bien "max" si max n'est pas plus grand que 0</returns>
        public static int RandomWithMax(int max)
        {
            return max > 0 ? m_Random.Next(max + 1) : max;
        }

        /// <summary>
        /// Calcul un nombre aléatoire entre Min et 0
        /// </summary>
        /// <param name="max">Minimum que peut atteindre le random (negatif)</param>
        /// <returns>Retourne un nombre aléatoire entre Min et 0, ou bien Min si min n'est pas plus petit que 0</returns>
        public static int RandomWithMin(int min)
        {
            return min < 0 ? -m_Random.Next(-min + 1) : min;
        }

        /// <summary>
        /// Calcul un nombre aléatoire entre StartVal et StartVal+etendue
        /// </summary>
        /// <param name="startVal">Valeur de départ, l'une des bornes du random</param>
        /// <param name="etendue">Écart positif ou négatif entre les 2 bornes du random</param>
        /// <returns>Retourne un nombre aléatoire entre Min et Max</returns>
        public static int RandomWithLength(int startVal, int etendue)
        {
            if (etendue == 0)
                return etendue;
            else
                return startVal + etendue > 0 ? m_Random.Next(etendue) : -m_Random.Next(-etendue);
        }

        /// <summary>
        /// Calcul un nombre aléatoire entre Min et Max
        /// </summary>
        /// <param name="min">Borne inférieure du random</param>
        /// <param name="max">Borne supérieure du random</param>
        /// <returns>Retourne un nombre aléatoire entre Min et Max</returns>
        public static int RandomMinMax(int min, int max)
        {
            if (min == max)
                return min;
            int theMin = Math.Min(min, max);
            int theMax = Math.Max(min, max);
            return min + m_Random.Next((max - min) + 1);
        }

        /// <summary>
        /// Calcul une valeur aléatoire entre true et false
        /// </summary>
        /// <returns>Retourne aléatoirement true ou false</returns>
        public static bool RandomBool()
        {
            return m_Random.Next(2) == 0;
        }
    }
}
