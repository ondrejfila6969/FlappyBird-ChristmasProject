using System;

namespace FlappyBird
{
    public static class RandomHelper
    {
        private static Random _random = new Random();

        public static int Next(int minValue, int maxValue)
        {
            return _random.Next(minValue, maxValue);
        }
    }
}
