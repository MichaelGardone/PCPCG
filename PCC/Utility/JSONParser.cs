using PCC.ContentRepresentation.Features;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace PCC.Utility
{
    public static class JSONParser
    {

        public static Dictionary<string, List<Feature>> ReadFeatures(string pathToFile)
        {
            Dictionary<string, List<Feature>> features = new Dictionary<string, List<Feature>>();
            
            string json = File.ReadAllText(pathToFile);
            List<IntermediateFeature> intermediateFeatures = JsonSerializer.Deserialize<List<IntermediateFeature>>(json)!;

            foreach (IntermediateFeature intermediateFeature in intermediateFeatures)
            {

            }

            return features;
        }

        protected class IntermediateFeature
        {
            public string name { get; set; }
            public List<int[]> intRanges { get; set; }
            public List<float[]> floatRanges { get; set; }
        }

    }
}
