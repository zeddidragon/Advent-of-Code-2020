using System.Text.RegularExpressions;

public class Day20 : Day {
  Dictionary<long, Map> Tiles = new Dictionary<long, Map>();
  static Regex TileNum = new Regex(@"(?<Id>\d+)");

  public Day20 () : base(20) {
    long tileId = 0;
    List<string> mapTiles = new List<string>();
    foreach(var line in input) {
      if(line.StartsWith("Tile ")) { tileId = long.Parse(TileNum.Match(line).Groups["Id"].Value);
        mapTiles = new List<string>();
      } else if(line == "") {
        Tiles[tileId] = new Map(mapTiles.ToArray());
      } else {
        mapTiles.Add(line);
      }
    }
    if(mapTiles.Count() > 0) {
      Tiles[tileId] = new Map(mapTiles.ToArray());
    }
  }

  class MapGridTile {
    public long Id;
    public Map Map;
    public int EdgeLeft;
    public int EdgeRight;
    public int EdgeUp;
    public int EdgeDown;

    static int RowHash(IEnumerable<Tile> row) {
      int hash = 0;
      var i = 0;
      foreach(var tile in row) {
        var value = tile == Tile.Filled ? 1 : 0;
        hash = hash ^ value << i;
        i++;
      }
      return hash;
    }

    public MapGridTile(long id, Map map) {
      Id = id;
      Map = map;
      EdgeLeft = RowHash(map.Col(0));
      EdgeRight = RowHash(map.Col(-1));
      EdgeUp = RowHash(map.Row(0));
      EdgeDown = RowHash(map.Row(-1));
    }

    public override string ToString() {
      return $"""
      Tile {Id}:
      {Map}

      Left:  {EdgeLeft}
      Right: {EdgeRight}
      Top:   {EdgeUp}
      Bttm:  {EdgeDown}
      """;
    }
  }

  class MapGrid {
    public int Width { get; private set; }
    public int Height { get; private set; }
    public int Size { get => Width * Height; }
    public List<MapGridTile> Tiles = new List<MapGridTile>();
    public Dictionary<Vector, MapGridTile> Positioned = new Dictionary<Vector, MapGridTile>();
    public Vector[] AllPoints;
    HashSet<long> UsedIds = new HashSet<long>();
    int MaxResult = 0;

    public MapGrid(Dictionary<long, Map> tiles, int width, int height) {
      foreach(var kp in tiles) {
        foreach(var map in kp.Value.Rotations()) {
          Tiles.Add(new MapGridTile(kp.Key, map));
        }
      }

      Width = width;
      Height = height;

      AllPoints = Positions().ToArray();
    }

    static Vector Left = new Vector(-1, 0);
    static Vector Right = new Vector(1, 0);
    static Vector Up = new Vector(0, -1);
    static Vector Down = new Vector(0, 1);
    public void FakeCompute() {
      var index = 0;
      foreach(var pos in AllPoints) {
        Positioned[pos] = Tiles[index * 8];
        index++;
      }
    }
    public bool Compute(int index = 0) {
      if(index >= Size) {
        return true;
      }
      if(index > MaxResult) {
        MaxResult = index;
        // Console.WriteLine(this);
      }

      var pos = AllPoints[index];

      int? edgeL = Positioned.ContainsKey(pos + Left) ? Positioned[pos + Left].EdgeRight : null;
      int? edgeR = Positioned.ContainsKey(pos + Right) ? Positioned[pos + Right].EdgeLeft : null;
      int? edgeU = Positioned.ContainsKey(pos + Up) ? Positioned[pos + Up].EdgeDown : null;
      int? edgeD = Positioned.ContainsKey(pos + Down) ? Positioned[pos + Down].EdgeUp : null;

      var matches = Tiles
        .Where(tile => !UsedIds.Contains(tile.Id));
      foreach(var tile in matches) {
        if(edgeL != null && edgeL != tile.EdgeLeft) { continue; }
        if(edgeR != null && edgeR != tile.EdgeRight) { continue; }
        if(edgeU != null && edgeU != tile.EdgeUp) { continue; }
        if(edgeD != null && edgeD != tile.EdgeDown) { continue; }

        UsedIds.Add(tile.Id);
        Positioned[pos] = tile;
        if(Compute(index + 1)) {
          return true;
        }
        UsedIds.Remove(tile.Id);
      }
      Positioned.Remove(pos);
      return false;
    }

    public override string ToString() {
      var lines = new List<string>();
      for(var y = 0; y < Height; y++) {
        var chunk = Positioned
          .Where(pos => pos.Key.Y == y)
          .OrderBy(pos => pos.Key.X);
        if(chunk.Count() == 0) {
          continue;
        }
        lines.Add(String.Join(" ", chunk.Select(c => $"{c.Key} {c.Value.Id}")));
        lines.Add(Map.Display(chunk.Select(c => c.Value.Map)));
        lines.Add("");
      }
      lines.Add("=================");

      return String.Join("\n", lines);
    }

    bool IsInBounds(Vector pos) {
      return pos.X >= 0
        && pos.X < Width
        && pos.Y >= 0
        && pos.Y < Height;
    }

    public IEnumerable<Vector> Positions() {
      // Start by moving to the right
      // If hitting an obstacle (map border or an already yielded tile), turn
      // If there's an obstacle again, every tile has been visited

      // The motivation for spiraling inwards instead of going line-by-line
      // is to complete the edges first. Any given line we complete can
      // and is most likely to be in the middle, but after making a complete
      // L-bend we're more likely to be on the right track early
      var dir = new Vector(1, 0);
      var pos = new Vector(-1, 0);
      var visited = new HashSet<Vector>();
      var wasCollision = false;
      while(true) {
        var next = pos + dir;
        if(IsInBounds(next) && !visited.Contains(next)) {
          wasCollision = false;
          yield return next;
          visited.Add(next);
          pos = next;
        } else if(wasCollision) {
          yield break;
        } else {
          dir.Rotate();
          wasCollision = true;
        }
      }
    }

    // Assembles a Map by combining all the maps contained within
    // The edges of each submap is excluded.
    public Map Assemble() {
      var corner = Tiles.First().Map;
      var tileSize = new Vector(corner.Width - 2, corner.Height - 2);
      var map = new Map(Width * tileSize.X, Height * tileSize.Y);
      foreach(var point in AllPoints) {
        map.Blit(Positioned[point].Map.Cropped(), point * tileSize);
      }
      return map;
    }

    public IEnumerable<Vector> Corners() {
      yield return new Vector(0, 0);
      yield return new Vector(Width - 1, 0);
      yield return new Vector(0, Height - 1);
      yield return new Vector(Width - 1, Height - 1);
    }
  }

  MapGrid? Grid;
  public override string Part1() {
    var width = (int)Math.Sqrt(Tiles.Count);
    var height = width;
    Grid = new MapGrid(Tiles, width, height);
    Grid.Compute();
    long result = Grid
      .Corners()
      .Select(corner => Grid.Positioned[corner].Id)
      .Aggregate((long)1, (aggr, id) => aggr * id);
    return $"{result}";
  }

  static Map SeaMonster = new Map(new string[]{
    "                  # ",
    "#    ##    ##    ###",
    " #  #  #  #  #  #   ",
  });
  public override string Part2() {
    if(Grid == null) {
      return "Run part 1 first.";
    }
    foreach(var map in Grid.Assemble().Rotations()) {
      var count = 0;
      var monsterTiles = new HashSet<Vector>();
      Vector? seek = new Vector(-1, 0);
      while(seek != null) {
        seek = map.MatchPosition(SeaMonster, (Vector)seek);
        if(seek == null) { break; }
        count++;
        foreach(var pos in SeaMonster.EachPoint(Tile.Filled)) {
          monsterTiles.Add(pos + (Vector)seek);
        }
      }
      if(count == 0) {
        continue;
      }

      foreach(var pos in monsterTiles) {
        map.SetTileAt(pos, Tile.Ring);
      }
      Console.WriteLine(map);
      return($"{map.Count(Tile.Filled)}");
    }
    return $"Failed";
  }
}
