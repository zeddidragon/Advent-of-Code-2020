using System.Text.RegularExpressions;

public class Day14 : Day {
  private class Decoder {
    static long Mask36 = ((long)1 << 36) - 1;
    public string Mask { get; private set; } = "";
    public long Mask0 { get; private set; }
    public long Mask1 { get; private set; }
    public Dictionary<long, long> Registry = new Dictionary<long, long>();

    long ReadMask(string line, char c) {
      long mask = 0;
      for(var i = 0; i < line.Length; i++) {
        var bit = line.Length - i - 1;
        if(line[i] == c) {
          mask |= (long)1 << bit;
        }
      }
      return mask;
    }

    static Regex MemPattern = new Regex(@"\bmem\[(?<Index>\d+)\] = (?<Value>\d+)\b");
    public IEnumerable<KeyValuePair<long, long>> EachMemSet(IEnumerable<string> input) {
      foreach(var line in input) {
        if(line.StartsWith("mask = ")) {
          var l = line.Replace("mask = ", "");
          Mask0 = ~ReadMask(l, '0') & Mask36;
          Mask1 = ReadMask(l, '1') & Mask36;
          Mask = l;
          continue;
        }
        var match = MemPattern.Match(line);
        var index = int.Parse(match.Groups["Index"].Value);
        long value = long.Parse(match.Groups["Value"].Value);
        yield return new KeyValuePair<long, long>(index, value);
      }
    }

    public void ApplyFloatMask(long value, long index, string mask) {
      Registry[index] = value;
      var idx = mask.IndexOf('X');
      if(idx < 0) {
        return;
      }
      var bit = mask.Length - idx - 1;
      long flag = (long)1 << bit;
      var sliced = mask.Substring(idx + 1);
      long zeroed = (long)index & ~flag & Mask36;
      long oned = (long)index | flag;
      ApplyFloatMask(value, zeroed, sliced);
      ApplyFloatMask(value, oned, sliced);
    }

    public long Sum() {
      return Registry.Sum(x => x.Value);
    }
  }

  public Day14() : base(14) {
  }

  public override string Part1() {
    var decoder = new Decoder();
    foreach(var kv in decoder.EachMemSet(input)) {
      decoder.Registry[kv.Key] = (kv.Value & decoder.Mask0) | decoder.Mask1;
    }
    
    return $"{decoder.Sum()}";
  }

  public override string Part2() {
    var decoder = new Decoder();
    foreach(var kv in decoder.EachMemSet(input)) {
      long index = kv.Key | decoder.Mask1;
      long value = kv.Value;
      decoder.ApplyFloatMask(value, index, decoder.Mask);
    }
    return $"{decoder.Sum()}";
  }
}
