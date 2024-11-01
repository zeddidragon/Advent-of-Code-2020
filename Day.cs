public partial class Day {
  public int day {
    get;
    protected set;
  }
  protected string[] input;

  protected Day(int _day) {
    day = _day;
    var path = String.Format("inputs/input-day-{0:00}.txt", day);
    input = File.ReadAllLines(path);
  }

  protected IEnumerable<int> InputInts() {
    return input
      .Select(int.Parse);
  }

  public virtual string Part1() {
    throw new Exception("Not implemented");
  }

  public virtual string Part2() {
    throw new Exception("Not implemented");
  }
}
