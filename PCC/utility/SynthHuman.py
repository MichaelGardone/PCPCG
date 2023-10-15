from enum import Enum

import numpy as np
import utility.RandUtil as RandUtil

class GenerationType(Enum):
    MULTI_AREA = 0
    BROAD = 1
    NARROW = 2
##

class SynthHuman:
    AVAILABLE_ID = 1

    def __init__(self, features=[], gen_type = GenerationType.BROAD, tolerance = -1) -> None:
        self.__id = SynthHuman.AVAILABLE_ID
        SynthHuman.AVAILABLE_ID += 1
        self.__ratings = {"s":0, "n":0, "f":0}

        self.__gen_type = gen_type
        self.__features = features
        self.__tolerance = tolerance
        self.__in_row_bad = 0

        self.__preferences = {}
        self.__hates = {}

        for f in features:
            self.__preferences[f.name()] = 0
            if len(f.get_children()) > 0:
                for child in f.get_children():
                    self.__preferences[child.name()] = 0
                ##
            ##
        ##
    ##

    def is_neutral(self):
        return self.__allow_neutral
    ##
    
    def generate_human(self) -> tuple:
        if self.__allow_neutral:
            self.generate_preferences()
            self.generate_hates()
            return (self.__preferences, self.__hates)
        else:
            self.generate_preferences()
            return (self.__preferences)
        ##
    ##

    def generate_preferences(self):
        for f in self.__features:
            limits = f.ranges()
            children = f.get_children()

            is_float = False
            for l in limits:
                if isinstance(l[0], float) or isinstance(l[1], float):
                    is_float = True
                    break
                ##
            ##

            rnd = 0
            if is_float:
                rnd = RandUtil.randfloat_srange_from_ranges(limits, 0.8, 0.5)
            else:
                if len(limits) == 1:
                    rnd = RandUtil.randint_srange_from_ranges(limits, 0.8, 0.5)
                else:
                    rnd = RandUtil.randint_mrange_from_ranges(limits, 0.8, 0.5)
                ##
            ##

            print(f"{f.name()}: {rnd}")
            self.__preferences[f.name()] = rnd
        ##
    ##

    def generate_hates(self) -> list:
        return []
    ##

    def rate(self, sample) -> bool:
        pass
    ##

    def is_player_done(self) -> bool:
        return self.__tolerance < self.__in_row_bad if self.__tolerance > 0 else False
    ##

    def __str__(self) -> str:
        out = f"###########\nSynthHuman {self.__id}\n"

        style = "UNKNOWN"

        if self.__gen_type == GenerationType.BROAD:
            style = "BROAD"
        elif self.__gen_type == GenerationType.NARROW:
            style = "NARROW"
        elif self.__gen_type == GenerationType.MULTI_AREA:
            style = "MULTI AREA"
        # otherwise, we don't know

        out += f"Preference Style: { style }\n"

        out += "\n#-#-#-#\n"
        out += f"Current Preferences (n={len(self.__features)}):\n"

        for p in self.__preferences:
            out += f"{p} range:\n\t{self.__preferences[p]}\n"
        ##

        out += "#-#-#-#"

        out += "\n###########"
        return out
    ##
##
