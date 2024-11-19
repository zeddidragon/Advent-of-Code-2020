public class Day03 : Day {
  private Map map;
  private int height;

  public Day03() : base(3) {
    map = new Map(input);
    height = map.Height;
  }

  public override string Part1() {
    var x = 0;
    var trees = 0;

    // Move down the map 1 down, 3 to the right. The wrapping of x happens in the class
    for(var y = 0; y < height; y++) {
      if(map.TileAt(x, y) == Tile.Filled) {
        trees++;
      }
      x += 3;
    }
    return $"{trees}";
  }

  public override string Part2() {
    // This is like part 1 but multiple slopes are tested
    // Right 1, down 1.
    // Right 3, down 1. (This is the slope you already checked.)
    // Right 5, down 1.
    // Right 7, down 1.
    // Right 1, down 2.
    (int x, int y)[] slopes = {
      (1, 1),
      (3, 1), // Part 1
      (5, 1),
      (7, 1),
      (1, 2)
    };

    var result = slopes
      .Select(slope => {
        var x = 0;
        long trees = 0; // Resulting product will be bigger than 32 bits

        // Move down the map 1 down, 3 to the right. The wrapping of x happens in the class
        for(var y = 0; y < height; y += slope.y) {
          if(map.TileAt(x, y) == Tile.Filled) {
            trees++;
          }
          x += slope.x;
        }

        return trees;
      })
      .Aggregate((product, trees) => product * trees);
    return $"{result}";
  }
}
