import numpy as np

def rand_int(input, n = 1):
    final_ints = []

    if len(input) == 1:
        final_ints = np.random.randint(input[0][0], input[0][1] + 1, n)
    else:
        pull_from = np.random.randint(0, len(input), n)

        for i in pull_from:
            final_ints.append(np.random.randint(input[i][0], input[i][1] + 1))
        ##
    ##

    return final_ints
##

def rand_float(input, n = 1, sigfigs=3):
    final = []

    if len(input) == 1:
        final = np.random.uniform(input[0][0], input[0][1], n)
        final = [np.round(x, sigfigs) for x in final]
    else:
        pull_from = np.random.randint(0, len(input), n)

        for i in pull_from:
            final.append(np.round(np.random.uniform(input[i][0], input[i][1]), sigfigs))
        ##
    ##

    return final
##

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
                low = r[1]
            ##
            if high >= r[0] and high <= r[1]:
                high = r[0]
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
                if fr[1] == range[0]:
                    fr[1] = range[1]
                    merged = True
                if fr[0] == range[1]:
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
                    if fr[1] == t[0]:
                        fr[1] = t[1]
                        merged = True
                    if t[1] == fr[0]:
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

def rand_float_range(input, loff, hoff, n=1, sigfigs=3):
    # collate into one number line
    low_limit = min(x[0] for x in input)
    low_limit = low_limit if low_limit > 0 else 0.001
    high_limit = max(x[1] for x in input)

    # starting points for the ranges
    starts = np.random.uniform(low_limit, high_limit, n)
    starts = np.unique(starts)

    ranges = []
    for s in starts:
        low = np.round(s * loff, sigfigs)
        high = np.round(s * hoff, sigfigs)
        
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
                low = r[1]
            ##
            if high >= r[0] and high <= r[1]:
                high = r[0]
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
                if fr[1] == range[0]:
                    fr[1] = range[1]
                    merged = True
                if fr[0] == range[1]:
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
                    if fr[1] == t[0]:
                        fr[1] = t[1]
                        merged = True
                    if t[1] == fr[0]:
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
