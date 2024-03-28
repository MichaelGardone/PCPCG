using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PCC.CurationMethod
{
    public enum SampleGenerationMethod
    {
        SAFE_ONLY = 0,
        UNSAFE_ONLY,
        RANDOM,
        RANDOM_FROM_KNOWNS,
        RANDOM_FROM_UNKNOWNS,
        REUSE
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

        /// <summary>
        /// Clear out the backing memory of the curation method.
        /// </summary>
        public void ClearMemory();

        /// <summary>
        /// Get the labels associated with the curation method.
        /// </summary>
        public void GetLabels();

        /// <summary>
        /// Get the features associated with the curation method.
        /// </summary>
        public void GetFeatures();
    }
}
