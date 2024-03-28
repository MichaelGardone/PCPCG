using PCC.Utility.Range;
using System.Runtime.InteropServices;

namespace PCC.ContentRepresentation.Features
{
    [StructLayout(LayoutKind.Explicit)]
    public struct FeatureRange
    {
        [FieldOffset(0)]
        public IntRange intRange;

        [FieldOffset(0)]
        public FloatRange floatRange;
    }

    public class Feature
    {
        public string Name { get; protected set; }
        public FeatureType FeatureType { get; protected set; }

        protected FeatureRange range;
        public FeatureRange Range { get { return range; } }

        public Feature(string name, IntRange range)
        {
            Name = name;
            FeatureType = FeatureType.INT;
            this.range.intRange = range;
        }

        public Feature(string name, FloatRange range)
        {
            Name = name;
            FeatureType = FeatureType.FLOAT;
            this.range.floatRange = range;
        }
    }
}
