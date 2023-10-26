import utility.RingBuffer as RingBuffer
import learner.Learner as Learner
import utility.RandUtil as RandUtil

import numpy as np

class RSLearner():
    def __init__(self, features=[], labels=2, history_size=0, neg_e = 0.2, pos_e = 0.2, nue_e = 0.2, sigfigs=3):
        self.__limits = dict(map(lambda x: (x.name(), x), features))

        self.__neg_e = neg_e
        self.__pos_e = pos_e
        self.__nue_e = nue_e

        self.__sigfigs = sigfigs

        assert(labels > 3 or labels < 2, "Rejection Sampler can't have less than two or more than three labels!")
        self.__num_labels = labels
        
        if type(history_size) in [tuple, list]:
            self.__multi_history = True
            
            if self.__num_labels != len(history_size) and len(history_size) != 1:
                raise Exception("Missing information for the history!")
            ##

            if len(history_size > 1):
                for i in range(self.__num_labels):
                    if history_size[i] <= 0:
                        self.__history[i] = []
                    else:
                        self.__history[i] = RingBuffer(history_size[i])
                    ##
                ##
            else:
                for i in range(self.__num_labels):
                    if history_size[i] <= 0:
                        self.__history[i] = []
                    else:
                        self.__history[i] = RingBuffer(history_size)
                    ##
                ##
            ##
        else:
            self.__multi_history = False
            if history_size <= 0:
                self.__history = []
            else:
                self.__history = RingBuffer(history_size)
            ##
        ##
    ##
    
    def record_sample(self, sample, player_label):
        if self.__multi_history:
            self.__history[player_label].append(sample)
        else:
            s = [sample, player_label]
            self.__history.append(s)
        ##
    ##

    def generate_sample(self, sample_type=Learner.SampleType.LEARN, scount=1):
        samples = []
        
        picked = {}
        for key in self.__limits.keys():
            limits = self.__limits[key].ranges()
            ftype = self.__limits[key].type()

            selected = 0
            if ftype is int:
                selected = RandUtil.rand_int(limits, scount)
            else:
                selected = RandUtil.rand_float(limits, scount, self.__sigfigs)
            picked[key] = selected
        ##

        # Fix so it's a list of dictionaries
        for i in range(scount):
            samples.append(dict(map(lambda x: (x, picked[x][i]), picked)))
        ##

        return samples
    ##

    def rate_sample(self, sample):
        pass
    ##
##
