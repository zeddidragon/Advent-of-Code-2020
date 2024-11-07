public class Day10 : Day {
  public Day10() : base(10) {
  }

  long[] numbers = new long[0];

  public override string Part1() {
    // Sort the list, count difference gaps
    long prev = 0;
    numbers = InputInts()
      .ToList()
      .OrderBy(a => a)
      .ToArray();
    var diffs = new long[4];
    foreach(var num in numbers) {
      var diff = num - prev;
      diffs[diff]++;
      prev = num;
    }
    diffs[3]++; // Your device's adapter
    return $"{diffs[1] * diffs[3]}";
  }

  long CountPermutations(int index, Dictionary<int, long> cache) {
    if(index >= numbers.Length) {
      return 0;
    }
    if(cache.ContainsKey(index)) {
      return cache[index];
    }
    var num = index < 0 ? 0 : numbers[index];
    long counted = 0;
    for(var j = index + 1; j < numbers.Length; j++) {
      var num2 = numbers[j];
      if(num2 <= num + 3) {
        counted += CountPermutations(j, cache);
      } else {
        break;
      }
    }
    if(counted < 1) {
      counted = 1;
    }
    cache[index] = counted;
    return counted;
  }

  public override string Part2() {
    // How many sets/subsets exist from the input where no gaps are larger than 3?
    var cache = new Dictionary<int, long>();
    return $"{CountPermutations(-1, cache)}";
  }
}
