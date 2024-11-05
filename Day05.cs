struct BoardingPass {
  public static uint Cols = 8;
  public static uint Rows = 128;
  public static uint MaxId = Cols * (Rows - 1) - 1;

  string raw;
  public uint id {
    get;
    private set;
  }

  public uint row {
    get {
      return id / Cols;
    }
  }

  public uint col {
    get {
      return id % Cols;
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
    id = rowLower * Cols + colLower;
  }

  public override string ToString() {
    return $"{raw}: row {row}, col {col}, seat ID {id}";
  }
}

public class Day5 : Day {
  public Day5() : base(5) {
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

    // Max ID of all boarding passes?
    foreach(var pass in GetBoardingPasses()) {
      if(pass.id > maxId) {
        maxId = pass.id;
      }
      filled[pass.id] = true;
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
