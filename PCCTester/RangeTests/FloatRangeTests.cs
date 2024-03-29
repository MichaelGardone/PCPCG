using PCC.Utility.Range;
using PCC.Utility;

namespace PCCTester.RangeTests
{
    [TestClass]
    public class FloatRangeTests
    {
        [TestMethod]
        public void SingleFloatRangeTest()
        {
            FloatRange range = new FloatRange(10, 20);
            float[] results = range.Pick(10);

            Assert.AreEqual(new Tuple<float, float>(10, 20.0001f), range.GetARange(0)); // +1 because Next() is not inclusive of the max
            Assert.AreEqual(10, results.Length);

            // returns: list without 12, 13, or 14
            results = range.Pick(10, new List<Tuple<float, float>>() { new Tuple<float, float>(12, 14) });
            Assert.AreEqual(10, results.Length);
            Assert.IsFalse(results.Contains(12));
            Assert.IsFalse(results.Contains(13));
            Assert.IsFalse(results.Contains(14));

            // returns: null
            results = range.Pick(10, new List<Tuple<float, float>>() { new Tuple<float, float>(10, 20) });
            Assert.IsNull(results);

            // returns: null
            results = range.Pick(10, new List<Tuple<float, float>>() { new Tuple<float, float>(9, 22) });
            Assert.IsNull(results);
        }

        [TestMethod]
        public void MultiFloatRangeTest()
        {
            FloatRange range = new FloatRange(new float[] { 25, 10, 30 }, new float[] { 30, 24, 40 });

            // Only two are created as a result of merging
            Assert.AreEqual(2, range.RangeCount());
            // Re-arranged on the basis of max :--> (10,24), (24,40)
            Assert.AreEqual(new Tuple<float, float>(10, 24.0001f), range.GetARange(0)); // +1 because Next() is not inclusive of the max
            Assert.AreEqual(new Tuple<float, float>(25, 40.0001f), range.GetARange(1));

            float[] results = range.Pick(10);
            Assert.AreEqual(10, results.Length);

            // returns: list without 12, 13, or 14
            results = range.Pick(10, new List<Tuple<float, float>>() { new Tuple<float, float>(12, 14) });
            Assert.AreEqual(10, results.Length);
            Assert.IsFalse(results.Contains(12));
            Assert.IsFalse(results.Contains(13));
            Assert.IsFalse(results.Contains(14));

            // returns: a list excluding 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, and 20 -- can still use 21 - 24
            results = range.Pick(10, new List<Tuple<float, float>>() { new Tuple<float, float>(10, 20) });
            Assert.AreEqual(10, results.Length);
            Assert.IsFalse(results.Any(x => GlobalUtilities.IsFloatInRange(x, new List<Tuple<float, float>> { new Tuple<float, float>(10, 20) })));

            // returns: a list excluding everything in 10-24
            results = range.Pick(10, new List<Tuple<float, float>>() { new Tuple<float, float>(9, 24) });
            Assert.AreEqual(10, results.Length);
            Assert.IsFalse(results.Any(x => GlobalUtilities.IsFloatInRange(x, new List<Tuple<float, float>> { new Tuple<float, float>(10, 24) })));

            // returns: null
            results = range.Pick(10, new List<Tuple<float, float>>() { new Tuple<float, float>(9, 40) });
            Assert.IsNull(results);

            // returns: null
            results = range.Pick(10, new List<Tuple<float, float>>() { new Tuple<float, float>(9, 45) });
            Assert.IsNull(results);
        }

        [TestMethod]
        public void MultiFloatRange_LotsOData_Test()
        {
            FloatRange range = new FloatRange(
                new float[] { 25, 10, 30, 38, 60, 110, 70, 110 },
                new float[] { 30, 24, 40, 55, 75, 150, 100, 150 }
            );

            // Only two are created as a result of merging
            Assert.AreEqual(4, range.RangeCount());
            // Re-arranged on the basis of max :--> (10,24), (24,40)
            Assert.AreEqual(new Tuple<float, float>(10, 24.0001f), range.GetARange(0)); // +1 because Next() is not inclusive of the max
            Assert.AreEqual(new Tuple<float, float>(25, 55.0001f), range.GetARange(1));
            Assert.AreEqual(new Tuple<float, float>(60, 100.0001f), range.GetARange(2));
            Assert.AreEqual(new Tuple<float, float>(110, 150.0001f), range.GetARange(3));
        }
    }
}
