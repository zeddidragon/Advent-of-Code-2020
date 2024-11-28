public class Day22 : Day {
  public Day22 () : base(22) {
  }

  class Player {
    public int Id;
    int LastPlayed;
    List<int> Cards = new List<int>();

    public Player(int id) {
      Id = id;
    }

    public Player(IEnumerable<int> cards, int id) {
      Cards = new List<int>(cards);
      Id = id;
    }

    public Player Clone(int take) {
      return new Player(Cards.Take(take), Id);
    }

    public void Deal(int card) {
      Cards.Add(card);
    }

    public int Play() {
      var card = Cards[0];
      Cards.RemoveAt(0);
      LastPlayed = card;
      return card;
    }

    public void Win(int card1, int card2) {
      if(card1 == LastPlayed) {
        Cards.Add(card1);
        Cards.Add(card2);
      } else {
        Cards.Add(card2);
        Cards.Add(card1);
      }
    }

    public long Score() {
      long sum = 0;
      var index = Cards.Count();
      foreach(var card in Cards) {
        sum += card * (index--);
      }
      return sum;
    }

    public int Count() {
      return Cards.Count();
    }

    public bool Any() {
      return Cards.Any();
    }

    public string State() {
      return String.Join(',', Cards);
    }

    public override string ToString() {
      return $"Player {Id} Wins!\n{Score()}";
    }
  }

  (Player, Player) Deal() {
    var p1 = new Player(1);
    var p2 = new Player(2);
    var dealt = p1;

    // Deal cards
    foreach(var line in input) {
      if(line == "") {
        continue;
      };
      if(line == "Player 1:") {
        dealt = p1;
        continue;
      } else if(line == "Player 2:") {
        dealt = p2;
        continue;
      }
      dealt.Deal(int.Parse(line));
    }

    return (p1, p2);
  }

  public override string Part1() {
    var (p1, p2) = Deal();

    // Play a game of "Combat"
    var winner = p1;
    while(p1.Any() && p2.Any()) {
      var card1 = p1.Play();
      var card2 = p2.Play();
      winner = card1 > card2 ? p1 : p2;
      winner.Win(card1, card2);
    }

    return $"{winner}";
  }

  // Returns ID of the winner
  int RecursiveCombat(Player p1, Player p2, int depth = 1) {
    // Console.WriteLine($"\n=== Game {depth} ===");
    var states = new HashSet<string>();

    var winner = p1;
    var round = 1;
    while(p1.Any() && p2.Any()) {
      // Console.WriteLine($"\nRound {round} (Game {depth})");
      // If the state has existed before, p1 wins
      var p1State = p1.State();
      var p2State = p2.State();
      // Console.WriteLine($"P1 deck: {p1State}");
      // Console.WriteLine($"P2 deck: {p2State}");
      var state = $"{p1State}|{p2State}";
      if(states.Contains(state)) {
        // Console.WriteLine($"Repeating state:\n{state}\nP1 wins!");
        winner = p1;
        break;
      }
      states.Add(state);

      // Draw top of the deck
      var card1 = p1.Play();
      var card2 = p2.Play();
      // Console.WriteLine($"P1 plays: {card1}");
      // Console.WriteLine($"P2 plays: {card2}");

      // If both players have a deck equal or higher than their card, advance
      if(p1.Count() >= card1 && p2.Count() >= card2) {
        // Console.WriteLine("Playing a sub-game to determine the winner...");
        var winnerId = RecursiveCombat(p1.Clone(card1), p2.Clone(card2), depth + 1);
        winner = winnerId == p1.Id ? p1 : p2;
      } else {
        winner = card1 > card2 ? p1 : p2;
      }
      // Console.WriteLine($"P{winner.Id} wins round {round} of game {depth}!");
      winner.Win(card1, card2);
      round++;
    }

    return winner.Id;
  }

  public override string Part2() {
    var (p1, p2) = Deal();

    // Play a game of "Recursive Combat"
    var winnerId = RecursiveCombat(p1, p2);
    var winner = winnerId == p1.Id ? p1 : p2;

    return $"\n======\n\n{winner}";
  }
}
