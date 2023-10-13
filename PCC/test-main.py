import argparse, os

import structures.Feature as Feature
import learner.rejsampler.RSLearner as RS

def main(args):
    featureList = Feature.make_features("data/test.jsonl")
    rs = RS.RSLearner(features=featureList)

    rs.generate_sample()

    print(rs.__multi_history)
    print(rs._Learner__multi_history)
##

if __name__ == "__main__":
    main(0)
##
