using PCC.ContentRepresentation.Features;
using PCC.ContentRepresentation.Sample;
using PCC.Utility;
using PCC.Utility.Memory;
using PCC.Utility.Range;
using System;
using System.Collections.Generic;

namespace PCC.CurationMethod.Binary
{
    /// <summary>
    /// A binary rejection sampler-based content curator. Only accepts either "Like" or "Dislike" for samples.
    /// </summary>
    public class BRSCurator : ICurationMethod
    {
        protected IMemoryBuffer<HistoricSample>? likedBuffer;
        protected IMemoryBuffer<HistoricSample>? dislikedBuffer;
        protected IMemoryBuffer<Tuple<HistoricSample, int>>? memoryBuffer;

        protected int sigfigCount;
        protected float negInfluence;
        protected float posInfluence;

        protected List<Feature> features;

        public BRSCurator(List<Feature> features,
            int goodMemSize = -1, int badMemSize = -1, bool useDiffMems = false,
            float negInfluence = 0.2f, float posInfluence = 0.2f,
            int sigfigs = 3)
            {
            if (useDiffMems)
                {
                if (goodMemSize <= 0)
                {
                    likedBuffer = new InfiniteBuffer<HistoricSample>();
                }
                else
                {
                    likedBuffer = new RingBuffer<HistoricSample>(goodMemSize);
                }

                if (badMemSize <= 0)
                    dislikedBuffer = new InfiniteBuffer<HistoricSample>();
                else
                    dislikedBuffer = new RingBuffer<HistoricSample>(badMemSize);
            }
            else
            {
                if (goodMemSize > 0)
                    memoryBuffer = new RingBuffer<Tuple<HistoricSample, int>>(goodMemSize);
                else if (badMemSize > 0)
                    memoryBuffer = new RingBuffer<Tuple<HistoricSample, int>>(badMemSize);
                else
                    memoryBuffer = new InfiniteBuffer<Tuple<HistoricSample, int>>();
            }

            sigfigCount = sigfigs;
            this.negInfluence = negInfluence;
            this.posInfluence = posInfluence;

            this.features = features;
        }

        public void ClearMemory()
        {
            dislikedBuffer?.Clear();
            likedBuffer?.Clear();
            memoryBuffer?.Clear();
        }

        public List<Sample> GenerateSamples(int count = 1, SampleGenerationMethod method = SampleGenerationMethod.RANDOM)
        {
            List<Sample> samples = new List<Sample>();

            Dictionary<string, Tuple<int[], float[]>>? sampleVals;

            if (method == SampleGenerationMethod.RANDOM)
            {
                sampleVals = GenerateRandomSamples(count);
            }
            else if(method == SampleGenerationMethod.RANDOM_FROM_UNKNOWNS)
            {
                sampleVals = GenerateRandomSamplesUsingHistory(count, false, 0);
            }
            else if(method == SampleGenerationMethod.RANDOM_FROM_KNOWNS)
            {
                sampleVals = GenerateRandomSamplesUsingHistory(count, true, 0);
            }
            else if(method == SampleGenerationMethod.SAFE_ONLY)
            {
                sampleVals = GenerateRandomSamplesUsingHistory(count, true, 1);
            }
            else if(method == SampleGenerationMethod.UNSAFE_ONLY)
            {
                sampleVals = GenerateRandomSamplesUsingHistory(count, true, -1);
            }
            else
            {
                throw new ArgumentException("Unknown generation method detected!");
            }

            if (sampleVals.Count == 0)
                return samples;

            // Synthesize the samples
            for(int i = 0; i < count; i++)
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
                            s.AddValue(f.Name, MathF.Round(sampleVals![f.Name].Item2[i], sigfigCount));
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
            if(memoryBuffer != null)
            {
                return new Sample(memoryBuffer.Get(GlobalUtilities.GetRandom().Next(0, memoryBuffer.Count())).Item1);
            }

            if (dislikedBuffer!.Count() > 0 && likedBuffer!.Count() > 0)
            {
                int pick = GlobalUtilities.GetRandom().Next(0, 2);

                if (pick == 1)
                {
                    return new Sample(likedBuffer.Get(GlobalUtilities.GetRandom().Next(0, likedBuffer.Count())));
                }
                else
                {
                    return new Sample(dislikedBuffer.Get(GlobalUtilities.GetRandom().Next(0, dislikedBuffer.Count())));
                }
            }
            else if(dislikedBuffer!.Count() > 0)
            {
                return new Sample(dislikedBuffer.Get(GlobalUtilities.GetRandom().Next(0, dislikedBuffer.Count())));
            }
            else if(likedBuffer!.Count() > 0)
            {
                return new Sample(likedBuffer.Get(GlobalUtilities.GetRandom().Next(0, likedBuffer.Count())));
            }
            else
            {
                // No samples so... you get nothing!
                return null;
            }
        }

        public Sample? GetSample(int label, int sampleIndex)
        {
            if (memoryBuffer != null)
            {
                for(int i = 0; i < memoryBuffer.Count(); i++)
                {
                    if(label == memoryBuffer.Get(i).Item2 && i >= sampleIndex)
                    {
                        return new Sample(memoryBuffer.Get(i).Item1);
                    }
                }

                return null;
            }

            if (label == -1)
            {
                return new Sample(dislikedBuffer!.Get(sampleIndex));
            }
            else
            {
                return new Sample(likedBuffer!.Get(sampleIndex));
            }
        }

        public void RecordSample(Sample sample, int label)
        {
            float eps = 0;
            if(label == 1)
            {
                eps = posInfluence;
                likedBuffer?.Add(new HistoricSample(features, sample, eps, sigfigCount));
            }
            else if(label == -1)
            {
                eps = negInfluence;
                dislikedBuffer?.Add(new HistoricSample(features, sample, eps, sigfigCount));
            }

            memoryBuffer?.Add(new Tuple<HistoricSample, int>(new HistoricSample(features, sample, eps, sigfigCount), label));
        }

        protected Dictionary<string, Tuple<int[], float[]>> GenerateRandomSamples(int count)
        {
            Dictionary<string, Tuple<int[], float[]>> generatedFeatures = new Dictionary<string, Tuple<int[], float[]>>();

            foreach(Feature f in features)
            {
                if(f.FeatureType == FeatureType.INT)
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

        /// <summary>
        /// Generate a random sample using the curator's history.
        /// </summary>
        /// <param name="count">Num of samples to be generated.</param>
        /// <param name="useKnown">Set to true if to only select in known ranges. If false, will only look at unknown ranges.</param>
        /// <param name="whichHistory">0 = use both, 1 = use 1, 2 = use -1</param>
        /// <returns></returns>
        protected Dictionary<string, Tuple<int[], float[]>> GenerateRandomSamplesUsingHistory(int count, bool useKnown, int whichHistory = 0)
        {
            Dictionary<string, Tuple<int[], float[]>> generatedFeatures = new Dictionary<string, Tuple<int[], float[]>>();

            // Get all historic samples out
            Dictionary<string, List<Tuple<int,int>>> knownIntRanges = new Dictionary<string, List<Tuple<int, int>>>();
            Dictionary<string, List<Tuple<float, float>>> knownFloatRanges = new Dictionary<string, List<Tuple<float, float>>>();

            if (memoryBuffer != null)
            {
                for (int i = 0; i < memoryBuffer.Count(); i++)
                {
                    Tuple<HistoricSample, int> sample = memoryBuffer.Get(i);

                    // Skip if we aren't looking at the right sample
                    if ((whichHistory == 1 && sample.Item2 == -1) || (whichHistory == -1 && sample.Item2 == 1))
                        continue;

                    foreach(Feature feat in features)
                    {
                        Tuple<SampleValue, SampleValue> vals = sample.Item1[feat.Name]!;

                        switch (feat.FeatureType)
                        {
                            case FeatureType.INT:
                                if (knownIntRanges.ContainsKey(feat.Name) == false)
                                    knownIntRanges[feat.Name] = new List<Tuple<int, int>>();

                                knownIntRanges[feat.Name].Add(new Tuple<int, int>(vals.Item1.intVal, vals.Item2.intVal));
                                break;
                            case FeatureType.FLOAT:
                                if (knownFloatRanges.ContainsKey(feat.Name) == false)
                                    knownFloatRanges[feat.Name] = new List<Tuple<float, float>>();

                                knownFloatRanges[feat.Name].Add(new Tuple<float, float>(vals.Item1.floatVal, vals.Item2.floatVal));
                                break;
                        }
                    }
                }
            }
            else
            {
                if (whichHistory == 0 || whichHistory == 1)
                {
                    for (int i = 0; i < likedBuffer!.Count(); i++)
                    {
                        HistoricSample sample = likedBuffer!.Get(i);
                        
                        foreach (Feature feat in features)
                        {
                            Tuple<SampleValue, SampleValue> vals = sample[feat.Name]!;

                            switch (feat.FeatureType)
                            {
                                case FeatureType.INT:
                                    if (knownIntRanges.ContainsKey(feat.Name) == false)
                                        knownIntRanges[feat.Name] = new List<Tuple<int, int>>();

                                    knownIntRanges[feat.Name].Add(new Tuple<int, int>(vals.Item1.intVal, vals.Item2.intVal));
                                    break;
                                case FeatureType.FLOAT:
                                    if (knownFloatRanges.ContainsKey(feat.Name) == false)
                                        knownFloatRanges[feat.Name] = new List<Tuple<float, float>>();

                                    knownFloatRanges[feat.Name].Add(new Tuple<float, float>(vals.Item1.floatVal, vals.Item2.floatVal));
                                    break;
                            }
                        }
                    }
                }

                if(whichHistory == 0 || whichHistory == -1)
                {
                    for (int i = 0; i < dislikedBuffer!.Count(); i++)
                    {
                        HistoricSample sample = dislikedBuffer.Get(i);
                        
                        foreach (Feature feat in features)
                        {
                            Tuple<SampleValue, SampleValue> vals = sample[feat.Name]!;

                            switch (feat.FeatureType)
                            {
                                case FeatureType.INT:
                                    if (knownIntRanges.ContainsKey(feat.Name) == false)
                                        knownIntRanges[feat.Name] = new List<Tuple<int, int>>();

                                    knownIntRanges[feat.Name].Add(new Tuple<int, int>(vals.Item1.intVal, vals.Item2.intVal));
                                    break;
                                case FeatureType.FLOAT:
                                    if (knownFloatRanges.ContainsKey(feat.Name) == false)
                                        knownFloatRanges[feat.Name] = new List<Tuple<float, float>>();

                                    knownFloatRanges[feat.Name].Add(new Tuple<float, float>(vals.Item1.floatVal, vals.Item2.floatVal));
                                    break;
                            }
                        }
                    }
                }
            }

            if (knownIntRanges.Count == knownFloatRanges.Count && knownIntRanges.Count == 0)
            {
                return generatedFeatures;
            }

            // Done, now fix
            Dictionary<string, IntRange> intRange = new Dictionary<string, IntRange>();
            foreach(var irng in knownIntRanges)
            {
                intRange.Add(irng.Key, new IntRange(knownIntRanges[irng.Key]));
            }

            Dictionary<string, FloatRange> floatRange = new Dictionary<string, FloatRange>();
            foreach (var frng in knownFloatRanges)
            {
                floatRange.Add(frng.Key, new FloatRange(knownFloatRanges[frng.Key]));
            }

            if (useKnown == false)
            {
                foreach (Feature f in features)
                {
                    if (f.FeatureType == FeatureType.INT)
                    {
                        if (f.Range.intRange.DoesOtherRangeOverlapEntirely(intRange[f.Name]))
                        {
                            // If it does, doesn't really matter because we know everything about this
                            generatedFeatures.Add(f.Name, new Tuple<int[], float[]>(f.Range.intRange.Pick(count), Array.Empty<float>()));
                        }
                        else
                        {
                            generatedFeatures.Add(f.Name, new Tuple<int[], float[]>(f.Range.intRange.Pick(intRange[f.Name], count), Array.Empty<float>()));
                        }
                    }
                    else if (f.FeatureType == FeatureType.FLOAT)
                    {
                        if (f.Range.floatRange.DoesOtherRangeOverlapEntirely(floatRange[f.Name]))
                        {
                            // If it does, doesn't really matter because we know everything about this
                            generatedFeatures.Add(f.Name, new Tuple<int[], float[]>(Array.Empty<int>(), f.Range.floatRange.Pick(count)));
                        }
                        else
                        {
                            generatedFeatures.Add(f.Name, new Tuple<int[], float[]>(Array.Empty<int>(), f.Range.floatRange.Pick(floatRange[f.Name], count)));
                        }
                    }
                }
            }
            else
            {
                foreach (Feature f in features)
                {
                    if (f.FeatureType == FeatureType.INT)
                    {
                        generatedFeatures.Add(f.Name, new Tuple<int[], float[]>(intRange[f.Name].Pick(count), Array.Empty<float>()));
                    }
                    else if (f.FeatureType == FeatureType.FLOAT)
                    {
                        generatedFeatures.Add(f.Name, new Tuple<int[], float[]>(Array.Empty<int>(), floatRange[f.Name].Pick(count)));
                    }
                }
            }

            return generatedFeatures;
        }
    }
}
