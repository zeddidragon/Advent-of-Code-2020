enum Tile {
  Field = '.',
  Tree = '#'
}

class Map {
  public int width {
    get;
    private set;
  }
  public int height {
    get => tiles.Length / width;
  }
  Tile[] tiles;

  public Map(string[] input) {
    width = input[0].Length;
    int height = input.Length;

    tiles = new Tile[width * height];
    var index = 0;
    foreach(var line in input) {
      foreach(var c in line) {
        if(c == '#') {
          tiles[index] = Tile.Tree;
        } else {
          tiles[index] = Tile.Field;
        }
        index++;
      }
    }
  }

  public Tile TileAt(int x, int y) {
    x = x % width;
    return tiles[y * width + x];
  }

  public override string ToString() {
    var rows = tiles
      .Select(tile => (char)tile)
      .Chunk(width)
      .Select(row => String.Join("", row));
    return String.Join("\n", rows.ToArray());
  }
}

public class Day03 : Day {
  private Map map;
  private int height;

  public Day03() : base(3) {
    map = new Map(input);
    height = map.height;
  }

  public override string Part1() {
    var x = 0;
    var trees = 0;

    // Move down the map 1 down, 3 to the right. The wrapping of x happens in the class
    for(var y = 0; y < height; y++) {
      if(map.TileAt(x, y) == Tile.Tree) {
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
          if(map.TileAt(x, y) == Tile.Tree) {
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
