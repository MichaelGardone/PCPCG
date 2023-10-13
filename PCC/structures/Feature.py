import json, itertools

class Feature():
    def __init__(self, *args, **kwargs) -> None:
        # args -- tuple of anonymous arguments
        # kwargs -- dictionary of named arguments
        self.__name = kwargs.get('name')
        self.__ranges = kwargs.get('ranges')
    ##

    def name(self):
        return self.__name
    ##

    def ranges(self):
        return self.__ranges
    ##

    def __str__(self) -> str:
        s = self.__name + ": "
        for itm in range(len(self.__ranges)):
            s += str(self.__ranges[itm])
            if itm + 1 < len(self.__ranges):
                s += ", "
        ##
        return s
    ##
##

def make_features(path) -> list:
    features = []
    with open(path, 'r') as file:
        lst = list(file)

        for itm in lst:
            result = json.loads(itm)
            name = result["name"]
            ranges = result["ranges"]
            features.append(Feature(name=name, ranges=ranges))
        ##
    ##
    return features
##
