import argparse, os

import utility.Feature as Feature
import learner.rejsampler.RSLearner as RS
import utility.SynthHuman as SynthHuman

def main(args):
    featureList = Feature.make_features("data/test-simple.jsonl")

    # for f in featureList:
    #     print(f)

    # rs = RS.RSLearner(features=featureList)

    # rs.generate_sample()

    no_neutrality = SynthHuman.SynthHuman(features=featureList)
    no_neutrality.generate_human()
    print(no_neutrality)

    default_pacman = {"pellets":240, "power pellets":4, "cherries": 1}
    # default_pacman = {"pellets":240, "power pellets":4, "cherries": 1, "ghosts":4}
##

if __name__ == "__main__":
    main(0)
##
