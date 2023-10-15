import json, itertools

class Feature():
    def __init__(self, *anargs, **kwargs) -> None:
        # args -- tuple of anonymous arguments
        # kwargs -- dictionary of named arguments
        self.__name = kwargs.get('name')
        self.__ranges = kwargs.get('ranges')
        self.__composite = kwargs.get('composite')
        self.__children = []

        if kwargs.get('children'):
            assert(self.__composite)
            self.__children = kwargs.get('children')
        ##
    ##

    def name(self):
        return self.__name
    ##

    def ranges(self):
        return self.__ranges
    ##

    def add_child(self, child):
        self.__children.append(child)
    ##

    def get_child(self, i):
        return self.__children[i]
    ##

    def get_children(self):
        return self.__children
    ##

    def __str__(self) -> str:
        s = self.__name + ": "
        for itm in range(len(self.__ranges)):
            s += str(self.__ranges[itm])
            if itm + 1 < len(self.__ranges):
                s += ", "
            ##
        ##

        if self.__composite:
            s += "\n\t"
            s += ', '.join(map(lambda x: x.name(), self.__children))
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
            parent = result["parent"] if "parent" in result.keys() else None
            composite = result["composite"] if "composite" in result.keys() else False

            if parent == None:
                f = None
                if name in c2p.keys():
                    f = Feature(name=name, ranges=ranges, composite=composite, children=c2p[name])
                    del c2p[name]
                else:
                    f = Feature(name=name, ranges=ranges, composite=composite)
                ##
                features.append(f)
            else:
                indx = next((i for i, item in enumerate(features) if item.name() == parent), -1)

                if indx > -1:
                    features[indx].add_child(Feature(name=name, ranges=ranges, composite=composite))
                else:
                    if parent in c2p.keys():
                        c2p[parent].append(Feature(name=name, ranges=ranges, composite=composite))
                    else:
                        c2p[parent] = [Feature(name=name, ranges=ranges, composite=composite)]
                    ##
                ##
            ##
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
