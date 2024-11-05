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

  // Each row is one integer
  protected IEnumerable<int> InputInts() {
    return input
      .Select(int.Parse);
  }

  // Each chunk lasts until the next newline or end of file
  protected IEnumerable<IEnumerable<string>> InputChunks() {
    var start = 0;
    for(var i = 0; i < input.Length; i++) {
      var line = input[i];
      if(line == "") {
        yield return input.Skip(start).Take(i - start);
        start = i + 1;
      }
    }
    if(start < input.Length) {
      yield return input.Skip(start);
    }
  }

  public virtual string Part1() {
    throw new Exception("Not implemented");
  }

  public virtual string Part2() {
    throw new Exception("Not implemented");
  }
}
