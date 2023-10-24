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

    def __init__(self, features=[], gen_type = GenerationType.BROAD, tolerance = -1, max_num_ranges=3, sigfigs=3) -> None:
        self.__id = SynthHuman.AVAILABLE_ID
        SynthHuman.AVAILABLE_ID += 1
        self.__ratings = {"s":0, "n":0, "f":0}

        self.__gen_type = gen_type
        self.__features = features
        self.__tolerance = tolerance
        self.__in_row_bad = 0
        self.__sigfigs = sigfigs

        if gen_type == GenerationType.MULTI_AREA:
            assert(max_num_ranges > 1, "Need to have more than 1 area available! Otherwise, use BROAD or NARROW.")
        ##

        self.__max_num_ranges = max_num_ranges

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
    
    def generate_human(self, low_mod=0.2, high_mod=0.3) -> tuple:
        self.generate_preferences(low_mod, high_mod)
        self.generate_hates()
        return (self.__preferences, self.__hates)
    ##

    def generate_preferences(self, low_mod=0.2, high_mod=0.3) -> None:
        if self.__gen_type == GenerationType.BROAD:
            # ll = 0.8
            # hl = 0.5
            self._generate_broad_prefs(low_mod, high_mod)
        elif self.__gen_type == GenerationType.NARROW:
            # ll = 0.2
            # hl = 0.3
            self._generate_narrow_prefs(low_mod, high_mod)
        elif self.__gen_type == GenerationType.MULTI_AREA:
            # ll = 0.2
            # hl = 0.3
            self._generate_multi_area_prefs(low_mod, high_mod, np.random.randint(2, self.__max_num_ranges + 1))
        else:
            raise Exception("Error: unknown generation type!")
        ##
    ##

    def generate_hates(self) -> None:
        pass
    ##

    def rate(self, sample) -> bool:
        pass
    ##

    def is_player_done(self) -> bool:
        return self.__tolerance < self.__in_row_bad if self.__tolerance > 0 else False
    ##

    def get_ratings(self) -> dict:
        return self.__ratings
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

    def _generate_broad_prefs(self, ll, hl):
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
                if len(limits) == 1:
                    rnd = RandUtil.randfloat_srange_from_ranges(limits, ll, hl, self.__sigfigs)
                else:
                    rnd = RandUtil.randfloat_mrange_from_ranges(limits, ll, hl, self.__sigfigs)
            else:
                if len(limits) == 1:
                    rnd = RandUtil.randint_srange_from_ranges(limits, ll, hl)
                else:
                    rnd = RandUtil.randint_mrange_from_ranges(limits, ll, hl)
                ##
            ##

            # print(f"{f.name()}: {rnd}")
            self.__preferences[f.name()] = rnd
        ##
    ##

    def _generate_narrow_prefs(self, ll, hl):
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
                if len(limits) == 1:
                    rnd = RandUtil.randfloat_narrow_srange_from_ranges(limits, ll, hl, self.__sigfigs)
                else:
                    rnd = RandUtil.randfloat_narrow_mrange_from_ranges(limits, ll, hl, self.__sigfigs)
            else:
                if len(limits) == 1:
                    rnd = RandUtil.randint_narrow_srange_from_ranges(limits, ll, hl)
                else:
                    rnd = RandUtil.randint_narrow_mrange_from_ranges(limits, ll, hl)
                ##
            ##

            # print(f"{f.name()}: {rnd}")
            self.__preferences[f.name()] = rnd
        ##
    ##

    def _generate_multi_area_prefs(self, ll, hl, n):
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
                if len(limits) == 1:
                    rnd = RandUtil.randfloat_multiarea_from_ranges(limits, ll, hl, n, self.__sigfigs)
                else:
                    rnd = RandUtil.randfloat_multi_mrange_from_ranges(limits, ll, hl, n, self.__sigfigs)
            else:
                if len(limits) == 1:
                    rnd = RandUtil.randint_multiarea_from_ranges(limits, ll, hl, n)
                else:
                    rnd = RandUtil.randint_multi_mrange_from_ranges(limits, ll, hl, n)
                ##
            ##

            # print(f"{f.name()}: {rnd}")
            self.__preferences[f.name()] = rnd
        ##
    ##
##