using System.Text.RegularExpressions;

public class Day12 : Day {
  public class BaseBoat {
    public Vector Pos;
    private struct Command {
      public char Action;
      public int Value;

      static Regex rex = new Regex(@"\b(?<Command>\w)(?<Value>\d+)\b");
      public Command(string line) {
        var match = rex.Match(line);
        Action = match.Groups["Command"].Value[0];
        Value = int.Parse(match.Groups["Value"].Value);
      }
    }

    public void Move(string line) {
      var cmd = new Command(line);
      Move(cmd.Action, cmd.Value);
    }

    public virtual void Move(char cmd, int value) {
      throw new Exception("Not implemented: Move(char, int)");
    }

    public int Manhattan {
      get { return Pos.Manhattan; }
    }
  }

  public class Boat : BaseBoat {
    private Vector Dir;

    public Boat() {
      Dir.X = 1;
    }

    public override void Move(char action, int value) {
      // Move waypoint uing command and values, F moves ship to waypoint
      switch(action) {
        case 'N': Pos.Y -= value; break;
        case 'E': Pos.X += value; break;
        case 'S': Pos.Y += value; break;
        case 'W': Pos.X -= value; break;
        case 'L': Dir.Rotate(360 - value); break;
        case 'R': Dir.Rotate(value); break;
        case 'F': Pos += Dir * value; break;
      }
    }

    public override string ToString() {
      return $"{Pos} | {Dir}";
    }
  }

  private class Boat2 : Boat {
    private Vector Waypoint;

    public Boat2() {
      Waypoint.X = 10;
      Waypoint.Y = -1;
    }

    public override void Move(char action, int value) {
      // Move waypoint uing command and values, F moves ship to waypoint
      switch(action) {
        case 'N': Waypoint.Y -= value; break;
        case 'E': Waypoint.X += value; break;
        case 'S': Waypoint.Y += value; break;
        case 'W': Waypoint.X -= value; break;
        case 'L': Waypoint.Rotate(360 - value); break;
        case 'R': Waypoint.Rotate(value); break;
        case 'F': Pos += Waypoint * value; break;
      }
    }


    public override string ToString() {
      return $"{Pos} | {Waypoint}";
    }
  }

  public Day12() : base(12) {
  }

  public override string Part1() {
    var boat = new Boat();
    foreach(var line in input) {
      boat.Move(line);
    }
    return $"{boat.Manhattan}";
  }

  public override string Part2() {
    var boat = new Boat2();
    foreach(var line in input) {
      boat.Move(line);
    }
    return $"{boat.Manhattan}";
  }
}
