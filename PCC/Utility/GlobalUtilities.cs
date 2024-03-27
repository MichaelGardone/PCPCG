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

        public static bool IsIntegerInRange(int integer, List<Tuple<int,int>> ranges)
        {
            foreach (Tuple<int, int> range in ranges)
            {
                if (integer >= range.Item1 && integer <= range.Item2)
                    return true;
            }

            return false;
        }
    }
}
