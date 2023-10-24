import sys
import numpy as np

def randint_from_ranges(ranges):
    choices = []
    for i in range(len(ranges)):
        choices.append(np.random.randint(ranges[i][0], ranges[i][1] + 1))
    ##
    return np.random.choice(choices)
##

def randfloat_from_ranges(ranges, sigfigs=3):
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

        # if our biggest value is smaller than the smallest value of the current "band," break
        if res[1] < rng[0]:
            break
        ##

        # skip this area if we're at or over the limit's threshold
        if res[0] >= rng[1]:
            continue
        ##

        start = max(res[0], rng[0])
        end = min(res[1], rng[1])
        t = [start, end]

        res[0] = end
        results.append(t)
    ##

    return results
##

def randfloat_srange_from_ranges(ranges, ll, hl, sigfigs=3):
    # collate into one number line
    low_limit = min(x[0] for x in ranges)
    
    # Just having 0 doesn't make sense...
    low_limit = low_limit if low_limit > 0 else 0.001

    high_limit = max(x[1] for x in ranges)
    
    lower = np.round(np.random.uniform(low_limit, np.round(low_limit + low_limit * ll)), decimals=sigfigs)
    upper = np.round(np.random.uniform(np.round(high_limit - high_limit * hl), high_limit), decimals=sigfigs)

    return [lower, upper]
##

def randfloat_mrange_from_ranges(ranges, ll, hl, sigfigs=3):
    # this function takes care of producing one long number line
    res = randfloat_srange_from_ranges(ranges, ll, hl, sigfigs)

    # we need to break it back up into being in line with the features
    results = []
    for rng in ranges:
        if res[0] == res[1] and res[0] < rng[1]:
            break
        ##

        # if our biggest value is smaller than the smallest value of the current "band," break
        if res[1] < rng[0]:
            break
        ##

        # skip this area if we're at or over the limit's threshold
        if res[0] >= rng[1]:
            continue
        ##

        start = max(res[0], rng[0])
        end = min(res[1], rng[1])
        t = [start, end]

        res[0] = end
        results.append(t)
    ##

    return results
##

def randint_narrow_srange_from_ranges(ranges, ll, hl):
    # collate into one number line
    low_limit = min(x[0] for x in ranges)
    high_limit = max(x[1] for x in ranges)

    num = np.random.randint(low_limit, high_limit + 1)

    low = np.round(num - num * ll) if np.round(num - num * ll) > low_limit else low_limit
    high = np.round(num + num * hl) if np.round(num + num * hl) < high_limit else high_limit

    return [int(low), int(high)]
##

def randint_narrow_mrange_from_ranges(ranges, ll, hl):
    # this function takes care of producing one long number line
    res = randint_narrow_srange_from_ranges(ranges, ll, hl)

    # we need to break it back up into being in line with the features
    results = []
    for rng in ranges:
        if res[0] == res[1] and res[0] < rng[1]:
            break
        ##

        # if our biggest value is smaller than the smallest value of the current "band," break
        if res[1] < rng[0]:
            break
        ##
        
        # skip this area if we're at or over the limit's threshold
        if res[0] >= rng[1]:
            continue
        ##

        start = max(res[0], rng[0])
        end = min(res[1], rng[1])
        t = [start, end]

        res[0] = end
        results.append(t)
    ##

    return results
##

def randfloat_narrow_srange_from_ranges(ranges, ll, hl, sigfigs=3):
    # collate into one number line
    low_limit = min(x[0] for x in ranges)

    # Just having 0 doesn't make sense...
    low_limit = low_limit if low_limit > 0 else 0.001

    high_limit = max(x[1] for x in ranges)
    
    num = np.random.uniform(low_limit, high_limit)

    low = np.round(num - num * ll, decimals=sigfigs) if np.round(num - num * ll, decimals=sigfigs) > low_limit else low_limit
    high = np.round(num + num * hl, decimals=sigfigs) if np.round(num + num * hl, decimals=sigfigs) < high_limit else high_limit

    return [low, high]
##

def randfloat_narrow_mrange_from_ranges(ranges, ll, hl, sigfigs=3):
    # this function takes care of producing one long number line
    res = randfloat_narrow_srange_from_ranges(ranges, ll, hl, sigfigs)

    # we need to break it back up into being in line with the features
    results = []
    for rng in ranges:
        if res[0] == res[1] and res[0] < rng[1]:
            break
        ##

        # if our biggest value is smaller than the smallest value of the current "band," break
        if res[1] < rng[0]:
            break
        ##

        # skip this area if we're at or over the limit's threshold
        if res[0] >= rng[1]:
            continue
        ##

        start = max(res[0], rng[0])
        end = min(res[1], rng[1])
        t = [start, end]

        res[0] = end
        results.append(t)
    ##

    return results
##

def randint_multiarea_from_ranges(ranges, ll, hl, n):
    # collate into one number line
    low_limit = min(x[0] for x in ranges)
    high_limit = max(x[1] for x in ranges)

    nums = np.random.randint(low_limit, high_limit + 1, n)
    nums.sort()

    res = []

    for num in nums:
        low = int(np.round(num - num * ll)) if np.round(num - num * ll) > low_limit else low_limit
        high = int(np.round(num + num * hl)) if np.round(num + num * hl) < high_limit else high_limit

        for r in res:
            if low <= r[1]:
                low = r[1] + 1
            if high >= r[0] and high <= r[1]:
                high = r[0] - 1
            ##
        ##

        res.append([low, high])
    ##

    return res
##

def randint_multi_mrange_from_ranges(ranges, ll, hl, n):
    # this function takes care of producing one long number line
    res = randint_multiarea_from_ranges(ranges, ll, hl, n)

    # we need to break it back up into being in line with the features
    results = []
    for r in res:
        for rng in ranges:
            if r[0] == r[1] and r[0] < rng[1]:
                break
            ##

            # if our biggest value is smaller than the smallest value of the current "band," break
            if r[1] < rng[0]:
                break
            ##
            
            # skip this area if we're at or over the limit's threshold
            if r[0] >= rng[1]:
                continue
            ##

            start = max(r[0], rng[0])
            end = min(r[1], rng[1])
            t = [start, end]

            r[0] = end
            results.append(t)
        ##
    ##

    return results
##

def randfloat_multiarea_from_ranges(ranges, ll, hl, n, sigfigs=3):
    # collate into one number line
    low_limit = min(x[0] for x in ranges)
    
    # Just having 0 doesn't make sense...
    low_limit = low_limit if low_limit > 0 else 0.001

    high_limit = max(x[1] for x in ranges)

    nums = np.random.uniform(low_limit, high_limit, n)
    nums.sort()

    res = []

    for num in nums:
        low = np.round(num - num * ll, sigfigs) if num - num * ll > low_limit else low_limit
        high = np.round(num + num * hl, sigfigs) if num + num * hl < high_limit else high_limit

        invalid = False
        for r in res:
            if low <= r[1]:
                low = r[1] + r[1] * ll
                if low > high_limit:
                    invalid = True
                    break
            if high >= r[0] and high <= r[1]:
                high = r[0] - r[1] * hl
                if high > high_limit:
                    invalid = True
                    break
            ##
        ##

        if invalid == False:
            if low > high:
                s = low
                low = high
                high = s
            ##
            res.append([low, high])
        ##
    ##

    return res
##

def randfloat_multi_mrange_from_ranges(ranges, ll, hl, n, sigfigs=3):
    # this function takes care of producing one long number line
    res = randfloat_multiarea_from_ranges(ranges, ll, hl, n, sigfigs)

    # we need to break it back up into being in line with the features
    results = []
    for r in res:
        for rng in ranges:
            if r[0] == r[1] and r[0] < rng[1]:
                break
            ##

            # if our biggest value is smaller than the smallest value of the current "band," break
            if r[1] < rng[0]:
                break
            ##
            
            # skip this area if we're at or over the limit's threshold
            if r[0] >= rng[1]:
                continue
            ##

            start = max(r[0], rng[0])
            end = min(r[1], rng[1])
            t = [start, end]

            r[0] = end
            results.append(t)
        ##
    ##

    return results
##
