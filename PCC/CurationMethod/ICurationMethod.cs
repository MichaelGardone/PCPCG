using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PCC.CurationMethod
{
    public enum SampleGenerationMethod
    {
        SAFE_ONLY = 0,
        UNSAFE_ONLY,
        RANDOM,
        RANDOM_FROM_KNOWNS,
        RANDOM_FROM_UNKNOWNS
    }

    public interface ICurationMethod
    {
        public void RecordSample();

        /// <summary>
        /// Generate a sample from the SampleGenerationMethod enums.
        /// </summary>
        public void GenerateSample(int count = 1, SampleGenerationMethod method = SampleGenerationMethod.RANDOM);

        /// <summary>
        /// Generate a sample based on a known sample, and modifying selected parameters from SampleGenerationMethod.
        /// </summary>
        public void GeneratePickedSample(int count = 1, SampleGenerationMethod method = SampleGenerationMethod.RANDOM);

        public void ClearMemory();
    }
}
