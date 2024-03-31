using LibSVMsharp;
using LibSVMsharp.Extensions;
using PCC.ContentRepresentation.Features;
using PCC.ContentRepresentation.Sample;
using PCC.Utility;
using PCC.Utility.Memory;

namespace PCC.CurationMethod.Binary
{
    public struct SVMCuratorProperties
    {
        public SVMCuratorProperties() { }

        public int DislikedMemSize = 15;
        public int LikedMemSize = 15;
        public int BagSize = 5;
        public int SigfigCount = 3;

        /// <summary>
        /// Retrain a given model if the number of samples falls below this.
        /// </summary>
        public float RetrainModelOnSampleCount = 0;

        /// <summary>
        /// Range of uncertainty for a given sample. Used in RANDOM_FROM_UNKNOW, RANDOM_FROM_KNOWN, SAFE_ONLY, and UNSAFE_ONLY generation.
        /// </summary>
        public Tuple<float, float> UnknownRange = new Tuple<float, float>(0.4f, 0.55f);

        /// <summary>
        /// The max number of attempts to generate a sample before taking whatever comes out.
        /// </summary>
        public int MaxAttempts = 5;
    }

    public struct ModelNComponents
    {
        public SVMModel model;

        // If either of these hit zero, retrain on new data!
        public List<int> positiveSampleIds;
        public List<int> negativeSampleIds;

        public int totalNumSamples;
    }

    public enum RangeTreatement
    {
        /// <summary>
        /// Use values in the unknown range (i.e., high model disagreement
        /// </summary>
        UNKNOWN,
        /// <summary>
        /// Only let the sample through if the models are in agreement in one direction of another.
        /// </summary>
        KNOWN,
        /// <summary>
        /// Only let the sample through if most models rate highly.
        /// </summary>
        LIKED,
        /// <summary>
        /// Only let the sample throgh if most models rate poorly.
        /// </summary>
        DISLIKED,
    }

    public class BSVMCurator : ICurationMethod
    {
        // Data
        protected RingBuffer<SVMNode[]> likedSamples;
        protected RingBuffer<SVMNode[]> dislikedSamples;

        protected RingBuffer<ModelNComponents> models;
        // Data

        // SVM information
        protected SVMParameter parameter;
        protected SVMCuratorProperties properties;
        protected List<Feature> features;
        // SVM information

        protected int minSamples;
        public int MinimumSamplesForSVM { get { return minSamples; } }

        /// <summary>
        /// If the history has at least one liked and one disliked sample, then we're good to train!
        /// </summary>
        public bool CanTrain
        {
            get
            {
                return dislikedSamples.Count() > 1 && likedSamples.Count() > 1 && likedSamples.Count() + dislikedSamples.Count() > minSamples;
            }
        }

        public BSVMCurator(List<Feature> features, SVMParameter svmParameters, SVMCuratorProperties curProperties)
        {
            minSamples = features.Count;
            this.features = features;

            properties = curProperties;
            parameter = svmParameters;
            
            models = new RingBuffer<ModelNComponents>(curProperties.BagSize);
            likedSamples = new RingBuffer<SVMNode[]>(curProperties.LikedMemSize);
            dislikedSamples = new RingBuffer<SVMNode[]>(curProperties.DislikedMemSize);
        }

        public void ClearMemory()
        {
            likedSamples.Clear();
            dislikedSamples.Clear();

            // Re-allocate the array, LibSVM will free the memory itself
            models = new RingBuffer<ModelNComponents>(properties.BagSize);
        }

        public List<Sample> GenerateSamples(int count = 1, SampleGenerationMethod method = SampleGenerationMethod.RANDOM)
        {
            List<Sample> samples = new List<Sample>();

            Dictionary<string, Tuple<int[], float[]>>? sampleVals;

            if (method == SampleGenerationMethod.RANDOM || (method != SampleGenerationMethod.RANDOM && models.Count() == 0))
            {
                sampleVals = GenerateRandomSamples(count);
            }
            else if (method == SampleGenerationMethod.RANDOM_FROM_UNKNOWNS)
            {
                sampleVals = GenerateRandomSamplesWithModels(count, RangeTreatement.UNKNOWN);
            }
            else if (method == SampleGenerationMethod.RANDOM_FROM_KNOWNS)
            {
                sampleVals = GenerateRandomSamplesWithModels(count, RangeTreatement.KNOWN);
            }
            else if (method == SampleGenerationMethod.SAFE_ONLY)
            {
                sampleVals = GenerateRandomSamplesWithModels(count, RangeTreatement.LIKED);
            }
            else if (method == SampleGenerationMethod.UNSAFE_ONLY)
            {
                sampleVals = GenerateRandomSamplesWithModels(count, RangeTreatement.DISLIKED);
            }
            else
            {
                throw new ArgumentException("Unknown generation method detected!");
            }

            if (sampleVals.Count == 0)
                return samples;

            // Synthesize the samples
            for (int i = 0; i < count; i++)
            {
                Sample s = new Sample();
                foreach (Feature f in features)
                {
                    // enums are fun
                    switch (f.FeatureType)
                    {
                        case FeatureType.INT:
                            s.AddValue(f.Name, sampleVals![f.Name].Item1[i]);
                            break;
                        case FeatureType.FLOAT:
                            s.AddValue(f.Name, MathF.Round(sampleVals![f.Name].Item2[i], properties.SigfigCount));
                            break;
                    }
                }
                samples.Add(s);
            }

            return samples;
        }

        public List<Feature> GetFeatures()
        {
            return features;
        }

        public List<int> GetLabels()
        {
            return new List<int> { -1, 1 };
        }

        public Sample? GetRandomSample()
        {
            throw new NotImplementedException();
        }

        public Sample? GetSample(int label, int sampleIndex)
        {
            throw new NotImplementedException();
        }

        public void RecordSample(Sample sample, int label)
        {
            if(label == -1)
            {
                dislikedSamples.Add(LibSVMHelper.SampleToSVMNode(sample));

                // Get the one we just replaced...
                int id = dislikedSamples.GetNextSlotToFill() - 1;
                if (id < 0)
                {
                    id = dislikedSamples.Count() - 1;
                }

                for (int i = 0; i < models.Count(); i++)
                {
                    ModelNComponents mnc = models[i];

                    if (mnc.negativeSampleIds.Contains(0))
                    {
                        mnc.negativeSampleIds.Remove(id);
                        
                        // If we have no more samples here, we can't trust this model anymore!
                        if (mnc.negativeSampleIds.Count == properties.RetrainModelOnSampleCount)
                        {
                            models.Replace(i, TrainNewModel());
                        }
                    }
                }
            }
            else if(label == 1)
            {
                likedSamples.Add(LibSVMHelper.SampleToSVMNode(sample));

                for (int i = 0; i < models.Count(); i++)
                {
                    ModelNComponents mnc = models[i];

                    if (mnc.positiveSampleIds.Contains(0))
                    {
                        mnc.positiveSampleIds.Remove(0);

                        // If we have no more samples here, we can't trust this model anymore!
                        if (mnc.positiveSampleIds.Count == properties.RetrainModelOnSampleCount)
                        {
                            models.Replace(i, TrainNewModel());
                        }
                    }
                }
            }
        }

        public float RateSample(Sample sample)
        {
            float rate = 1f;
            float modelContrib = 1f / models.Count();

            // Generate once, then use in every model
            SVMNode[] nodes = LibSVMHelper.SampleToSVMNode(sample);

            // Run each model to predict the rating on it
            foreach (ModelNComponents model in models)
            {
                if (model.model.Predict(nodes) == 1)
                    rate -= modelContrib;
            }

            return rate;
        }

        /// <summary>
        /// Force the curator to retrain all models under the current history.
        /// </summary>
        public void ForceRetrain()
        {
            // Just toss them out, no need to keep them around
            models.Clear();

            // Now just make new ones! This may take some time unfortunately...
            for(int i = 0; i < properties.BagSize; i++)
            {
                models.Add(TrainNewModel());
            }
        }

        public void AddModel(ModelNComponents model)
        {
            models.Add(model);
        }

        public ModelNComponents TrainNewModel()
        {
            // Bail, we can't actually do anything
            if (CanTrain == false)
                return new ModelNComponents();

            double off = GlobalUtilities.GetRandom().NextDouble();
            off = off < 0.1 ? 0.1 : off;
            int posSampleCount = (int)Math.Round(minSamples * off);

            off = GlobalUtilities.GetRandom().NextDouble();
            off = off < 0.1 ? 0.1 : off;
            int negSampleCount = (int)Math.Round(minSamples * off);

            List<int> posSamples = new List<int>();
            List<int> negSamples = new List<int>();

            if (likedSamples.Count() > posSampleCount)
            {
                do
                {
                    int indx;
                    do
                    {
                        indx = GlobalUtilities.GetRandom().Next(0, likedSamples.Count());
                    } while (posSamples.Contains(indx));

                    posSamples.Add(indx);
                } while(posSamples.Count <  posSampleCount);
            }
            else
            {
                // Randomize the order
                List<int> ints = GlobalUtilities.CreateListOfIntsUpTo(likedSamples.Count());

                while(ints.Count > 0)
                {
                    int indx = GlobalUtilities.GetRandom().Next(0, ints.Count);

                    posSamples.Add(ints[indx]);
                    ints.RemoveAt(indx);
                }
            }

            if (dislikedSamples.Count() > negSampleCount)
            {
                do
                {
                    int indx;
                    do
                    {
                        indx = GlobalUtilities.GetRandom().Next(0, dislikedSamples.Count());
                    } while (negSamples.Contains(indx));

                    negSamples.Add(indx);
                } while (negSamples.Count < posSampleCount);
            }
            else
            {
                // Randomize the order
                List<int> ints = GlobalUtilities.CreateListOfIntsUpTo(dislikedSamples.Count());

                while (ints.Count > 0)
                {
                    int indx = GlobalUtilities.GetRandom().Next(0, ints.Count);

                    negSamples.Add(ints[indx]);
                    ints.RemoveAt(indx);
                }
            }

            // Grab samples and train models
            SVMProblem problem = new SVMProblem();
            foreach (int indx in posSamples)
            {
                problem.Add(likedSamples[indx], 1);
            }

            foreach (int indx in negSamples)
            {
                problem.Add(dislikedSamples[indx], -1);
            }

            ModelNComponents model = new ModelNComponents
            {
                model = problem.Train(parameter),
                positiveSampleIds = posSamples,
                negativeSampleIds = negSamples,
                totalNumSamples = posSamples.Count + negSamples.Count
            };

            return model;
        }

        protected Dictionary<string, Tuple<int[], float[]>> GenerateRandomSamples(int count)
        {
            Dictionary<string, Tuple<int[], float[]>> generatedFeatures = new Dictionary<string, Tuple<int[], float[]>>();

            foreach (Feature f in features)
            {
                if (f.FeatureType == FeatureType.INT)
                {
                    generatedFeatures.Add(f.Name, new Tuple<int[], float[]>(f.Range.intRange.Pick(count), Array.Empty<float>()));
                }
                else if (f.FeatureType == FeatureType.FLOAT)
                {
                    generatedFeatures.Add(f.Name, new Tuple<int[], float[]>(Array.Empty<int>(), f.Range.floatRange.Pick(count)));
                }
            }

            return generatedFeatures;
        }

        protected Dictionary<string, Tuple<int[], float[]>> GenerateRandomSamplesWithModels(int count, RangeTreatement treatment)
        {
            Dictionary<string, Tuple<int[], float[]>> generatedFeatures = new Dictionary<string, Tuple<int[], float[]>>();

            return generatedFeatures;
        }
    }
}
