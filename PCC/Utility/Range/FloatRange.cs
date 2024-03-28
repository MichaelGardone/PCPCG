using System;
using System.Diagnostics.CodeAnalysis;

namespace PCC.Utility.Range
{
    public class FloatRange
    {
        private List<Tuple<float,float>> m_ranges;
        private Random m_random;

        public FloatRange(float min, float max)
        {
            m_ranges = new List<Tuple<float, float>>()
            {
                new Tuple<float, float> (min, max + 1)
            };

            m_random = GlobalUtilities.GetRandom();
        }

        public FloatRange(float[] mins, float[] maxes, bool errorOut = false)
        {
            if (mins == null || maxes == null) throw new ArgumentNullException("Min/max cannot be null!");
            if (mins.Length != maxes.Length) throw new ArgumentException("You must have the same number of minimums to maximums!");

            m_ranges = new List<Tuple<float,float>>();

            // Create lists
            if (errorOut == false)
            {
                for(int i = 0; i < mins.Length; i++)
                {
                    if (mins[i] > maxes[i])
                        m_ranges.Add(new Tuple<float, float>(maxes[i], mins[i] + 1));
                    else
                        m_ranges.Add(new Tuple<float, float>(mins[i], maxes[i] + 1));
                }
            }
            else
            {
                for (int i = 0; i < mins.Length; i++)
                {
                    if (mins[i] > maxes[i])
                        throw new ArgumentException("Min/Max ranges are mixed!");
                    m_ranges.Add(new Tuple<float, float>(mins[i], maxes[i] + 1));
                }
            }

            // Order/fix
            m_ranges = m_ranges.OrderBy(t => t.Item2).ToList();

            // Flag ones that overlap
            bool[] merge = new bool[m_ranges.Count];

            for(int i = 0; i < m_ranges.Count - 1; i++)
            {
                if (m_ranges[i].Item2 - 1 >= m_ranges[i + 1].Item1)
                    merge[i + 1] = true;
            }

            // Merge them
            for(int i = merge.Length - 1; i > 0; i--)
            {
                if (merge[i])
                {
                    float min = m_ranges[i - 1].Item1;
                    float max = m_ranges[i].Item2;

                    m_ranges.RemoveAt(i);
                    m_ranges[i - 1] = new Tuple<float, float>(min, max);
                }
            }

            m_random = GlobalUtilities.GetRandom();
        }

        public FloatRange(List<Tuple<float, float>> ranges)
        {
            m_ranges = ranges;

            // Order/fix
            m_ranges = m_ranges.OrderBy(t => t.Item2).ToList();

            // Flag ones that overlap
            bool[] merge = new bool[m_ranges.Count];

            for (int i = 0; i < m_ranges.Count - 1; i++)
            {
                if (m_ranges[i].Item2 - 1 >= m_ranges[i + 1].Item1)
                    merge[i + 1] = true;
            }

            // Merge them
            for (int i = merge.Length - 1; i > 0; i--)
            {
                if (merge[i])
                {
                    float min = m_ranges[i - 1].Item1;
                    float max = m_ranges[i].Item2;

                    m_ranges.RemoveAt(i);
                    m_ranges[i - 1] = new Tuple<float, float>(min, max);
                }
            }

            m_random = GlobalUtilities.GetRandom();
        }

        public FloatRange PruneInvalidRanges()
        {
            return null;
        }

        public float[] Pick(int num = 1, List<Tuple<float,float>>? exclude = null)
        {
            float[] picked = new float[num];

            if (exclude != null)
            {
                // Bail if we have no actual range to play with
                if (DoesOtherRangeOverlapEntirely(exclude))
                    return null;

                List<Tuple<float, float>> validRanges = new List<Tuple<float, float>>();

                foreach(Tuple<float,float> range in m_ranges)
                {
                    if (DoesOtherRangeOverlapRange(range, exclude))
                        continue;
                    validRanges.Add(range);
                }

                for (int i = 0; i < num; i++)
                {
                    do
                    {
                        int indx = m_random.Next(0, validRanges.Count);
                        picked[i] = (float)(m_random.NextDouble() * (validRanges[indx].Item2 - validRanges[indx].Item1) + validRanges[indx].Item1);
                    } while (GlobalUtilities.IsFloatInRange(picked[i], exclude));
                }
            }
            else
            {
                for(int i = 0; i < num; i++)
                {
                    int indx = m_random.Next(0, m_ranges.Count);
                    picked[i] = (float)(m_random.NextDouble() * (m_ranges[indx].Item2 - m_ranges[indx].Item1) + m_ranges[indx].Item1);
                }
            }

            return picked;
        }

        public Tuple<float,float> GetRandomRange()
        {
            return m_ranges[m_random.Next(0, m_ranges.Count)];
        }

        public Tuple<float, float> GetARange(int i)
        {
            if (i >= m_ranges.Count || i < 0)
                return null;

            return m_ranges[i];
        }
        
        public bool DoesOtherRangeOverlapEntirely([NotNull] List<Tuple<float,float>> otherFloatRange)
        {
            Tuple<float, float> minRange = otherFloatRange[0];
            Tuple<float, float> maxRange = otherFloatRange[otherFloatRange.Count - 1];

            if (m_ranges[0].Item1 >= minRange.Item1 && m_ranges[m_ranges.Count - 1].Item2 - 1 <= maxRange.Item2)
                return true;

            return false;
        }

        public bool DoesOtherRangeOverlapRange(Tuple<float, float> range, [NotNull] List<Tuple<float, float>> otherFloatRange)
        {
            Tuple<float, float> minRange = otherFloatRange[0];
            Tuple<float, float> maxRange = otherFloatRange[otherFloatRange.Count - 1];

            if (range.Item1 >= minRange.Item1 && range.Item2 - 1 <= maxRange.Item2)
                return true;

            return false;
        }

        public int RangeCount()
        {
            return m_ranges.Count;
        }
    }
}
