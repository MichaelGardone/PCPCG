using LibSVMsharp;
using PCC.ContentRepresentation.Features;
using PCC.ContentRepresentation.Sample;
using PCC.CurationMethod.Binary;
using PCC.Utility;
using PCC.Utility.Range;

namespace PCCTester.CurationFunctionality.Binary
{
    [TestClass]
    public class SVMCuratorTests
    {
        [TestMethod]
        public void CSVMCuratorTest()
        {
            GlobalUtilities.ResetRandom();

            SVMParameter parameters = new SVMParameter {
                Type = SVMType.C_SVC,
                Kernel = SVMKernelType.RBF,
                C = 1,
                Gamma = 1
            };

            SVMCuratorProperties properties = new SVMCuratorProperties();
            
            List<Feature> features = new List<Feature>(){
                new Feature("cherries", new IntRange(0, 2)),
                new Feature("power pellets", new FloatRange(0.02f, 0.3f))
            };

            BSVMCurator curator = new BSVMCurator(features, parameters, properties);

            List<Sample> samples = curator.GenerateSamples(5, PCC.CurationMethod.SampleGenerationMethod.RANDOM);

            for(int i = 0; i < samples.Count; i++)
            {
                if (i % 2 == 0)
                    curator.RecordSample(samples[i], -1);
                else
                    curator.RecordSample(samples[i], 1);
            }

            Assert.IsTrue(curator.CanTrain);

            // Train and add a new model
            curator.AddModel(curator.TrainNewModel());
        }

    }
}
