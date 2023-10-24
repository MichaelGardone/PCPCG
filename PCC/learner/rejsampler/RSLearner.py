import utility.RingBuffer as RingBuffer
import learner.Learner as Learner

import numpy as np

class RSLearner(Learner.Learner):
    def __init__(self, features=[], labels=2, history_size=0, neg_e = 0.2, pos_e = 0.2, nue_e = 0.2):
        self.__limits = dict(map(lambda x: (x.name(), (x.ranges(), x.get_children())), features))

        self.__neg_e = neg_e
        self.__pos_e = pos_e
        self.__nue_e = nue_e

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

    def generate_sample(self):
        sample = {}
        
        for key in self.__limits.keys():
            limits = self.__limits[key]
            print(key, limits)
        ##

        return sample
    ##

    def rate_sample(self, sample):
        pass
    ##
##
