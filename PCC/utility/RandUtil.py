import numpy as np

def rand_int_range(input, loff, hoff, n=1):
    # collate into one number line
    low_limit = min(x[0] for x in input)
    low_limit = low_limit if low_limit > 0 else 1
    high_limit = max(x[1] for x in input)

    # starting points for the ranges
    starts = np.random.randint(low_limit, high_limit + 1, n)
    starts = np.unique(starts)

    ranges = []
    for s in starts:
        low = int(np.round(s * loff))
        high = int(np.round(s * hoff))
        
        if low < low_limit:
            low = low_limit
        ##

        if high > high_limit:
            high = high_limit
        ##

        skip = False
        for r in ranges:
            if low >= r[0] and high <= r[1]:
                skip = True
                break
            ##
            if low <= r[1]:
                low = r[1] + 1
            ##
            if high >= r[0] and high <= r[1]:
                high = r[0] - 1
            ##
        ##

        if skip == False:
            ranges.append([low, high])
        ##
    ##

    final_ranges = []

    if len(input) == 1:
        for range in ranges:
            merged = False
            for fr in final_ranges:
                if fr[1] == range[0] - 1:
                    fr[1] = range[1]
                    merged = True
                if fr[0] + 1 == range[1]:
                    fr[0] = range[0]
                    merged = True
                ##
            ##
            if merged == False:
                final_ranges.append(range)
            ##
        ##
    else:
        for range in ranges:
            for inp in input:
                if range[0] == range[1] and range[0] < inp[0]:
                    break
                ##

                # if the range's current maximum is smaller than the minimum of the provided range, stop -- no more refinements
                #   can be made!
                if range[1] < inp[0]:
                    break
                ##

                # skip this area if we're at or over the limit's threshold
                if range[0] > inp[1]:
                    continue
                ##

                start = max(inp[0], range[0])
                end = min(inp[1], range[1])
                t = [start, end]
                
                # merge ones that are too close together
                merged = False
                for fr in final_ranges:
                    if fr[1] == t[0] - 1:
                        fr[1] = t[1]
                        merged = True
                    if t[1] == fr[0] - 1:
                        fr[0] = t[0]
                        merged = True
                    ##
                ##

                range[0] = end

                if merged == False:
                    final_ranges.append(t)
                ##
            ##
        ##
    ##

    return final_ranges
##

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

    low_limit = low_limit if low_limit > 0 else 1

    high_limit = max(x[1] for x in ranges)
    
    num = np.round(np.random.randint(low_limit, high_limit + 1))
    lower = int(np.round(num * ll))
    upper = int(np.round(num * hl))
    if upper > high_limit:
        upper = high_limit

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
    if low_limit == 0:
        low_limit = 1
    high_limit = max(x[1] for x in ranges)

    nums = np.random.randint(low_limit, high_limit + 1, n)
    nums = np.unique(nums)

    res = []

    for num in nums:
        low = int(np.round(num - num * ll)) if np.round(num - num * ll) > low_limit else low_limit
        high = int(np.round(num + num * hl)) if np.round(num + num * hl) < high_limit else high_limit

        skip = False
        for r in res:
            if low >= r[0] and high <= r[1]:
                skip = True
                break
            ##
            if low <= r[1]:
                low = r[1] + 1
            if high >= r[0] and high <= r[1]:
                high = r[0] - 1
                if high < low:
                    high = low
            ##
        ##
        if skip == False:
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
