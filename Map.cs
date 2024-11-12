enum Tile {
  Ground = '.',
  Filled = '#',
  Seat = 'L',
}

class Map {
  public int width {
    get;
    private set;
  }
  public int height {
    get => Tiles.Length / width;
  }
  Tile[] Tiles;

  Tile ParseTile(char input) {
    switch(input) {
      case '#':
        return Tile.Filled;
      case 'L':
        return Tile.Seat;
      default:
        return Tile.Ground;
    }
  }

  public Map(Tile[] _Tiles, int _width) {
    Tiles = (Tile[])_Tiles.Clone();
    width = _width;
  }

  public Map(string[] input) {
    width = input[0].Length;
    int height = input.Length;

    Tiles = new Tile[width * height];
    var index = 0;
    foreach(var line in input) {
      foreach(var c in line) {
        Tiles[index] = ParseTile(c);
        index++;
      }
    }
  }

  private int CoordIndex(int x, int y) {
    x = x % width;
    return y * width + x;
  }

  public Tile TileAt(int x, int y) {
    return Tiles[CoordIndex(x, y)];
  }

  public void SetTileAt(int x, int y, Tile tile) {
    Tiles[CoordIndex(x, y)] = tile;
  }

  // Counts instances of `cmp` mapwide
  public int Count(Tile cmp) {
    return Tiles.Count(tile => tile == cmp);
  }

  static Vector[] neighbours = {
    new Vector( 0, -1), // North
    new Vector(+1, -1), // North-East
    new Vector(+1,  0), // East
    new Vector(+1, +1), // South-East
    new Vector( 0, +1), // South
    new Vector(-1, +1), // South-West
    new Vector(-1,  0), // West
    new Vector(-1, -1), // North-West
  };


  // Counts instances of `cmp` adjacent to this position
  // Position itself is not included
  // Diagonals are counted
  public int CountNeighbours(int x, int y, Tile cmp = Tile.Filled) {
    var count = 0;
    foreach(var dir in neighbours) {
      var _x = x + dir.X;
      var _y = y + dir.Y;
      if(_x < 0 || _x >= width) continue;
      if(_y < 0 || _y >= height) continue;
      var index = _y * width + _x;
      var tile = Tiles[index];
      if(tile == cmp) {
        count++;
      }
    }
    return count;
  }

  // Counts instances of `cmp` from 8 rays from this position
  public int Count8Rays(int x, int y, Tile cmp = Tile.Filled, Tile pass = Tile.Ground) {
    var count = 0;
    foreach(var dir in neighbours) {
      var _x = x + dir.X;
      var _y = y + dir.Y;
      while(_x >= 0 && _x < width
        && _y >= 0 && _y < height) {
        var index = _y * width + _x;
        var tile = Tiles[index];
        if(tile == pass) {
          _x += dir.X;
          _y += dir.Y;
          continue;
        } else if(tile == cmp) {
          count++;
          break;
        } else {
          break;
        }
      }
    }
    return count;
  }

  public Map Clone() {
    return new Map(Tiles, width);
  }

  public override string ToString() {
    var rows = Tiles
      .Select(tile => (char)tile)
      .Chunk(width)
      .Select(row => String.Join("", row));
    return String.Join("\n", rows.ToArray());
  }
}
