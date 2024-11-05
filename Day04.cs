using System.Text.RegularExpressions;

struct Passport {
  static Regex rex = new Regex(@"\b((?<Key>\w{3}):(?<Val>#?\w+))\b");
  // byr (Birth Year)
  // iyr (Issue Year)
  // eyr (Expiration Year)
  // hgt (Height)
  // hcl (Hair Color)
  // ecl (Eye Color)
  // pid (Passport ID)
  // cid (Country ID)
  string? byr;
  string? iyr;
  string? eyr;
  string? hgt;
  string? hcl;
  string? ecl;
  string? pid;
  string? cid;

  public void scan(string line) {
    var match = rex.Match(line);
    while(match.Success) {
      var key = match.Groups["Key"].Value;
      var val = match.Groups["Val"].Value;
      switch(key) {
        case "byr":
          byr = val;
          break;
        case "iyr":
          iyr = val;
          break;
        case "eyr":
          eyr = val;
          break;
        case "hgt":
          hgt = val;
          break;
        case "hcl":
          hcl = val;
          break;
        case "ecl":
          ecl = val;
          break;
        case "pid":
          pid = val;
          break;
        case "cid":
          cid = val;
          break;
        default:
          Console.WriteLine("Unhandled: {0}:{1}", key, val);
          break;
      }
      match = match.NextMatch();
    }
  }

  // Validate all fields being present except CID
  public bool IsValidIsh {
    get {
      return
        !( byr == null
        || iyr == null
        || eyr == null
        || hgt == null
        || hcl == null
        || ecl == null
        || pid == null
      );
    }
  }

  private static Regex yearRex =  new Regex(@"\b(?<Year>\d{4})\b");
  private bool IsValidYear(string? str, uint min, uint max) {
    if(str == null) {
      return false;
    }
    if(!yearRex.IsMatch(str)) {
      return false;
    }
    var year = uint.Parse(str);
    return year >= min && year <= max;
  }

  private static Regex heightRex =  new Regex(@"\b(?<Height>\d+)(?<Unit>(cm|in))\b");
  private bool IsValidHeight(string? str) {
    if(str == null) {
      return false;
    }
    var match = heightRex.Match(str);
    if(!match.Success) {
      return false;
    }
    var height = uint.Parse(match.Groups["Height"].Value);
    var unit = match.Groups["Unit"].Value;
    switch(unit) {
      case "cm": 
        return height >= 150 && height <= 193;
      case "in":
        return height >= 59 && height <= 76; 
      default:
        return false;
    }
  }

  private static Regex hairRex = new Regex(@"#(?<Color>[a-fA-F0-9]{6})\b");
  private bool IsValidHair(string? str) {
    if(str == null) {
      return false;
    }
    return hairRex.IsMatch(str);
  }

  private static Regex eyeRex = new Regex(@"\b(amb|blu|brn|gry|grn|hzl|oth)\b");
  private bool IsValidEyes(string? str) {
    if(str == null) {
      return false;
    }
    return eyeRex.IsMatch(str);
  }

  private static Regex pidRex =  new Regex(@"\b(\d{9}\b)");
  private bool IsValidPid(string? str) {
    if(str == null) {
      return false;
    }
    return pidRex.IsMatch(str);
  }


  public bool IsValid {
    get {
      return IsValidYear(byr, 1920, 2002)
        && IsValidYear(iyr, 2010, 2020)
        && IsValidYear(eyr, 2020, 2030)
        && IsValidHeight(hgt)
        && IsValidHair(hcl)
        && IsValidEyes(ecl)
        && IsValidPid(pid);
    }
  }

  public override string ToString() {
    return $"""
      Birth Year: {byr}
      Issue Year: {iyr}
      Expires:    {eyr}
      Height:     {hgt}
      Hair Color  {hcl}
      Eye Color   {ecl}
      Passport ID {pid}
      Country  ID {cid}
    """;
  }
}

public class Day4 : Day {
  public Day4() : base(4) {
  }

  IEnumerable<Passport> GetPassports() {
    var passport = new Passport();
    foreach(var line in input) {
      if(line == "") {
        yield return passport;
        passport = new Passport();
      } else {
        passport.scan(line);
      }
    }
    yield return passport;
  }

  public override string Part1() {
    // How many passports have all required fields _filled in_?
    var valid = 0;
    foreach(var passport in GetPassports()) {
      if(passport.IsValidIsh) {
        valid++;
      }
    }
    return $"{valid}";
  }

  public override string Part2() {
    // How many passports have all required fields _valid_?
    var valid = 0;
    foreach(var passport in GetPassports()) {
      if(passport.IsValid) {
        valid++;
      }
    }
    return $"{valid}";
  }
}
