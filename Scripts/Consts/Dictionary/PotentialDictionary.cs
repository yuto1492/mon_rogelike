using System.Collections.Generic;

namespace Consts.Dictionary
{
    public class PotentialDictionary
    {
        public static Dictionary<int, int> Hp = new Dictionary<int, int>()
        {
            {1, 10},
            {2, 15},
            {3, 20},
            {4, 25},
            {5, 30},
            {6, 35},
            {7, 40},
            {8, 45},
            {9, 50},
            {10, 55},
        };

        public static Dictionary<int, int> Mp = new Dictionary<int, int>()
        {
            {1, 2},
            {2, 4},
            {3, 6},
            {4, 8},
            {5, 10},
            {6, 12},
            {7, 14},
            {8, 16},
            {9, 18},
            {10, 20}
        };

        public static Dictionary<int, float> Spd = new Dictionary<int, float>()
        {
            {1, 0.8f},
            {2, 1.6f},
            {3, 2.4f},
            {4, 3.2f},
            {5, 4f},
            {6, 4.8f},
            {7, 5.6f},
            {8, 6.4f},
            {9, 7.4f},
            {10, 8.2f}
        };

        public static Dictionary<int, int> Param = new Dictionary<int, int>()
        {
            {1, 2},
            {2, 4},
            {3, 6},
            {4, 8},
            {5, 10},
            {6, 12},
            {7, 14},
            {8, 16},
            {9, 18},
            {10, 20}
        };
    }
}