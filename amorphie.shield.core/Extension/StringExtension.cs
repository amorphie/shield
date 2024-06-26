
namespace amorphie.shield.Extension;

public static class StringExtension
{
    public static string FirstCharToUpper(this string input)
    {
        return input switch
        {
            null => "",
            "" => "",
            _ => string.Concat(input[0].ToString().ToUpper(), input.AsSpan(1))
        };
    }

}
