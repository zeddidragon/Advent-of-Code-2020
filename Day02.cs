using System.Text;
using System.Text.RegularExpressions;

class Password {
  public int x;
  public int y;
  public char letter;
  public string password;

  public Password(Match match) {
    x = int.Parse(match.Groups["Min"].Value);
    y = int.Parse(match.Groups["Max"].Value);
    letter = char.Parse(match.Groups["Letter"].Value);
    password = match.Groups["Password"].Value;
  }

  public char? charAt(int pos) {
    pos = pos - 1; // Password positions are 1-indexed
    if(password.Length <= pos) {
      return null;
    }
    return password[pos];
  }

  public override string ToString() {
    return $"{x}-{y} {letter}: {password}";
  }
}

public class Day2 : Day {
  static string pattern = @"\b(?<Min>\d+)-(?<Max>\d+) (?<Letter>\w): (?<Password>\w+)";

  public Day2() : base(2) {
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
      var pass = new Password(match);
      var count = 0;
      foreach(char c in pass.password) {
        if(c == pass.letter) {
          count++;
        }
      }
      if(count >= pass.x && count <= pass.y) {
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
      var pass = new Password(match);
      var a = pass.letter == pass.charAt(pass.x);
      var b = pass.letter == pass.charAt(pass.y);
      Console.WriteLine(pass);
      Console.WriteLine($"a:{a}: b:{b}, pass:{pass}");
      if(a ^ b) {
        valid++;
      }
    }
    return $"{valid}";
  }
}
