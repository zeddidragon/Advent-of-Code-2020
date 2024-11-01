public class Day1 : Day {
  public Day1() : base(1) {
  }

  public override string Part1() {
    var nums = InputInts();

    // find the two entries that sum to 2020
    int target = 2020;

    int length = nums.Count();
    var i = 0;
    foreach(var a in nums) {
      i++;
      foreach(var b in nums.Skip(i)) {
        if(a + b == target) {
          return $"{a} + {b} = {target}\n{a * b}";
        }
      }
    }
    return "Failed part 1";
  }

  public override string Part2() {
    var nums = InputInts();

    // find the THREE entries that sum to 2020
    int target = 2020;

    int length = nums.Count();
    var i = 0;
    foreach(var a in nums) {
      i++;
      var j = i;
      foreach(var b in nums.Skip(j)) {
        j++;
        foreach(var c in nums.Skip(j)) {
          if(a + b + c == target) {
            return $"{a} + {b} + {c} = {target}\n{a * b * c}";
          }
        }
      }
    }
    return "Failed part 2";
  }
}
