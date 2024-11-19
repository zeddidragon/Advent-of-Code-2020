using System.Collections.Immutable;

public class Day17 : Day {
  public Day17() : base(17) {
  }

  struct VecN : IEquatable<VecN> {
    public ImmutableArray<int> Coords;

    public VecN(int[] coords) {
      Coords = coords.ToImmutableArray();
    }
    public VecN(int dimensions) {
      var coords = new int[dimensions];
      coords.Initialize();
      Coords = coords.ToImmutableArray();
    }

    public int Manhattan {
      get {
        return Coords.Sum(x => Math.Abs(x));
      }
    }

    public int X {
      get { return Coords[0]; }
    }
    public int Y {
      get { return Coords[1]; }
    }
    public int Z {
      get { return Coords[2]; }
    }
    public int W {
      get { return Coords[3]; }
    }

    // Neighbors includes self
    static int[] NeighborRow = new int[]{ -1, 0, 1 };
    public IEnumerable<VecN> Neighbors() {
      if(Coords.Length == 0) {
        yield break;
      }
      if(Coords.Length == 1) {
        foreach(var n in NeighborRow) {
          yield return new VecN(new int[]{Coords[0] + n});
        }
        yield break;
      }
      foreach(var n in NeighborRow) {
        var coords = Coords.ToArray();
        var end = Coords.Length - 1;
        coords[end] = Coords[end] + n;
        var vec = new VecN(coords);
        var subVec = new VecN(coords.Length - 1);
        foreach(var n2 in subVec.Neighbors()) {
          yield return vec + n2;
        }
      }
    }

    public static VecN operator +(VecN a) => a;
    public static VecN operator -(VecN a) {
      var coords = new int[a.Coords.Length];
      for(var i = 0; i < coords.Length; i++) {
        coords[i] = -a.Coords[i];
      }
      return new VecN(coords);
    }
    public static VecN operator +(VecN a, VecN b) {
      if(a.Coords.Length < b.Coords.Length) {
        return b + a;
      }
      var coords = new int[a.Coords.Length];
      for(var i = 0; i < b.Coords.Length; i++) {
        coords[i] = a.Coords[i] + b.Coords[i];
      }
      for(var i = b.Coords.Length; i < coords.Length; i++) {
        coords[i] = a.Coords[i];
      }
      return new VecN(coords);
    }
    public static VecN operator -(VecN a, VecN b) => a + -b;
    public static VecN operator *(VecN a, int b) {
      var coords = new int[a.Coords.Length];
      for(var i = 0; i < coords.Length; i++) {
        coords[i] = a.Coords[i] * b;
      }
      return new VecN(coords);
    }

    public override bool Equals(Object? o) {
      return o is VecN && Equals((VecN)o);
    }

    public bool Equals(VecN b) {
      if(Coords.Length != b.Coords.Length) {
        return false;
      }
      for(var i = 0; i < Coords.Length; i++) {
        if(Coords[i] != b.Coords[i]) {
          return false;
        }
      }
      return true;
    }

    public static bool operator ==(VecN a, VecN b) {
      return a.Equals(b);
    }

    public static bool operator !=(VecN a, VecN b) {
      return !a.Equals(b);
    }

    // This hash code is in general extremely prone to collisions
    // However, in practice more values in this task are very small
    public override int GetHashCode() {
      var hash = 0;
      for(var i = 0; i < Coords.Length; i++) {
        hash = hash ^ ((Coords[i] + 1024) << (i * 4));
      }
      return hash;
    }

    public VecN Clone() {
      return new VecN(Coords.ToArray());
    }

    public override string ToString() => $"({String.Join(',', Coords)})";
  }

  class MapN {
    Dictionary<VecN, Tile> Tiles;
    Tile ParseTile(char input) {
      switch(input) {
        case '#':
          return Tile.Filled;
        default:
          return Tile.Empty;
      }
    }

    public MapN() {
      Tiles = new Dictionary<VecN, Tile>();
    }

    public MapN(Dictionary<VecN, Tile> tiles) {
      Tiles = new Dictionary<VecN, Tile>(tiles);
    }

    public MapN(string[] input, int dimensionCount) {
      var width = input[0].Length;
      var height = input.Length;
      Tiles = new Dictionary<VecN, Tile>();
      var y = 0;
      foreach(var line in input) {
        var x = 0;
        foreach(var c in line) {
          var tile = ParseTile(c);
          if(tile == Tile.Filled) {
            var pos = new VecN(dimensionCount) + new VecN(new int[]{x, y});
            SetTileAt(pos, tile);
          }
          x++;
        }
        y++;
      }
    }

    // Returns every filled point along with it's neighbours
    public IEnumerable<VecN> EachPoint() {
      var seen = new HashSet<VecN>();
      foreach(var pair in Tiles) {
        foreach(var n in pair.Key.Neighbors()) {
          if(seen.Contains(n)) {
            continue;
          }
          seen.Add(n);
          yield return n;
        }
      }
    }

    public Tile TileAt(VecN pos) {
      return Tiles.GetValueOrDefault(pos, Tile.Empty);
    }

    public void SetTileAt(VecN pos, Tile tile) {
      if(tile == Tile.Empty) {
        Tiles.Remove(pos);
      } else {
        Tiles[pos] = tile;
      }
    }

    // Counts instances of `cmp` mapwide
    public int Count(Tile cmp = Tile.Filled) {
      return Tiles.Values.Count(tile => tile == cmp);
    }

    // Counts instances of `cmp` adjacent to this position
    // Position itself is not included
    // Diagonals are counted
    public int CountNeighbours(VecN pos, Tile cmp = Tile.Filled) {
      var count = 0;
      foreach(var nPos in pos.Neighbors()) {
        if(TileAt(nPos) == cmp && nPos != pos) {
          count++;
        }
      }
      return count;
    }

    public MapN Clone() {
      return new MapN(Tiles);
    }
  }

  public string Simulation(int dimensions = 3, int steps = 6) {
    var map = new MapN(input, dimensions);
    for(var i = 0; i < steps; i++) {
      var source = map.Clone();
      foreach(var point in source.EachPoint()) {
        var tile = source.TileAt(point);
        var neighbours = source.CountNeighbours(point);
        if(tile == Tile.Filled && (neighbours < 2 || neighbours > 3)) {
          map.SetTileAt(point, Tile.Empty);
        } else if(tile != Tile.Filled && neighbours == 3) {
          map.SetTileAt(point, Tile.Filled);
        }
      }
    }
    return $"{map.Count()}";
  }

  public override string Part1() {
    return Simulation(3);
  }

  public override string Part2() {
    return Simulation(4);
  }
}
