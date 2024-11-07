public class Day06 : Day {
  public Day06() : base(6) {
  }

  static int Questions = 26;
  static int A = 97;

  public override string Part1() {
    // How many questions were answered yes to in a group?
    var yesCount = 0;
    foreach(var chunk in InputChunks()) {
      var answered = Enumerable.Repeat(false, Questions).ToArray();
      foreach(var line in chunk) {
        foreach(var c in line) {
          var index = (int)c - A;
          answered[index] = true;
        }
      }

      foreach(var yes in answered) {
        if(yes) {
          yesCount++;
        }
      }
    }
    return $"{yesCount}";
  }

  public override string Part2() {
    // How many questions were answered yes to by everyone in a group?
    var yesCount = 0;
    foreach(var chunk in InputChunks()) {
      var answered = Enumerable.Repeat(true, Questions).ToArray();
      foreach(var line in chunk) {
        var lineAnswers = Enumerable.Repeat(false, Questions).ToArray();
        foreach(var c in line) {
          var index = (int)c - A;
          lineAnswers[index] = true;
        }
        for(var i = 0; i < answered.Length; i++) {
          if(answered[i] && !lineAnswers[i]) {
            answered[i] = false;
          }
        }
      }

      foreach(var yes in answered) {
        if(yes) {
          yesCount++;
        }
      }
    }
    return $"{yesCount}";
  }
}
