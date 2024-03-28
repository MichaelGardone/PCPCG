using PCC.Utility;
using PCC.Utility.Range;

namespace PCCTester.RangeTests
{
    [TestClass]
    public class IntRangeTests
    {
        [TestMethod]
        public void SingleIntRangeTest()
        {
            IntRange range = new IntRange(10, 20);
            int[] results = range.Pick(10);

            Assert.AreEqual(new Tuple<int, int>(10, 21), range.GetARange(0)); // +1 because Next() is not inclusive of the max
            Assert.AreEqual(10, results.Length);

            // returns: list without 12, 13, or 14
            results = range.Pick(10, new List<Tuple<int, int>>() { new Tuple<int, int>(12, 14) });
            Assert.AreEqual(10, results.Length);
            Assert.IsFalse(results.Contains(12));
            Assert.IsFalse(results.Contains(13));
            Assert.IsFalse(results.Contains(14));

            // returns: null
            results = range.Pick(10, new List<Tuple<int, int>>() { new Tuple<int, int>(10, 20) });
            Assert.IsNull(results);

            // returns: null
            results = range.Pick(10, new List<Tuple<int, int>>() { new Tuple<int, int>(9, 22) });
            Assert.IsNull(results);
        }

        [TestMethod]
        public void MultiIntRangeTest()
        {
            IntRange range = new IntRange(new int[] { 25, 10, 30 }, new int[] { 30, 24, 40 });

            // Only two are created as a result of merging
            Assert.AreEqual(2, range.RangeCount());
            // Re-arranged on the basis of max :--> (10,24), (24,40)
            Assert.AreEqual(new Tuple<int, int>(10, 25), range.GetARange(0)); // +1 because Next() is not inclusive of the max
            Assert.AreEqual(new Tuple<int, int>(25, 41), range.GetARange(1));

            int[] results = range.Pick(10);
            Assert.AreEqual(10, results.Length);

            // returns: list without 12, 13, or 14
            results = range.Pick(10, new List<Tuple<int, int>>() { new Tuple<int, int>(12, 14) });
            Assert.AreEqual(10, results.Length);
            Assert.IsFalse(results.Contains(12));
            Assert.IsFalse(results.Contains(13));
            Assert.IsFalse(results.Contains(14));

            // returns: a list excluding 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, and 20 -- can still use 21 - 24
            results = range.Pick(10, new List<Tuple<int, int>>() { new Tuple<int, int>(10, 20) });
            Assert.AreEqual(10, results.Length);
            Assert.IsFalse(results.Any(x => GlobalUtilities.IsIntegerInRange(x, new List<Tuple<int, int>> { new Tuple<int, int>(10, 20) })));

            // returns: a list excluding everything in 10-24
            results = range.Pick(10, new List<Tuple<int, int>>() { new Tuple<int, int>(9, 24) });
            Assert.AreEqual(10, results.Length);
            Assert.IsFalse(results.Any(x => GlobalUtilities.IsIntegerInRange(x, new List<Tuple<int, int>> { new Tuple<int, int>(10, 24) })));

            // returns: null
            results = range.Pick(10, new List<Tuple<int, int>>() { new Tuple<int, int>(9, 40) });
            Assert.IsNull(results);

            // returns: null
            results = range.Pick(10, new List<Tuple<int, int>>() { new Tuple<int, int>(9, 45) });
            Assert.IsNull(results);
        }

        [TestMethod]
        public void MultiIntRange_LotsOData_Test()
        {
            IntRange range = new IntRange(
                new int[] { 25, 10, 30, 38, 60, 110, 70, 110 },
                new int[] { 30, 24, 40, 55, 75, 150, 100, 150 }
            );

            // Only two are created as a result of merging
            Assert.AreEqual(4, range.RangeCount());
            // Re-arranged on the basis of max :--> (10,24), (24,40)
            Assert.AreEqual(new Tuple<int, int>(10, 25), range.GetARange(0)); // +1 because Next() is not inclusive of the max
            Assert.AreEqual(new Tuple<int, int>(25, 56), range.GetARange(1));
            Assert.AreEqual(new Tuple<int, int>(60, 101), range.GetARange(2));
            Assert.AreEqual(new Tuple<int, int>(110, 151), range.GetARange(3));
        }
    }
}