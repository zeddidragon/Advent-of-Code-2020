enum Tile {
  Empty = '.',
  Filled = '#',
  Seat = 'L',
  Ring = 'O',
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
      case 'O':
        return Tile.Ring;
      default:
        return Tile.Empty;
    }
  }

  public Map(Tile[] tiles, int width) {
    Tiles = (Tile[])tiles.Clone();
    Width = width;
  }

  public Map(int width, int height) {
    Tiles = new Tile[width * height];
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

  public IEnumerable<Vector> EachPoint(Tile cmp) {
    return EachPoint().Where(pos => TileAt(pos) == cmp);
  }

  public Map Clone() {
    return new Map(Tiles, Width);
  }

  public Map Rotate(int dir = 90) {
    if(dir == 0) {
      return this;
    }

    var map = new Map(Height, Width);
    foreach(var point in EachPoint()) {
      var rotated = new Vector(Width - point.Y - 1, point.X);
      map.SetTileAt(rotated, TileAt(point));
    }

    return map.Rotate(dir - 90);
  }

  public Map FlipX() {
    var map = new Map(Height, Width);
    foreach(var point in EachPoint()) {
      var rotated = new Vector(Width - point.X - 1, point.Y);
      map.SetTileAt(rotated, TileAt(point));
    }

    return map;
  }

  public Map FlipY() {
    var map = new Map(Height, Width);
    foreach(var point in EachPoint()) {
      var rotated = new Vector(point.X, Height - point.Y - 1);
      map.SetTileAt(rotated, TileAt(point));
    }

    return map;
  }

  public void Blit(Map other, Vector offset) {
    foreach(var point in other.EachPoint()) {
      var pointAt = point + offset;
      if(pointAt.X < 0
        || pointAt.X >= Width
        || pointAt.Y < 0
        || pointAt.Y >= Height
      ) {
        continue;
      }
      SetTileAt(pointAt, other.TileAt(point));
    }
  }

  // Crops x tiles off each edge
  public Map Cropped(int count = 1) {
    return Cropped(new Vector(count, count), new Vector(Width - count, Height - count));
  }

  public Map Cropped(Vector offset, Vector limit) {
    var width = limit.X - offset.X;
    var height = limit.Y - offset.Y;
    var map = new Map(width, height);
    map.Blit(this, -offset);
    return map;
  }

  public Vector? MatchPosition(Map subMap) {
    return MatchPosition(subMap, new Vector(-1, 0));
  }

  Vector East = new Vector(1, 0);
  public Vector? MatchPosition(Map subMap, Vector pos) {
    while(true) {
      pos += East;
      if(pos.X + subMap.Width > Width) {
        pos = new Vector(0, pos.Y + 1);
      }
      if(pos.Y + subMap.Height > Height) {
        break;
      }
      if(subMap
        .EachPoint(Tile.Filled)
        .All(subPos => TileAt(pos + subPos) == Tile.Filled)
      ) {
        return pos;
      }
    }
    return null;
  }

  public IEnumerable<Map> Rotations() {
    var map = Clone();
    for(var dir = 0; dir < 360; dir += 90) {
      yield return map;
      yield return map.FlipX();
      yield return map.FlipY();
      map = map.Rotate();
    }
  }

  public IEnumerable<Tile> Row(int rowNum) {
    while(rowNum < 0) {
      rowNum += Width;
    }
    return Tiles
      .Chunk(Width)
      .Skip(rowNum)
      .First();
  }

  public string StringRow(int rowNum) {
    return String.Join("", Row(rowNum).Select(tile => (char)tile));
  }

  public IEnumerable<Tile> Col(int colNum) {
    while(colNum < 0) {
      colNum += Height;
    }
    return EachPoint()
      .Where(pos => pos.X == colNum)
      .Select(pos => TileAt(pos));
  }

  public string StringCol(int colNum) {
    return String.Join("", Col(colNum).Select(tile => (char)tile));
  }

  public override string ToString() {
    var rows = Tiles
      .Select(tile => (char)tile)
      .Chunk(Width)
      .Select(row => String.Join("", row));
    return String.Join("\n", rows.ToArray());
  }

  public static string Display(IEnumerable<Map> maps) {
    var maxHeight = maps.Max(map => {
        return map.Height;
    });
    var lines = new List<string>();
    for(var i = 0; i < maxHeight; i++) {
      lines.Add(String.Join(" ", maps.Select(map => map.StringRow(i))));
    }
    return String.Join("\n", lines);
  }
}
