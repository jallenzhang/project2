using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebNotifications.Helper
{
    public class LevelExpHelper
    {
        /// <summary>
        /// 根据经验算等级
        /// </summary>
        /// <param name="total_exp"></param>
        /// <returns></returns>
        public static int GetJustLevel(double total_exp)
        {
            int[] res = GetLevel(total_exp, 1);
            return res[0];
        }

        /// <summary>
        /// 根据经验算等级
        /// </summary>
        /// <param name="total_exp"></param>
        /// <param name="level1">1</param>
        /// <returns></returns>
        public static int[] GetLevel(double total_exp, double level1)
        {
            double level = level1;

            double needexp = Math.Floor(level * level / 3d * (level + 4d));

            if (total_exp < needexp)
            {
                return new int[] { (int)level, (int)total_exp };
            }

            total_exp = total_exp - needexp;
            level++;
            int[] retLevel = GetLevel(total_exp, level);

            return retLevel;
        }
    }
}