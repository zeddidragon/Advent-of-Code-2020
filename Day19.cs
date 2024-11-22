public class Day19 : Day {
  public Day19() : base(19) {
  }

  class Rule {
    char? Char;
    int[][]? SubRules;

    public Rule(string line) {
      if(line.StartsWith('"')) {
        Char = line[1];
        SubRules = null;
      } else {
        Char = null;
        SubRules = line
          .Split(" | ")
          .Select(row => {
            return row
              .Split(" ")
              .Select(v => int.Parse(v))
              .ToArray();
          })
          .ToArray();
      }
    }


    public struct SearchNode {
      public int Offset;
      public IEnumerable<int> RuleNumbers;

      public SearchNode(IEnumerable<int> ruleNumbers, int offset = 0) {
        RuleNumbers = ruleNumbers;
        Offset = offset;
      }
    }
    public bool IsMatch(Dictionary<int, Rule> rules, string line) {
      var nodes = new Stack<SearchNode>();
      if(SubRules == null) {
        return false;
      }
      foreach(var ruleNums in SubRules) {
        nodes.Push(new SearchNode(ruleNums, 0));
      }
      SearchNode node;
      while(nodes.TryPop(out node)) {
        if(node.Offset == line.Length) {
          return true;
        }
        if(!node.RuleNumbers.Any()) {
          continue;
        }
        if(node.RuleNumbers.Count() > line.Length - node.Offset) {
          continue;
        }
        var rule = rules[node.RuleNumbers.First()];
        var ruleNums = node.RuleNumbers.Skip(1);
        if(rule.Char == line[node.Offset]) {
          nodes.Push(new SearchNode(ruleNums, node.Offset + 1));
          continue;
        }
        if(rule.SubRules == null) {
          continue;
        }
        foreach(var subRuleNums in rule.SubRules) {
          nodes.Push(new SearchNode(subRuleNums.Concat(ruleNums), node.Offset));
        }
      }

      return false;
    }
  }

  Dictionary<int, Rule> rules = new Dictionary<int, Rule>();
  HashSet<string> lines = new HashSet<string>();
  public override string Part1() {
    var isParsingRules = true;
    foreach(var line in input) {
      if(line == "") {
        isParsingRules = false;
        continue;
      }
      if(isParsingRules) {
        var split = line.Split(": ");
        var ruleIndex = int.Parse(split[0]);
        rules[ruleIndex] = new Rule(split[1]);
        continue;
      } else {
        lines.Add(line);
      }
    }

    // How many lines match rule 0?
    var count = lines.Count(line => rules[0].IsMatch(rules, line));
    return $"{count}";
  }

  public override string Part2() {
    // Two of the rules are changed, otherwise same questions as part 1
    // Required a rewrite of rules matching but new function works retroactively
    rules[8] = new Rule("42 | 42 8");
    rules[11] = new Rule("42 31 | 42 11 31");
    var count = lines.Count(line => rules[0].IsMatch(rules, line));
    return $"{count}";
  }
}
