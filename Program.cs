void Main() {
  Day[] advent = {
    new Day01(),
    new Day02(),
    new Day03(),
    new Day04(),
    new Day05(),
    new Day06(),
    new Day07(),
    new Day08(),
    new Day09(),
    new Day10(),
    new Day11(),
    new Day12(),
    new Day13(),
    new Day14(),
    new Day15(),
    new Day16(),
    new Day17(),
    new Day18(),
  };

  foreach(var day in advent) {
    Console.WriteLine("# Day {0}", day.day);
    Console.WriteLine("## Part 1", day.day);
    Console.WriteLine(day.Part1());
    Console.WriteLine("## Part 2", day.day);
    Console.WriteLine(day.Part2());
    Console.WriteLine("");
  }
}

Main();
