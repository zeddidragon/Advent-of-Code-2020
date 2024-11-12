public class Day05 : Day {
  private struct BoardingPass {
    public static uint Cols = 8;
    public static uint Rows = 128;
    public static uint MaxId = Cols * (Rows - 1) - 1;

    string raw;
    public uint Id {
      get;
      private set;
    }

    public uint Row {
      get {
        return Id / Cols;
      }
    }

    public uint Col {
      get {
        return Id % Cols;
      }
    }

    public BoardingPass(string input) {
      raw = input;
      uint rowLower = 0;
      uint rowUpper = Rows - 1;
      uint colLower = 0;
      uint colUpper = Cols - 1;
      foreach(var c in input) {
        switch(c) {
          case 'F':
            rowUpper = rowLower + (rowUpper - rowLower) / 2;
            break;
          case 'B':
            rowLower = rowLower + (rowUpper - rowLower) / 2 + 1;
            break;
          case 'L':
            colUpper = colLower + (colUpper - colLower) / 2;
            break;
          case 'R':
            colLower = colLower + (colUpper - colLower) / 2 + 1;
            break;
        }
      }
      Id = rowLower * Cols + colLower;
    }

    public override string ToString() {
      return $"{raw}: Row {Row}, Col {Col}, seat ID {Id}";
    }
  }

  public Day05() : base(5) {
  }

  IEnumerable<BoardingPass> GetBoardingPasses() {
    foreach(var line in input) {
      yield return new BoardingPass(line);
    }
  }
  
  bool[]? filled;
  public override string Part1() {
    uint maxId = 0;
    filled = new bool[BoardingPass.MaxId + 1];

    // Max Id of all boarding passes?
    foreach(var pass in GetBoardingPasses()) {
      if(pass.Id > maxId) {
        maxId = pass.Id;
      }
      filled[pass.Id] = true;
    }

    return $"{maxId}";
  }

  public override string Part2() {
    if(filled == null) {
      return "Part 1 must run first!";
    }

    for(var i = 1; i < filled.Length; i++) {
      // Find the first seat that's free
      // with a preceding seat filled
      if(filled[i - 1] && !filled[i]) {
        return $"{i}";
      }
    }

    return "Seat ID not found";
  }
}
