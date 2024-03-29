namespace PCC.Utility
{
    public static class GlobalUtilities
    {
        private static Random random;

        public static Random GetRandom(int seed = 424413)
        {
            if (random == null)
                random = new Random(seed);

            return random;
        }

        public static void ResetRandom(int seed = 424413)
        {
            random = new Random(seed);
        }

        public static bool IsIntegerInRange(int integer, List<Tuple<int,int>> ranges)
        {
            foreach (Tuple<int, int> range in ranges)
            {
                if (integer >= range.Item1 && integer <= range.Item2)
                    return true;
            }

            return false;
        }

        public static bool IsFloatInRange(float real, List<Tuple<float, float>> ranges)
        {
            foreach (Tuple<float, float> range in ranges)
            {
                if (real >= range.Item1 && real <= range.Item2)
                    return true;
            }

            return false;
        }
    }
}
