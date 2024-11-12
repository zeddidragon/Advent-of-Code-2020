public struct Vector {
  public int X;
  public int Y;

  public Vector(int x, int y) {
    X = x;
    Y = y;
  }

  // Rotate vector clockwise
  // Argument must be 0-360 and divide by 90
  public void Rotate(int dir) {
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

  public int Manhattan {
    get { return Math.Abs(X) + Math.Abs(Y); }
  }

  public static Vector operator +(Vector a) => a;
  public static Vector operator -(Vector a) => new Vector(-a.X, -a.Y);
  public static Vector operator +(Vector a, Vector b) => new Vector(a.X + b.X, a.Y + b.Y);
  public static Vector operator -(Vector a, Vector b) => new Vector(a.X - b.X, a.Y - b.Y);
  public static Vector operator *(Vector a, int b) => new Vector(a.X * b, a.Y * b);

  public override string ToString() => $"({X}, {Y})";
}

