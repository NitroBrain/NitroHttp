namespace NitroHttp.Core.Rendering;

public static class Ansi
{
  public const string Reset = "\u001b[0m";

  // keys
  public const string Cyan = "\u001b[38;5;81m";

  // string Values
  public const string Green = "\u001b[38;5;114m";

  // numbers
  public const string Orange = "\u001b[38;5;215m";

  // true / false
  public const string Purple = "\u001b[38;5;141m";

  // null
  public const string Gray = "\u001b[38;5;245m";

  // braces, brackets, commas, colons
  public const string White = "\u001b[38;5;252m";
}
