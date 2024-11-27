using System.Text.RegularExpressions;

public class Day21 : Day {
  public Day21 () : base(21) {
  }

  class Food {
    public HashSet<string> Ingredients;
    public HashSet<string> Allergens;

    static Regex FoodPattern = new Regex(@"^((?<Ingredient>\w+) )+\(contains ((?<Allergen>\w+)(, )?)+\)$");
    public Food(string line) {
      var match = FoodPattern.Match(line);
      Ingredients = new HashSet<string>(
        match
          .Groups["Ingredient"]
          .Captures
          .Select(c => c.Value)
      );
      Allergens = new HashSet<string>(
        match
          .Groups["Allergen"]
          .Captures
          .Select(c => c.Value)
      );
    }

    public override string ToString() {
      return $"{String.Join(" ", Ingredients)} (contains {String.Join(", ", Allergens)})";
    }
  }
  
  Dictionary<string, string> Part2Solution = new Dictionary<string, string>();
  public override string Part1() {
    var foods = input.Select(line => new Food(line));
    var ingredients = new HashSet<string>();
    var allergens = new HashSet<string>();
    var allergenCandidates = new Dictionary<string, HashSet<string>>();

    // Enumerate every ingredient and allergen
    foreach(var food in foods) {
      ingredients.UnionWith(food.Ingredients);
      allergens.UnionWith(food.Allergens);
    }

    // Map for every candidate for an allergen
    foreach(var allergen in allergens) {
      allergenCandidates[allergen] = foods
        .Where(f => f.Allergens.Contains(allergen))
        .Select(f => f.Ingredients)
        .Aggregate((a, b) => new HashSet<string>(a.Intersect(b)));
    }

    // Find ingredients that can't possibly have allergesn
    var excluded = new HashSet<string>(ingredients);
    foreach(var set in allergenCandidates.Values) {
      excluded.ExceptWith(set);
    }

    // Count instances of these throughout all food
    var count = foods
      .Aggregate((long)0, (sum, food) => sum + food.Ingredients.Intersect(excluded).Count());


    // Might as well solve part 2 here
    var processed = new HashSet<string>();
    while(processed.Count < allergenCandidates.Count) {
      var kv = allergenCandidates.First(c => c.Value.Count == 1);
      processed.Add(kv.Key);
      var allergen = kv.Key;
      var ingredient = kv.Value.First();
      Part2Solution.Add(allergen, ingredient);
      foreach(var kv2 in allergenCandidates) {
        kv2.Value.Remove(ingredient);
      }
    }

    return $"{count}";
  }
  public override string Part2() {
    return String.Join(",", Part2Solution.OrderBy(kv => kv.Key).Select(kv => kv.Value));
  }
}
