public class Day18 : Day {
  public Day18() : base(18) {
  }

  class Computer {
    string Line;
    int Pos;

    public Computer(string line) {
      Line = line;
    }

    enum Operator {
      Add = '+',
      Mul = '*',
    }

    long? Operate(long? a, long? b, Operator op) {
      if(op == Operator.Mul) {
        return a.GetValueOrDefault(1) * b.GetValueOrDefault(1);
      } else {
        return a.GetValueOrDefault(0) + b.GetValueOrDefault(0);
      }
    }

    public long? Compute() {
      long? result = null;
      long? currentValue = null;
      var op = Operator.Add;
      for(; Pos < Line.Length; Pos++) {
        var c = Line[Pos];
        switch(c) {
          case '+':
            op = Operator.Add;
            break;
          case '*':
            op = Operator.Mul;
            break;
          case ' ':
            result = Operate(result, currentValue, op);
            currentValue = null;
            break;
          case '(':
            Pos++;
            currentValue = Compute();
            break;
          case ')':
            goto End;
          default:
            if(currentValue == null) {
              currentValue = (c - '0');;
            } else {
              currentValue = currentValue * 10 + (c - '0');
            }
            break;
        }
        continue;
        End:
          break;
      }
      result = Operate(result, currentValue, op);
      return result;
    }
  }

  public override string Part1() {
    // Solve each line of math equation
    // There is no operator presedence
    // Return the sum of each equation's answer
    long sum = 0;
    foreach(var line in input) {
      var result = new Computer(line).Compute();
      if(result != null) {
        sum += (long)result;
      }
    }
    return $"{sum}";
  }

  public override string Part2() {
    // Solve each line of math equation
    // There operator presedence is opposite (+ solves before *)
    // Return the sum of each equation's answer
    long sum = 0;
    foreach(var line in input) {
      // Rather than change the computer, we can change each input line so that parantheses force the presedence for us
      var replaced = "(" + (line
        .Replace("(", "((")
        .Replace(")", "))")
        .Replace(" * ", ") * (")
      ) + ")";
      var result = new Computer(replaced).Compute();
      if(result != null) {
        sum += (long)result;
      }
    }
    return $"{sum}";
  }
}
