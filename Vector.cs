public struct Vector : IEquatable<Vector> {
  public int X, Y;

  public Vector(int x, int y) {
    X = x;
    Y = y;
  }
  public Vector(Vector b) {
    X = b.X;
    Y = b.Y;
  }

  // Rotate vector clockwise
  // Argument must be 0-360 and divide by 90
  public void Rotate(int dir = 90) {
    switch(dir) {
      case 0:
      case 360:
        break;
      case 90: {
        var tmp = X;
        X = -Y;
        Y = tmp;
        break;
      }
      case 180: {
        X = -X;
        Y = -Y;
        break;
      }
      case 270: {
        var tmp = X;
        X = Y;
        Y = -tmp;
        break;
      }
    }
  }

  public int Manhattan { get => Math.Abs(X) + Math.Abs(Y); }

  public static Vector operator +(Vector a) => a;
  public static Vector operator -(Vector a) => new Vector(-a.X, -a.Y);
  public static Vector operator +(Vector a, Vector b) => new Vector(a.X + b.X, a.Y + b.Y);
  public static Vector operator -(Vector a, Vector b) => new Vector(a.X - b.X, a.Y - b.Y);
  public static Vector operator *(Vector a, int b) => new Vector(a.X * b, a.Y * b);
  public static Vector operator *(Vector a, Vector b) => new Vector(a.X * b.X, a.Y * b.Y);
  public static bool operator ==(Vector a, Vector b) => a.X == b.X && a.Y == b.Y;
  public static bool operator !=(Vector a, Vector b) => a.X != b.X || a.Y != b.Y;

  public override bool Equals(Object? o) => o is Vector && Equals((Vector)o);
  public bool Equals(Vector b) => X == b.X && Y == b.Y;

  // This hash code is in general a bit prone to collisions
  // However, in practice most values are 16 bits or less
  public override int GetHashCode() => (Int16)X ^ ((Int16)Y << 16);
  public override string ToString() => $"({X},{Y})";
}

