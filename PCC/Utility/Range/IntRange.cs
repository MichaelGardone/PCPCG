using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace PCC.Utility.Range
{
    public class IntRange
    {
        private List<Tuple<int,int>> m_ranges;
        private Random m_random;

        public IntRange(int min, int max)
        {
            m_ranges = new List<Tuple<int, int>>()
            {
                new Tuple<int, int> (min, max + 1)
            };

            m_random = GlobalUtilities.GetRandom();
        }

        public IntRange(int[] mins, int[] maxes, bool errorOut = false)
        {
            if (mins == null || maxes == null) throw new ArgumentNullException("Min/max cannot be null!");
            if (mins.Length != maxes.Length) throw new ArgumentException("You must have the same number of minimums to maximums!");

            m_ranges = new List<Tuple<int,int>>();

            // Create lists
            if (errorOut == false)
            {
                for(int i = 0; i < mins.Length; i++)
                {
                    Tuple<int, int> add;

                    if (mins[i] > maxes[i])
                        add = new Tuple<int, int>(maxes[i], mins[i] + 1);
                    else
                        add = new Tuple<int, int>(mins[i], maxes[i] + 1);

                    // Don't add something that already exists
                    if (m_ranges.Any(x => x.Item1 == add.Item1 && x.Item2 == add.Item2))
                        continue;

                    m_ranges.Add(add);
                }
            }
            else
            {
                for (int i = 0; i < mins.Length; i++)
                {
                    if (mins[i] > maxes[i])
                        throw new ArgumentException("Min/Max ranges are mixed!");

                    // Don't add something that already exists
                    if (m_ranges.Any(x => x.Item1 == mins[i] && x.Item2 == maxes[i] + 1))
                        throw new ArgumentException("Duplicate detected!");

                    m_ranges.Add(new Tuple<int, int>(mins[i], maxes[i] + 1));
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
                    int min = m_ranges[i - 1].Item1;
                    int max = m_ranges[i].Item2;

                    m_ranges.RemoveAt(i);
                    m_ranges[i - 1] = new Tuple<int, int>(min, max);
                }
            }

            m_random = GlobalUtilities.GetRandom();
        }

        public IntRange(List<Tuple<int, int>> ranges)
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
                    int min = m_ranges[i - 1].Item1;
                    int max = m_ranges[i].Item2;

                    m_ranges.RemoveAt(i);
                    m_ranges[i - 1] = new Tuple<int, int>(min, max);
                }
            }

            m_random = GlobalUtilities.GetRandom();
        }

        public IntRange PruneInvalidRanges()
        {
            return null;
        }

        public int[] Pick(IntRange exclude, int num = 1)
        {
            return Pick(num, exclude.m_ranges);
        }

        public int[] Pick(int num = 1, List<Tuple<int,int>>? exclude = null)
        {
            int[] picked = new int[num];

            if (exclude != null)
            {
                // Bail if we have no actual range to play with
                if (DoesOtherRangeOverlapEntirely(exclude))
                    return null;

                List<Tuple<int, int>> validRanges = new List<Tuple<int, int>>();

                foreach(Tuple<int,int> range in m_ranges)
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
                        picked[i] = m_random.Next(validRanges[indx].Item1, validRanges[indx].Item2);
                    } while (GlobalUtilities.IsIntegerInRange(picked[i], exclude));
                }
            }
            else
            {
                for(int i = 0; i < num; i++)
                {
                    int indx = m_random.Next(0, m_ranges.Count);
                    picked[i] = m_random.Next(m_ranges[indx].Item1, m_ranges[indx].Item2);
                }
            }

            return picked;
        }

        public Tuple<int,int> GetRandomRange()
        {
            return m_ranges[m_random.Next(0, m_ranges.Count)];
        }

        public Tuple<int, int> GetARange(int i)
        {
            if (i >= m_ranges.Count || i < 0)
                return null;

            return m_ranges[i];
        }

        public Tuple<int,int>? WhichRangeHasValue(int value)
        {
            Tuple<int, int>? rangeWithVal = null;
            
            foreach(Tuple<int,int> range in m_ranges)
            {
                if(value >= range.Item1 && value <= range.Item2)
                {
                    rangeWithVal = range;
                    break;
                }
            }

            return rangeWithVal;
        }

        public bool DoesOtherRangeOverlapEntirely(IntRange otherIntRange)
        {
            return DoesOtherRangeOverlapEntirely(otherIntRange.m_ranges);
        }

        public bool DoesOtherRangeOverlapEntirely(List<Tuple<int,int>> otherIntRange)
        {
            Tuple<int, int> minRange = otherIntRange[0];
            Tuple<int, int> maxRange = otherIntRange[otherIntRange.Count - 1];

            if (m_ranges[0].Item1 >= minRange.Item1 && m_ranges[m_ranges.Count - 1].Item2 - 1 <= maxRange.Item2)
                return true;

            return false;
        }

        public bool DoesOtherRangeOverlapRange(Tuple<int, int> range, List<Tuple<int, int>> otherIntRange)
        {
            Tuple<int, int> minRange = otherIntRange[0];
            Tuple<int, int> maxRange = otherIntRange[otherIntRange.Count - 1];

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
