import utility.RingBuffer as RingBuffer
import learner.Learner as Learner
import utility.RandUtil as RandUtil

import numpy as np

class RSLearner():
    def __init__(self, features=[], labels=2, history_size=0, neg_e = 0.2, pos_e = 0.2, nue_e = 0.2, sigfigs=3):
        self.__limits = dict(map(lambda x: (x.name(), x), features))

        self.__neg_ep = neg_e
        self.__pos_ep = pos_e
        self.__nue_ep = nue_e

        self.__sigfigs = sigfigs

        assert labels == 3 or labels == 2, "Rejection Sampler can't have less than two or more than three labels!"
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
        rsample = {}
        
        if player_label == -1 or player_label == False:
            for key in sample:
                maxLim = 0
                for r in self.__limits[key].ranges():
                    maxLim = max(maxLim, r[1])
                
                offset = sample[key] * self.__neg_ep
                rsample[key] = (np.round(sample[key] - offset, self.__sigfigs) if sample[key] - offset > 0 else 0,
                                np.round(sample[key] + offset, self.__sigfigs) if sample[key] + offset > maxLim else maxLim)
        elif player_label == 1 or player_label == True:
            for key in sample:
                maxLim = 0
                for r in self.__limits[key].ranges():
                    maxLim = max(maxLim, r[1])
                
                offset = sample[key] * self.__neg_ep
                rsample[key] = (np.round(sample[key] - offset, self.__sigfigs) if sample[key] - offset > 0 else 0,
                                np.round(sample[key] + offset, self.__sigfigs) if sample[key] + offset > maxLim else maxLim)
        else:
            for key in sample:
                maxLim = 0
                for r in self.__limits[key].ranges():
                    maxLim = max(maxLim, r[1])
                
                offset = sample[key] * self.__neg_ep
                rsample[key] = (np.round(sample[key] - offset, self.__sigfigs) if sample[key] - offset > 0 else 0,
                                np.round(sample[key] + offset, self.__sigfigs) if sample[key] + offset > maxLim else maxLim)
            ##
        ##

        if self.__multi_history:
            self.__history[player_label].append(rsample)
        else:
            s = (rsample, player_label)
            self.__history.append(s)
        ##
    ##

    def generate_sample(self, sample_type=Learner.SampleType.LEARN, scount=1, unk_threshold=0.5):
        samples = []
        
        picked = None

        match sample_type:
            case Learner.SampleType.RANDOM:
                picked = self._generate_random_samples(scount)
            case Learner.SampleType.LEARN:
                picked = self._generate_learner_samples(unk_threshold, scount)
            case Learner.SampleType.SAFE:
                pass
            case Learner.SampleType.FAR:
                pass
            case _:
                raise Exception("ERROR: Unknown sample type requested")
            ##
        ##
        
        if picked == None:
            raise Exception("ERROR:: Cannot generate any samples for some reason!")
        ##

        # Fix so it's a list of dictionaries
        for i in range(scount):
            samples.append(dict(map(lambda x: (x, picked[x][i]), picked)))
        ##

        return samples
    ##

    def _generate_learner_samples(self, threshold, scount):
        # if len(self.__history) == 0:
        #     return self._generate_random_samples(scount)
        ##

        # Get all features as IDs
        keys = list(self.__limits.keys())
        fids = np.arange(len(keys))
        num_exploring = int(round(len(fids) * threshold))

        unk_fids = np.random.choice(fids, num_exploring, replace=False)
        print([keys[ufid] for ufid in unk_fids])

        for id in unk_fids:
            # Step 1: generate the known safe and unsafe zones for the feature
            learned_lims = []
            key = keys[id]
            if self.__multi_history:
                pass
            else:
                for lim in self.__history:
                    learned_lims.append(lim[0][key])
                ##
            ##
            print(learned_lims)
            # Step 2: subtract these zones from the limits
            intersectionless = []
            for lim in self.__limits[key].ranges():
                print(lim)
            ##

            # Step 3: generate random numbers from the new limits
            pass
        ##

    ##

    def _generate_random_samples(self, scount):
        picked = {}
        for key in self.__limits.keys():
            limits = self.__limits[key].ranges()
            ftype = self.__limits[key].type()

            selected = 0
            if ftype is int:
                selected = RandUtil.rand_int(limits, scount)
            elif ftype is float:
                selected = RandUtil.rand_float(limits, scount, self.__sigfigs)
            elif ftype is bool:
                selected = np.random.choice([0, 1])
            else:
                raise Exception(f"Unknown feature type: {key} -is-> {ftype}")
            
            picked[key] = selected
        ##
        return picked
    ##

    def rate_sample(self, sample):
        rating = 1

        # go through every 

        if rating == 1:
            return True
        if rating <= 0:
            return False

        return np.random.uniform(0, 1) < rating
    ##
##
