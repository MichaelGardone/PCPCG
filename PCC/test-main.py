import argparse, os

import utility.Feature as Feature
import learner.rejsampler.RSLearner as RS
import utility.SynthHuman as SynthHuman

import utility.RandUtil as RandUtil

def main(args):
    # featureList = Feature.make_features("data/test-simple.jsonl")

    # featureList = Feature.make_features("data/test.jsonl")

    # for f in featureList:
    #     print(f)

    # rs = RS.RSLearner(features=featureList)

    # rs.generate_sample()

    print(RandUtil.rand_int_range([[10,400]], loff=0.2, hoff=1.2))
    print(RandUtil.rand_int_range([[0,3], [6,10], [15,20]], loff=0.2, hoff=1.2))
    print(RandUtil.rand_int_range([[10,400]], loff=0.8, hoff=1.1, n=3))
    print(RandUtil.rand_int_range([[0,3], [6,10], [15,20]], loff=0.2, hoff=1.2, n = 3))
    # print(RandUtil.randuni([[0,0.5]], loff=0.1, hoff=1.5))
    # print(RandUtil.randuni([[1, 5]], loff=0.1, hoff=1.5))
    # print(RandUtil.randuni([[1, 5], [7, 10]], hoff=0.1, high=1.5))
    # print(RandUtil.randuni([[1, 5]], loff=0.1, hoff=1.5, n = 2))

    # print(RandUtil.randint_mrange_from_ranges([[10,400]], 0.2, 0.3, 3))
    # print(RandUtil.randint_multi_mrange_from_ranges([[2,4],[6,8],[10,16], [30,40]], 0.2, 0.3, 3))
    # print(RandUtil.randfloat_mrange_from_ranges([[0,0.25]], 0.1, 0.2, 3))
    # print(RandUtil.randfloat_multi_mrange_from_ranges([[0,0.2],[0.3,0.4],[0.55,0.7]], 0.2, 0.3, 3))

    # broad_synth = SynthHuman.SynthHuman(features=featureList, gen_type=SynthHuman.GenerationType.BROAD)
    # broad_synth.generate_human(0.2, 1.5)
    # print(broad_synth)

    # narrow_synth = SynthHuman.SynthHuman(features=featureList, gen_type=SynthHuman.GenerationType.NARROW)
    # narrow_synth.generate_human()
    # print(narrow_synth)

    # ma_synth = SynthHuman.SynthHuman(features=featureList, gen_type=SynthHuman.GenerationType.MULTI_AREA)
    # ma_synth.generate_human()
    # print(ma_synth)

    # default_pacman = {"pellet":240, "power pellet":0.04, "cherries": 1}
    # default_pacman = {"pellet":240, "power pellet":4, "cherries": 1, "inky":1,  "blinky":1, "pinky":1, "clyde":1}
    
    # print(broad_synth.rate(default_pacman))
    # print(narrow_synth.rate(default_pacman))
    # print(ma_synth.rate(default_pacman))
##

if __name__ == "__main__":
    main(0)
##
