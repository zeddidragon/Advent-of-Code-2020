public class Day15 : Day {
  private class Game {
    Dictionary<int, int> Registry = new Dictionary<int, int>();
    public int Turn { get; private set; }
    public int Prev { get; private set; }
    public int? Diff { get; private set; }

    public Game(string input) {
      var seed = input
        .Split(",")
        .Select(x => int.Parse(x))
        .ToArray();
      foreach(var n in seed) {
        Seed(n);
      }
    }

    public void Seed(int num) {
      Turn++;
      Prev = num;
      if(Registry.ContainsKey(num)) {
        Diff = Turn - Registry[num];
      } else {
        Diff = null;
      }
      Registry[num] = Turn;
    }

    public int Play() {
      if(Diff == null) {
        Seed(0);
      } else {
        Seed((int)Diff);
      }
      return Prev;
    }

    public int Play(int turns) {
      while(Turn < turns) {
        Play();
      }
      return Prev;
    }
  }

  public Day15() : base(15) {
  }

  public override string Part1() {
    var game = new Game(input[0]);
    return $"{game.Play(2020)}";
  }

  public override string Part2() {
    var game = new Game(input[0]);
    return $"{game.Play(30000000)}";
  }
}
