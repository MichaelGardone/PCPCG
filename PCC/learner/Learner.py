import structures.RingBuffer as RingBuffer

class Learner():
    def __init__(self, features=[], labels=2, history_size=0):
        self.__limits = dict(map(lambda x: (x.name(), x.ranges()), features))

        if type(history_size) is tuple:
            self.__multi_history = True
            
            if len(labels) != len(history_size):
                raise Exception("Missing information for the history!")
            ##

            for i in range(len(labels)):
                pass
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
        pass
    ##

    def generate_sample(self):
        pass
    ##

    def rate_sample(self, sample):
        pass
    ##
##
