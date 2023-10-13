class RingBuffer():
    def __init__(self, size_limit) -> None:
        self.__data = []
        self.__dindexer = 0
        self.__count = 0
        self.__size = size_limit
    ##

    def append(self, data):
        if len(self.__data) == self.__size:
            self.__data[self.__dindexer] = data
            
            self.__dindexer = self.__dindexer + 1 if self.__dindexer < self.__size else 0
        else:
            self.__data.append(data)
        ##
    ##

    def clear(self):
        self.__data = []
        self.__dindexer = 0
        self.__count = 0
    ##

    def __getitem__(self, key):
        return self.__data[key % self.__count]
    ##
##
