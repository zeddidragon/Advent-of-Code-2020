public class Day09 : Day {
  public Day09() : base(9) {
  }

  long[] numbers = new long[0];
  long weakpoint = 0;

  public override string Part1() {
    // First number that isn't a sum of two of the previous 25 numbers?
    var bufferSize = 25;
    numbers = InputInts().ToArray();
    for(var i = bufferSize; i < numbers.Length; i++) {
      var num = numbers[i];
      for(var j = i - bufferSize; j < i; j++) {
        var a = numbers[j];
        for(var k = j + 1; k < i; k++) {
          var b = numbers[k];
          if(a + b == num) {
            goto Found;
          }
        }
      }
      weakpoint = num;
      return $"{num}";
    Found:
      continue;
    }
    return "Failed part 1";
  }

  public override string Part2() {
    // Find a contiguous set of numbers adding together to the answer of part 1.
    // Return the sum of the smallest and largest number in that set.
    for(var i = 0; i < numbers.Length; i++) {
      long sum = 0;
      long min = weakpoint;
      long max = 0;
      for(var j = i; j < numbers.Length; j++) {
        var num = numbers[j];
        if(num < min) {
          min = num;
        }
        if(num > max) {
          max = num;
        }
        sum += num;
        if(sum == weakpoint) {
          return $"{min + max}";
        } else if(sum > weakpoint) {
          break;
        }
      }
    }
    return "Failed part 2";
  }
}
