public class Day13 : Day {
  public Day13() : base(13) {
  }

  public override string Part1() {
    int leaving = int.Parse(input[0]);
    int[] buses = input[1]
      .Split(",")
      .Where(x => x != "x")
      .Select(x => int.Parse(x))
      .OrderBy(x => x - (leaving % x))
      .ToArray();
    var bus = buses[0];
    var wait = bus - (leaving % bus);
    // Console.WriteLine(String.Join(",", buses));
    // Console.WriteLine(String.Join(",", buses.Select(x => x - (leaving % x))));
    // Console.WriteLine($"#{bus} ({wait})");
    return $"{bus * wait}";
  }

  private struct Bus {
    public long Id;
    public long Offset;
    public Bus(long id, long offset) {
      Id = id;
      Offset = offset;
    }

    public Bus Synced(Bus b) {
      long? cycleCount = null;
      long? inverseOffset = null;
      long max = Id * b.Id;
      for(long i = Id; i <= max; i += Id) {
        if(i % b.Id == 0) {
          cycleCount = i;
          break;
        }
      }

      if(cycleCount == null) {
        throw new Exception($"No cycle found ({Id}, {b.Id} | {max})");
      }

      for(long i = Id - Offset; i <= max; i += Id) {
        if((cycleCount - i) % b.Id == (b.Offset % b.Id)) {
          inverseOffset = i;
          break;
        }
      }

      if(inverseOffset == null) {
        throw new Exception("No inverse offset found");
      }

      long offset = (long)cycleCount - (long)inverseOffset;
      
      return new Bus((long)cycleCount, offset);
    }

    public override string ToString() {
      return $"#({Id} - {Offset})";
    }
  }

  public override string Part2() {
    Bus[] buses = input[1]
      .Split(",")
      .Select((line, index) => {
        long id = 0;
        long.TryParse(line, out id);
        return new Bus(id, index);
      })
      .Where(bus => bus.Id > 0)
      .OrderBy(bus => -bus.Id)
      .ToArray();
    // For one bus, their correct time come in cycles that can be described as x * Id - Offset
    // For two buses, there exists a third aggregate bus that lines up with both in a similar manner, x * Id - Offset
    // We calculate this imaginary bus for the first pair of buses, then use that aggregate bus to find the same
    // for each further bus down the line.
    var aggr = buses[0];
    foreach(var bus in buses.Skip(1)) {
      aggr = aggr.Synced(bus);
    }

    return $"{aggr.Id - aggr.Offset}";
  }
}
