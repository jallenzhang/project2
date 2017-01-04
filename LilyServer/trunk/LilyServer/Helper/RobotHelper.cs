using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DataPersist;

namespace LilyServer.Helper
{
    public static class RobotHelper
    {
        public static int CreateRobotCount()
        {
            int count = 0;
            Random random = new Random();
            double rate = random.NextDouble();
            double[] rates = new double[] { 0.0d, 0.05d, 0.1d, 0.3d, 0.6d, 0.9d, 0.93d };//,0.95d,1d
            for (int i = 0; i < rates.Length-1; i++)
            {
                count++;
                if (rates[i] <= rate && rate <= rates[i + 1])
                {
                    break;
                }
            }
            return count;
        }

        public static int CreateRobotCount(int playersCount)
        {
            int i = 0;
            switch (playersCount)
            {
                case 1:
                    i = RandomRange(3,5);
                    break;
                case 2:
                    i = RandomRange(2,4);
                    break;
                case 3:
                    i = RandomRange(2,3);
                    break;
                case 4:
                    i = RandomRange(1,3);
                    break;
                case 5:
                    i = RandomRange(1,2);
                    break;
                case 6:
                    i = RandomRange(0,2);
                    break;
                case 7:
                    i = RandomRange(0,1);
                    break;
                default:
                    i = 0;
                    break;
            }
            return i;
        }

        public static int MoveRobotCount(int playersCount) {
            int i = 0;
            switch (playersCount)
            {
                case 0:
                case 1:
                case 2:
                case 3:
                    i = 0;
                    break;
                case 4:
                case 5:
                    i = RandomRange(0,2);
                    break;               
                case 6:
                case 7:
                    i = RandomRange(1,2);
                    break;
                case 8:
                    i = RandomRange(2,3);
                    break;
                default:
                    i = RandomRange(2,4);
                    break;
            }
            return i;
        }

        public static bool ShouldLeaveTableByPlayerCount(int playerCount)
        {
            if (playerCount <= 2)
            {
                return false;
            }
            double rate = playerCount / 20;//1 - (1 / (playerCount - 2));
            Random random = new Random();
            return random.NextDouble() <= rate ? true : false;
        }

        public static int RandomRange(int min, int max)
        {
            Random r = new Random(Guid.NewGuid().GetHashCode());
            return r.Next(min, max);
        }

        public static int[] getLevel(double total_exp, double level1)
        {

            double level = level1;

            double needexp = Math.Floor(level * level / 3d * (level + 4d));

            if (total_exp < needexp)
            {
                return new int[] { (int)level, (int)total_exp };
            }

            total_exp = total_exp - needexp;
            level++;
            int[] retLevel = getLevel(total_exp, level);

            return retLevel;
        }
    }
	
}
