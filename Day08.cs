using System.Text.RegularExpressions;

enum Operation {
  acc,
  jmp,
  nop,
}

struct Instruction {
  public Operation op;
  public int val { get; private set; }

  public Instruction(Operation _op, int _val) {
    op = _op;
    val = _val;
  }

  public Instruction(string opstring, string valstring) {
    Enum.TryParse<Operation>(opstring, out op);
    val = int.Parse(valstring);
  }

  public override string ToString() {
    return $"{op} {val}";
  }
}

class Code {
  Instruction[] code;
  public int accumulator  { get; private set; } = 0;
  public int Length { get { return code.Length; } }

  Regex rex = new Regex(@"\b(?<Op>\w{3}) (?<Val>(\+|-)?\d+)\b");
  public Code(string[] input) {
    code = input
      .Select(line => {
        var match = rex.Match(line);
        return new Instruction(
            match.Groups["Op"].Value,
            match.Groups["Val"].Value);
      })
      .ToArray();
  }

  public Code(Instruction[] _code) {
    code = _code;
  }

  // Returns true if the program terminates, false if there's an infinite loop
  public bool Run() {
    var visited = new bool[Length];
    accumulator = 0;
    for(var pos = 0; pos < Length; pos++) { 
      if(visited[pos]) {
        return false;
      }
      visited[pos] = true;
      var posPre = pos;
      var instruction = code[pos];
      switch(instruction.op) {
        case Operation.acc:
          accumulator += instruction.val;
          break;
        case Operation.jmp:
          pos += instruction.val - 1;
          break;
        case Operation.nop:
          break;
      }
    }
    return true;
  }

  public Code Overwrite(int pos, Instruction line) {
    var newCode = new Instruction[Length];
    for(var i = 0; i < Length; i++) {
      newCode[i] = code[i];
    }
    newCode[pos] = line;
    return new Code(newCode);
  }

  public Instruction InstructionAt(int pos) {
    return code[pos];
  }

  public override string ToString() {
    return String.Join("\n", code);
  }
}

public class Day8 : Day {
  public Day8() : base(8) {
  }

  public override string Part1() {
    var program = new Code(input);
    program.Run();
    return $"{program.accumulator}";
  }

  public override string Part2() {
    var source = new Code(input);
    for(var i = 0; i < source.Length; i++) {
      var line = source.InstructionAt(i);
      Code? overwritten = null;
      switch(line.op) {
        case Operation.nop:
          overwritten = source.Overwrite(i, new Instruction(
            Operation.jmp,
            line.val));
          break;
        case Operation.jmp:
          overwritten = source.Overwrite(i, new Instruction(
            Operation.nop,
            line.val));
          break;
      }
      if(overwritten != null && overwritten.Run()) {
        return $"{overwritten.accumulator}";
      }
    }
    return "Failed part 2";
  }
}
