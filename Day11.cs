public class Day11 : Day {
  public Day11() : base(11) {
  }

  // This is a game of life variant
  // - If a seat is empty and no seats around it are occupied,
  // the seat becomes occupied
  // - If the seat is occupied, and four or more adjacent (/w diagonals)
  // seats are occupied, the seat becomes empty
  // - Otherwise, nothing happens
  int StepPart1(Map map) {
    var changes = 0;
    var old = map.Clone();
    for(var y = 0; y < map.height; y++) {
      for(var x = 0; x < map.width; x++) {
        var tile = old.TileAt(x, y);
        var filled = old.CountNeighbours(x, y); 
        if(tile == Tile.Seat && filled == 0) {
          map.SetTileAt(x, y, Tile.Filled);
          changes++;
        } else if(tile == Tile.Filled && filled >= 4) {
          map.SetTileAt(x, y, Tile.Seat);
          changes++;
        }
      }
    }
    return changes;
  }

  public override string Part1() {
    // Run the simulation until it's stable.
    // How many seats are occupied?
    var map = new Map(input);
    while(StepPart1(map) > 0) {}
    return $"{map.Count(Tile.Filled)}";
  }

  // Part 2 has somewhat rules
  // - Consider 8 seats within _line of sight_, not adjacency
  // - If a seat is empty and no visible seats around it are occupied,
  // the seat becomes occupied
  // - If the seat is occupied, and _five_ or more _visible_ (/w diagonals)
  // seats are occupied, the seat becomes empty
  // - Otherwise, nothing happens
  int StepPart2(Map map) {
    var changes = 0;
    var old = map.Clone();
    for(var y = 0; y < map.height; y++) {
      for(var x = 0; x < map.width; x++) {
        var tile = old.TileAt(x, y);
        var filled = old.Count8Rays(x, y); 
        if(tile == Tile.Seat && filled == 0) {
          map.SetTileAt(x, y, Tile.Filled);
          changes++;
        } else if(tile == Tile.Filled && filled >= 5) {
          map.SetTileAt(x, y, Tile.Seat);
          changes++;
        }
      }
    }
    return changes;
  }
  public override string Part2() {
    var map = new Map(input);
    while(StepPart2(map) > 0) {}
    return $"{map.Count(Tile.Filled)}";
  }
}
