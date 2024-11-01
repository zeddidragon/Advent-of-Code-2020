﻿void Main() {
  Day[] advent = {
    new Day1(),
    new Day2(),
    new Day3(),
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
