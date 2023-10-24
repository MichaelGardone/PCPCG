import argparse, os

import utility.Feature as Feature
import learner.rejsampler.RSLearner as RS
import utility.SynthHuman as SynthHuman

import utility.RandUtil as RandUtil

def main(args):
    featureList = Feature.make_features("data/test-simple.jsonl")

    # for f in featureList:
    #     print(f)

    # rs = RS.RSLearner(features=featureList)

    # rs.generate_sample()

    # print(RandUtil.randint_mrange_from_ranges([[10,400]], 0.2, 0.3, 3))
    # print(RandUtil.randint_multi_mrange_from_ranges([[2,4],[6,8],[10,16], [30,40]], 0.2, 0.3, 3))
    # print(RandUtil.randfloat_mrange_from_ranges([[0,0.25]], 0.1, 0.2, 3))
    # print(RandUtil.randfloat_multi_mrange_from_ranges([[0,0.2],[0.3,0.4],[0.55,0.7]], 0.2, 0.3, 3))

    broad_synth = SynthHuman.SynthHuman(features=featureList, gen_type=SynthHuman.GenerationType.BROAD)
    broad_synth.generate_human(0.8, 0.5)
    print(broad_synth)

    narrow_synth = SynthHuman.SynthHuman(features=featureList, gen_type=SynthHuman.GenerationType.NARROW)
    narrow_synth.generate_human()
    print(narrow_synth)

    ma_synth = SynthHuman.SynthHuman(features=featureList, gen_type=SynthHuman.GenerationType.MULTI_AREA)
    ma_synth.generate_human()
    print(ma_synth)

    default_pacman = {"pellets":240, "power pellets":0.04, "cherries": 1}
    # default_pacman = {"pellets":240, "power pellets":4, "cherries": 1, "ghosts":4}
##

if __name__ == "__main__":
    main(0)
##
