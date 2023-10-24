import utility.RingBuffer as RingBuffer
import learner.Learner as Learner

import numpy as np

class SVMLeaner(Learner.Learner):
    def __init__(self, features=[], labels=2, history_size=0) -> None:
        self.__limits = dict(map(lambda x: (x.name(), (x.ranges(), x.get_children())), features))
    ##
##
