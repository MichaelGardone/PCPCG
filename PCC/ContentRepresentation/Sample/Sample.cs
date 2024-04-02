using PCC.ContentRepresentation.Features;
using System.Runtime.InteropServices;
using System.Text;

namespace PCC.ContentRepresentation.Sample
{
    [StructLayout(LayoutKind.Explicit)]
    public struct SampleValue
    {
        [FieldOffset(0)]
        public int intVal;

        [FieldOffset(0)]
        public float floatVal;
    }

    public class Sample
    {
        protected Dictionary<string, Tuple<FeatureType, int>> features;
        protected List<int> intList;
        protected List<float> floatList;

        public Tuple<SampleValue, FeatureType>? this[string feature]
        {
            get
            {
                if (features.ContainsKey(feature) == false)
                {
                    return null;
                }

                if (features[feature].Item1 == FeatureType.INT)
                {
                    return new Tuple<SampleValue, FeatureType>(new SampleValue() { intVal = intList[features[feature].Item2] }, FeatureType.INT);
                }
                else if (features[feature].Item1 == FeatureType.FLOAT)
                {
                    return new Tuple<SampleValue, FeatureType>(new SampleValue() { floatVal = floatList[features[feature].Item2] }, FeatureType.FLOAT);
                }

                return null;
            }
        }

        public Sample()
        {
            features = new Dictionary<string, Tuple<FeatureType, int>>();
            intList = new List<int>();
            floatList = new List<float>();
        }

        public Sample(HistoricSample historicSample)
        {
            if (historicSample == null)
                throw new ArgumentNullException("Historic sample doesn't exist?");

            features = historicSample.GetFeatures();
            intList = new List<int>();
            floatList = new List<float>();

            foreach (string key in features.Keys)
            {
                Tuple<SampleValue, SampleValue> extents = historicSample[key]!;

                switch (features[key].Item1)
                {
                    case FeatureType.INT:
                        intList.Add((extents.Item1.intVal + extents.Item2.intVal) / 2);
                        features[key] = new Tuple<FeatureType, int>(FeatureType.INT, intList.Count - 1);
                        break;
                    case FeatureType.FLOAT:
                        floatList.Add((extents.Item1.floatVal + extents.Item2.floatVal) / 2);
                        features[key] = new Tuple<FeatureType, int>(FeatureType.FLOAT, floatList.Count - 1);
                        break;
                }
            }
        }

        public void AddValue(string feature, int value)
        {
            intList.Add(value);
            features.Add(feature, new Tuple<FeatureType, int>(FeatureType.INT, intList.Count - 1));
        }

        public void AddValue(string feature, float value)
        {
            floatList.Add(value);
            features.Add(feature, new Tuple<FeatureType, int>(FeatureType.FLOAT, floatList.Count - 1));
        }

        public List<string> GetFeatures()
        {
            return features.Keys.ToList();
        }

        public Sample? MergeSamples(Sample otherSample)
        {
            if (otherSample == null) return null;

            Sample sample = new Sample();



            return sample;
        }

        public Tuple<FeatureType, SampleValue> GetSampleValue(string key)
        {
            Tuple<FeatureType, int> info = features[key];

            switch (info.Item1)
            {
                case FeatureType.INT:
                    return new Tuple<FeatureType, SampleValue>(info.Item1, new SampleValue() { intVal = intList[info.Item2] });
                case FeatureType.FLOAT:
                    return new Tuple<FeatureType, SampleValue>(info.Item1, new SampleValue() { floatVal = floatList[info.Item2] });
                case FeatureType.DOUBLE:
                    throw new NotImplementedException("PCC does not currently support type Double samples.");
                case FeatureType.LONG:
                    throw new NotImplementedException("PCC does not currently support type Long samples.");
                case FeatureType.BOOLEAN:
                    throw new NotImplementedException("PCC does not currently support type Boolean samples.");
                case FeatureType.RULE:
                    throw new NotImplementedException("PCC does not currently support type Rule samples.");
                default:
                    throw new ArgumentException("PCC does not know what this feature type is... How did you get this far?");
            }
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();

            sb.Append("(");

            foreach(var feature in features)
            {
                switch(feature.Value.Item1)
                {
                    case FeatureType.INT:
                        sb.Append($"({feature.Key}: {intList[feature.Value.Item2]}),");
                        break;
                    case FeatureType.FLOAT:
                        sb.Append($"({feature.Key}: {floatList[feature.Value.Item2]}),");
                        break;
                }
            }

            sb.Length--;
            sb.Append(")");

            return sb.ToString();
        }
    }

    public class HistoricSample
    {
        protected Dictionary<string, Tuple<FeatureType, int>> features;
        protected List<Tuple<int, int>> intList;
        protected List<Tuple<float, float>> floatList;

        public Tuple<SampleValue, SampleValue>? this[string feature]
        {
            get
            {
                if (features.ContainsKey(feature) == false)
                {
                    return null;
                }

                if (features[feature].Item1 == FeatureType.INT)
                {
                    return new Tuple<SampleValue, SampleValue> (
                        new SampleValue() { intVal = intList[features[feature].Item2].Item1 },
                        new SampleValue() { intVal = intList[features[feature].Item2].Item2 }
                        );
                }
                else if (features[feature].Item1 == FeatureType.FLOAT)
                {
                    return new Tuple<SampleValue, SampleValue>(
                        new SampleValue() { floatVal = floatList[features[feature].Item2].Item1 },
                        new SampleValue() { floatVal = floatList[features[feature].Item2].Item2 }
                        );
                }

                return null;
            }
        }

        public HistoricSample(Feature[] archetypes, Sample sample, float change, int sigfigs = 3)
        {
            this.features = new Dictionary<string, Tuple<FeatureType, int>>();
            intList = new List<Tuple<int, int>>();
            floatList = new List<Tuple<float, float>>();

            foreach (string key in sample.GetFeatures())
            {
                Tuple<SampleValue, FeatureType> type = sample[key]!;

                switch (type.Item2)
                {
                    case FeatureType.INT:
                        int intVal = type.Item1.intVal;

                        // Will never be null
                        Tuple<int, int> extents = null;
                        for (int i = 0; i < archetypes.Length; i++)
                        {
                            if (archetypes[i].Name == key)
                            {
                                extents = archetypes[i].Range.intRange.WhichRangeHasValue(intVal)!;
                                break;
                            }
                        }

                        int min = (int)Math.Floor(intVal - intVal * change);
                        int max = (int)Math.Ceiling(intVal + intVal * change);

                        intList.Add(new Tuple<int, int>(
                                Math.Max(min, extents!.Item1),
                                Math.Min(max, extents!.Item2)
                            )
                        );
                        this.features.Add(key, new Tuple<FeatureType, int>(type.Item2, intList.Count - 1));
                        break;
                    case FeatureType.FLOAT:
                        float floatVal = type.Item1.floatVal;

                        Tuple<float, float> extentsF = null;
                        for(int i = 0; i < archetypes.Length; i++)
                        {
                            if (archetypes[i].Name == key)
                            {
                                extentsF = archetypes[i].Range.floatRange.WhichRangeHasValue(floatVal)!;
                                break;
                            }
                        }
                        float minF = MathF.Round(floatVal - floatVal * change, sigfigs);
                        float maxF = MathF.Round(floatVal + floatVal * change, sigfigs);

                        floatList.Add(
                            new Tuple<float, float>(
                                MathF.Max(minF, extentsF!.Item1),
                                MathF.Min(maxF, extentsF!.Item2)
                            )
                        );
                        this.features.Add(key, new Tuple<FeatureType, int>(type.Item2, floatList.Count - 1));
                        break;
                }
            }
        }

        public HistoricSample(List<Feature> archetypes, Sample sample, float change, int sigfigs = 3)
        {
            this.features = new Dictionary<string, Tuple<FeatureType, int>>();
            intList = new List<Tuple<int, int>>();
            floatList = new List<Tuple<float, float>>();

            foreach (string key in sample.GetFeatures())
            {
                Tuple<SampleValue, FeatureType> type = sample[key]!; 
                
                switch (type.Item2)
                {
                    case FeatureType.INT:
                        int intVal = type.Item1.intVal;
                        
                        // Will never be null
                        Tuple<int, int> extents = archetypes.Find(x => x.Name == key)!.Range.intRange.WhichRangeHasValue(intVal)!;
                        int min = (int)Math.Floor(intVal - intVal * change);
                        int max = (int)Math.Ceiling(intVal + intVal * change);

                        intList.Add(new Tuple<int, int>(
                                Math.Max(min, extents.Item1),
                                Math.Min(max, extents.Item2)
                            )
                        );
                        this.features.Add(key, new Tuple<FeatureType, int>(type.Item2, intList.Count - 1));
                        break;
                    case FeatureType.FLOAT:
                        float floatVal = type.Item1.floatVal;

                        Tuple<float, float> extentsF = archetypes.Find(x => x.Name == key)!.Range.floatRange.WhichRangeHasValue(floatVal)!;
                        float minF = MathF.Round(floatVal - floatVal * change, sigfigs);
                        float maxF = MathF.Round(floatVal + floatVal * change, sigfigs);

                        floatList.Add(
                            new Tuple<float, float>(
                                MathF.Max(minF, extentsF.Item1),
                                MathF.Min(maxF, extentsF.Item2)
                            )
                        );
                        this.features.Add(key, new Tuple<FeatureType, int>(type.Item2, floatList.Count - 1));
                        break;
                }
            }
        }

        public HistoricSample(Dictionary<string, Tuple<FeatureType, int>> features, List<int> sampleInts, List<float> sampleFloats,
            float change, int sigfigs=3)
        {
            this.features = features;

            intList = new List<Tuple<int, int>>();

            foreach(int sample in sampleInts)
            {
                intList.Add(new Tuple<int, int>((int)Math.Floor(sample - sample * change), (int)Math.Ceiling(sample + sample * change)));
            }

            floatList = new List<Tuple<float, float>>();
            foreach (float sample in sampleFloats)
            {
                floatList.Add(
                    new Tuple<float, float>(
                        MathF.Round(sample - sample * change, sigfigs),
                        MathF.Round(sample + sample * change, sigfigs)
                    )
                );
            }
        }

        public Dictionary<string, Tuple<FeatureType, int>> GetFeatures()
        {
            return features;
        }
    }
}
