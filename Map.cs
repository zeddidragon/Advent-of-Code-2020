enum Tile {
  Empty = '.',
  Filled = '#',
  Seat = 'L',
}

class Map {
  public int Width {
    get;
    protected set;
  }
  public int Height {
    get => Tiles.Length / Width;
  }
  Tile[] Tiles;

  Tile ParseTile(char input) {
    switch(input) {
      case '#':
        return Tile.Filled;
      case 'L':
        return Tile.Seat;
      default:
        return Tile.Empty;
    }
  }

  public Map(Tile[] tiles, int width) {
    Tiles = (Tile[])tiles.Clone();
    Width = width;
  }

  public Map(string[] input) {
    Width = input[0].Length;
    Tiles = new Tile[Width * input.Length];
    var index = 0;
    foreach(var line in input) {
      foreach(var c in line) {
        Tiles[index] = ParseTile(c);
        index++;
      }
    }
  }

  private int CoordIndex(int x, int y) {
    x = x % Width;
    return y * Width + x;
  }

  private int CoordIndex(Vector pos) {
    return CoordIndex(pos.X, pos.Y);
  }

  public Tile TileAt(int x, int y) {
    return Tiles[CoordIndex(x, y)];
  }

  public Tile TileAt(Vector pos) {
    return Tiles[CoordIndex(pos)];
  }

  public void SetTileAt(int x, int y, Tile tile) {
    Tiles[CoordIndex(x, y)] = tile;
  }

  public void SetTileAt(Vector pos, Tile tile) {
    Tiles[CoordIndex(pos)] = tile;
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
  public int CountNeighbours(Vector pos, Tile cmp = Tile.Filled) {
    var count = 0;
    foreach(var dir in neighbours) {
      var nPos = pos + dir;
      if(nPos.X < 0 || nPos.X >= Width) continue;
      if(nPos.Y < 0 || nPos.Y >= Height) continue;
      if(TileAt(nPos) == cmp) {
        count++;
      }
    }
    return count;
  }

  // Counts instances of `cmp` from 8 rays from this position
  public int Count8Rays(Vector pos, Tile cmp = Tile.Filled, Tile pass = Tile.Empty) {
    var count = 0;
    foreach(var dir in neighbours) {
      var nPos = pos + dir;
      while(nPos.X >= 0 && nPos.X < Width
        && nPos.Y >= 0 && nPos.Y < Height) {
        var tile = TileAt(nPos);
        if(tile == pass) {
          nPos += dir;
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

  public IEnumerable<Vector> EachPoint() {
    for(var y = 0; y < Height; y++) {
      for(var x = 0; x < Width; x++) {
        yield return new Vector(x, y);
      }
    }
  }

  public Map Clone() {
    return new Map(Tiles, Width);
  }

  public override string ToString() {
    var rows = Tiles
      .Select(tile => (char)tile)
      .Chunk(Width)
      .Select(row => String.Join("", row));
    return String.Join("\n", rows.ToArray());
  }
}
