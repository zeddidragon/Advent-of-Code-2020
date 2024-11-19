using System.Text.RegularExpressions;

public class Day16 : Day {
  public Day16() : base(16) {
  }

  private struct Range {
    public int Start;
    public int End;

    public Range(int start, int end) {
      Start = start;
      End = end;
    }

    public bool Contains(int value) {
      return Start <= value && value <= End;
    }
  }

  private class Rule {
    public string Name;
    Range[] Ranges;

    static Regex RulePattern = new Regex(@"\b(?<Name>\w+ ?\w+): (?<Min1>\d+)-(?<Max1>\d+) or (?<Min2>\d+)-(?<Max2>\d+)");
    public Rule(string line) {
      var match = RulePattern.Match(line);
      Name = match.Groups["Name"].Value;
      Ranges = new Range[]{
        new Range(int.Parse(match.Groups["Min1"].Value), int.Parse(match.Groups["Max1"].Value)),
        new Range(int.Parse(match.Groups["Min2"].Value), int.Parse(match.Groups["Max2"].Value)),
      };
    }

    public bool Contains(int value) {
      return Ranges.Any(range => range.Contains(value));
    }

    public override string ToString() {
      return $"{Name}: {Ranges[0].Start}-{Ranges[0].End} or {Ranges[1].Start}-{Ranges[1].End}";
    }
  }

  private struct Ticket {
    public int[] Values;

    static Regex TicketPattern = new Regex(@"\b((?<Value>\w+),?)+\b");
    public Ticket(string line) {
      var match = TicketPattern.Match(line);
      Values = match
        .Groups["Value"]
        .Captures
        .Select(c => int.Parse(c.Value))
        .ToArray();
    }

    public long ErrorRate(IEnumerable<Rule> rules) {
      long errorRate = 0;
      foreach(var value in Values) {
        var matched = rules.FirstOrDefault(rule => rule.Contains(value));
        if(matched == null) {
          errorRate += value;
        }
      }
      return errorRate;
    }

    public override string ToString() {
      return "Ticket: " + String.Join(",", Values);
    }
  }

  List<Rule> rules = new List<Rule>();
  List<Ticket> tickets = new List<Ticket>();
  Ticket? yourTicket;
  enum ParseMode {
    Rules,
    YourTicket,
    TheirTickets,
  };
  public override string Part1() {
    // Find all ticket values that aren't correct for any ticket rule
    // Add these together

    var parseMode = ParseMode.Rules;
    long errorRate = 0;
    foreach(var line in input) {
      if(line == "") {
        continue;
      }
      if(line == "your ticket:") {
        parseMode = ParseMode.YourTicket;
        continue;
      }
      if(line == "nearby tickets:") {
        parseMode = ParseMode.TheirTickets;
        continue;
      }
      
      switch(parseMode) {
        case ParseMode.Rules: {
          rules.Add(new Rule(line));
          break;
        }
        case ParseMode.YourTicket: {
          yourTicket = new Ticket(line);
          break;
        }
        default: {
          var ticket = new Ticket(line);
          var error = ticket.ErrorRate(rules);
          if(error == 0) {
            // Store valid tickets for part 2
            tickets.Add(ticket);
          } else {
            errorRate += error;
          }
          break;
        }
      }
    }

    return $"{errorRate}";
  }

  public override string Part2() {
    // Find which rule applies to which value
    // The order is consistent between all tickets
    //
    // Multiply all numbers on your ticket that starts with "departure"
    if(yourTicket == null) {
      throw new Exception("Your ticket was never parsed!");
    }

    var ruleOrder = new Rule[rules.Count];
    var allMatches = new List<Rule>[ruleOrder.Length];
    for(var i = 0; i < allMatches.Length; i++) {
      allMatches[i] = rules.FindAll(rule => {
        return tickets.All(ticket => {
          return rule.Contains(ticket.Values[i]);
        });
      });
    }

    // Find row with only one viable candidate,
    // pull that from all other matches
    while(true) {
      var index = 0;
      var singleton = allMatches.FirstOrDefault(row => {
        index++;
        return row.Count == 1;
      });
      if(singleton == null) {
        break;
      }
      var settled = singleton[0];
      foreach(var row in allMatches){
        row.Remove(settled);
      }
      ruleOrder[index - 1] = settled;
    }

    long checksum = 1;
    for(var i = 0; i < ruleOrder.Length; i++) {
      var rule = ruleOrder[i];
      if(!rule.Name.StartsWith("departure")) {
        continue;
      }
      checksum *= ((Ticket)yourTicket).Values[i];
    }
    return $"{checksum}";
  }
}
