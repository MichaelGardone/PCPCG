import structures.RingBuffer as RingBuffer
import learner.Learner as Learner

class RSLearner(Learner.Learner):
    def __init__(self, features=[], labels=["like", "dislike"], history_size=0, neg_e = 0.2, pos_e = 0.2):
        super().__init__(features, labels, history_size)
        self.__limits = self._Learner__limits
        self.__multi_history = self._Learner__multi_history

        self.__neg_e = neg_e
        self.__pos_e = pos_e
    ##
    
    def record_sample(self, sample, player_label):
        s = [sample, player_label]
        if self.__two_histories:
            if player_label:
                self.__pos_history.append(s)
            else:
                self.__neg_history.append(s)
        else:
            pass
        ##
    ##

    def generate_sample(self):
        sample = {}
        use_sample = False
        
        while use_sample == False:
            sample = {}

            for key in self.__limits.keys():
                limits = self.__limits[key]
                print(key, limits)
            ##

            use_sample = self.rate_sample(sample)
        ##

        return sample
    ##

    def rate_sample(self, sample):
        pass
    ##
##
