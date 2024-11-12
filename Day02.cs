using System.Text.RegularExpressions;

public class Day02 : Day {
  private struct PasswordLine {
    public int X;
    public int Y;
    public char Letter;
    public string Password;

    public PasswordLine(Match match) {
      X = int.Parse(match.Groups["Min"].Value);
      Y = int.Parse(match.Groups["Max"].Value);
      Letter = char.Parse(match.Groups["Letter"].Value);
      Password = match.Groups["Password"].Value;
    }

    public char? CharAt(int pos) {
      pos = pos - 1; // Password positions are 1-indexed
      if(Password.Length <= pos) {
        return null;
      }
      return Password[pos];
    }

    public override string ToString() {
      return $"{X}-{Y} {Letter}: {Password}";
    }
  }

  static string pattern = @"\b(?<Min>\d+)-(?<Max>\d+) (?<Letter>\w): (?<Password>\w+)";

  public Day02() : base(2) {
  }

  public override string Part1() {
    // How many lines of input are valid?
    // A lines has the format:
    // <x>-<y> <a>: <password>
    // a must appear x-y times in the password
    var rex = new Regex(pattern);
    var valid = 0;
    foreach(var line in input) {
      var match = rex.Match(line);
      var pass = new PasswordLine(match);
      var count = 0;
      foreach(char c in pass.Password) {
        if(c == pass.Letter) {
          count++;
        }
      }
      if(count >= pass.X && count <= pass.Y) {
        valid++;
      }
    }
    return $"{valid}";
  }

  public override string Part2() {
    // How many lines of input are valid?
    // A lines has the format:
    // <x>-<y> <a>: <password>
    // a must appear in position x or y, but not both
    var rex = new Regex(pattern);
    var valid = 0;
    foreach(var line in input) {
      var match = rex.Match(line);
      var pass = new PasswordLine(match);
      var a = pass.Letter == pass.CharAt(pass.X);
      var b = pass.Letter == pass.CharAt(pass.Y);
      if(a ^ b) {
        valid++;
      }
    }
    return $"{valid}";
  }
}
