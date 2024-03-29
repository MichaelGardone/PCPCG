using PCC.ContentRepresentation.Features;
using PCC.ContentRepresentation.Sample;

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
        public void RecordSample(Sample sample, int label);

        /// <summary>
        /// Generate a sample from the SampleGenerationMethod enums.
        /// </summary>
        /// <returns>A list of samples; this will never be null. If no samples are generated, then the list will return
        /// a size of 0.</returns>
        public List<Sample> GenerateSamples(int count = 1, SampleGenerationMethod method = SampleGenerationMethod.RANDOM);

        /// <summary>
        /// Clear out the backing memory of the curation method.
        /// </summary>
        public void ClearMemory();

        /// <summary>
        /// Get the labels associated with the curation method.
        /// </summary>
        public List<int> GetLabels();

        /// <summary>
        /// Get the features associated with the curation method.
        /// </summary>
        public List<Feature> GetFeatures();


        public Sample? GetRandomSample();

        public Sample? GetSample(int label, int sampleIndex);
    }
}
