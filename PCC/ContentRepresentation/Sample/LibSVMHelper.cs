using LibSVMsharp;
using PCC.ContentRepresentation.Features;
using System;
using System.Collections.Generic;

namespace PCC.ContentRepresentation.Sample
{
    public static class LibSVMHelper
    {

        public static SVMNode[] SampleToSVMNode(Sample sample)
        {
            List<SVMNode> nodeComponents = new List<SVMNode>();

            for (int i = 0; i < sample.GetFeatures().Count; i++)
            {
                SVMNode node = new SVMNode
                {
                    Index = i
                };

                Tuple<FeatureType, SampleValue> s = sample.GetSampleValue(sample.GetFeatures()[i]);
                switch (s.Item1)
                {
                    case FeatureType.INT:
                        node.Value = s.Item2.intVal;
                        break;
                    case FeatureType.FLOAT:
                        node.Value = s.Item2.floatVal;
                        break;
                }

                nodeComponents.Add(node);
            }

            return nodeComponents.ToArray();
        }

        public static Sample SVMNodeToSample(List<Feature> features, SVMNode[] nodeComponents)
        {
            Sample sample = new Sample();

            foreach(SVMNode node in nodeComponents)
            {
                Feature f = features[node.Index];

                switch (f.FeatureType)
                {
                    case FeatureType.INT:
                        sample.AddValue(f.Name, (int)node.Value);
                        break;
                    case FeatureType.FLOAT:
                        sample.AddValue(f.Name, (float)node.Value);
                        break;
                }

            }

            return sample;
        }

    }
}
