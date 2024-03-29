namespace SyntheticHumans
{
    public enum PreferenceStyle
    {
        NARROW,
        BROAD,
        MULTI_AREA,
        UNKNOWN
    }

    public class SynthHuman
    {
        protected static int SYNTH_ID = 0;
        protected int id;
        public int ID {  get { return id; } }

        protected int toleranceForBad;
        public int ToleranceForBadLevels {  get { return toleranceForBad; } }
        protected int currBadCount = 0;

        public SynthHuman(PreferenceStyle preference, int tolerance = -1, int maxNumRanges = 3)
        {


            id = SYNTH_ID;
            SYNTH_ID++;
        }


    }
}
