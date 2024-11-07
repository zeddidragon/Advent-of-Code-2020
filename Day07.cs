using System.Text.RegularExpressions;

class Ruleset {
  public Dictionary<string, Dictionary<string, int>> registry {
    get;
    private set;
  } = new Dictionary<string, Dictionary<string, int>>();

  static Regex rex = new Regex(@"\b(?<BagColor>\w+ \w+) bags contain( no other bags|( (?<Count>\d+) (?<Contains>\w+ \w+) bags?,?))+\b");
  public void Scan(string line) {
    var match = rex.Match(line);
    var container = match.Groups["BagColor"].Value;
    var rule = new Dictionary<string, int>();
    var contained = match.Groups["Contains"].Captures;
    var counts = match.Groups["Count"].Captures;
    for(var i = 0; i < contained.Count; i++) {
      var bag = contained[i].Value;
      var count = counts[i].Value;
      rule[bag] = int.Parse(count);
    }
    registry[container] = rule;
  }

  private bool ContainerCheck(
      string name,
      string container,
      Dictionary<string, bool> flagged) {
    if(flagged.ContainsKey(container)) {
      return flagged[container];
    }
    var result = name == container;
    foreach(var key in registry[container].Keys) {
      if(ContainerCheck(name, key, flagged)) {
        result = true;
        break;
      }
    }
    flagged[container] = result;
    return result;
  }

  public int ContainerCount(string name) {
    var flagged = new Dictionary<string, bool>();
    var count = 0;
    foreach(var key in registry.Keys) {
      if(key == name) {
        continue;
      }
      if(ContainerCheck(name, key, flagged)) {
        count++;
      }
    }
    return count;
  }

  private int SubContainedCount(string name, Dictionary<string, int> counted) {
    if(counted.ContainsKey(name)) {
      return counted[name];
    }
    var count = 1; // Count the bag in question
    foreach(var item in registry[name]) {
      count += item.Value * SubContainedCount(item.Key, counted);
    }
    counted[name] = count;
    return count;
  }

  public int ContainedCount(string name) {
    var count = 0;
    var counted = new Dictionary<string, int>();
    foreach(var item in registry[name]) {
      count += item.Value * SubContainedCount(item.Key, counted);
    }
    return count;
  }
}

public class Day07 : Day {
  public Day07() : base(7) {
  }

  Ruleset ruleset = new Ruleset();
  public override string Part1() {
    // How many containers can contain shiny gold bags?

    foreach(var line in input) {
      ruleset.Scan(line);
    }

    return $"{ruleset.ContainerCount("shiny gold")}";
  }

  public override string Part2() {
    // How many bags are inside the shiny gold bag?
    return $"{ruleset.ContainedCount("shiny gold")}";
  }
}
