import sys
import numpy as np

def randint_from_ranges(ranges):
    choices = []
    for i in range(len(ranges)):
        choices.append(np.random.randint(ranges[i][0], ranges[i][1] + 1))
    ##
    return np.random.choice(choices)
##

def randfloat_from_ranges(ranges, sigfigs=2):
    choices = []
    for i in range(len(ranges)):
        choices.append(np.random.uniform(ranges[i][0], ranges[i][1]))
    ##
    return np.round(np.random.choice(choices), decimals=sigfigs)
##

def randint_srange_from_ranges(ranges, ll, hl):
    # collate into one number line
    low_limit = min(x[0] for x in ranges)
    high_limit = max(x[1] for x in ranges)
    
    lower = np.round(np.random.randint(low_limit, np.round(low_limit + low_limit * ll) + 2))
    upper = np.round(np.random.randint(np.round(high_limit - high_limit * hl), high_limit + 1))

    return [lower, upper]
##

def randint_mrange_from_ranges(ranges, ll, hl):
    # this function takes care of producing one long number line
    res = randint_srange_from_ranges(ranges, ll, hl)

    # we need to break it back up into being in line with the features
    results = []
    for rng in ranges:
        if res[0] == res[1] and res[0] < rng[1]:
            break
        ##

        start = max(res[0], rng[0])
        end = min(res[1], rng[1])
        t = [start, end]

        res[0] = end
        results.append(t)
    ##

    return results
##

def randfloat_srange_from_ranges(ranges, ll, hl, sigfigs=2):
    # collate into one number line
    low_limit = min(x[0] for x in ranges)
    high_limit = max(x[1] for x in ranges)
    
    lower = np.round(np.random.uniform(low_limit, np.round(low_limit + low_limit * ll)), decimals=sigfigs)
    upper = np.round(np.random.uniform(np.round(high_limit - high_limit * hl), high_limit), decimals=sigfigs)

    return [lower, upper]
##
