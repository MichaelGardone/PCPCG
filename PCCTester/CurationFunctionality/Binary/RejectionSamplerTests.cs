using PCC.ContentRepresentation.Features;
using PCC.ContentRepresentation.Sample;
using PCC.CurationMethod;
using PCC.CurationMethod.Binary;
using PCC.Utility;
using PCC.Utility.Range;

namespace PCCTester.CurationFunctionality.Binary
{
    [TestClass]
    public class RejectionSamplerTests
    {
        [TestMethod]
        public void InfMem_RejectionSamplerTest()
        {
            // Reset in the event we are running all tests at once -- it's a static object,
            //  so it'll persist for application (test) duration
            GlobalUtilities.ResetRandom();

            int numOfSamples = 5;

            BRSCurator rejSampler = new BRSCurator(
                new List<Feature>() {
                    new Feature("cherries", new IntRange(0, 2)),
                    new Feature("power pellets", new FloatRange(0.02f, 0.3f))
                },
                -1, -1, false, 0.2f, 0.2f, 2
            );

            List<Sample> samples = rejSampler.GenerateSamples(numOfSamples, SampleGenerationMethod.RANDOM);
            Assert.IsNotNull(samples);
            Assert.AreEqual(numOfSamples, samples.Count);

            Random rand = GlobalUtilities.GetRandom();

            // Just randomly assign a value, because of the seed it'll be the same every time
            int[] choices = new int[2] { -1, 1 };
            for(int i = 0; i < samples.Count; i++)
            {
                rejSampler.RecordSample(samples[i], choices[rand.Next(0, 2)]);
            }

            samples = rejSampler.GenerateSamples(numOfSamples, SampleGenerationMethod.RANDOM_FROM_KNOWNS);
            Assert.IsNotNull(samples);
            Assert.AreEqual(numOfSamples, samples.Count);

            samples = rejSampler.GenerateSamples(numOfSamples, SampleGenerationMethod.RANDOM_FROM_UNKNOWNS);
            Assert.IsNotNull(samples);
            Assert.AreEqual(numOfSamples, samples.Count);

            samples = rejSampler.GenerateSamples(numOfSamples, SampleGenerationMethod.SAFE_ONLY);
            Assert.IsNotNull(samples);
            Assert.AreEqual(numOfSamples, samples.Count);

            samples = rejSampler.GenerateSamples(numOfSamples, SampleGenerationMethod.UNSAFE_ONLY);
            Assert.IsNotNull(samples);
            Assert.AreEqual(0, samples.Count);
        }
    }
}
