import json, itertools

class Feature():
    def __init__(self, *anargs, **kwargs) -> None:
        # args -- tuple of anonymous arguments
        # kwargs -- dictionary of named arguments
        self.__name = kwargs.get('name')
        self.__ranges = kwargs.get('ranges')

        match kwargs.get('type'):
            case "int":
                self.__type = int
            case "float":
                self.__type = float
            case "toggle":
                self.__type = bool
            case "bool":
                self.__type = bool
            case None:
                self.__type = kwargs.get('type')
            ##
        ##
    ##

    def name(self):
        return self.__name
    ##

    def ranges(self):
        return self.__ranges
    ##

    def type(self):
        return self.__type
    ##

    def add_child(self, child):
        self.__children.append(child)
    ##

    def get_child(self, i):
        return self.__children[i]
    ##

    def __str__(self) -> str:
        s = self.__name + ": "
        for itm in range(len(self.__ranges)):
            s += str(self.__ranges[itm])
            if itm + 1 < len(self.__ranges):
                s += ", "
            ##
        ##
        return s
    ##
##

def make_features(path) -> list:
    features = []
    with open(path, 'r') as file:
        lst = list(file)

        c2p = {}

        for itm in lst:
            result = json.loads(itm)
            name = result["name"]
            ranges = result["ranges"] if "ranges" in result.keys() else []
            feat_type = result["type"] if "type" in result.keys() else None

            features.append(Feature(name=name, ranges=ranges, type=feat_type))
        ##

        for key in c2p.keys():
            indx = next((i for i, item in enumerate(features) if item.name() == key), -1)
            for itm in c2p[key]:
                features[indx].add_child(itm)
            ##
        ##
    ##
    return features
##
