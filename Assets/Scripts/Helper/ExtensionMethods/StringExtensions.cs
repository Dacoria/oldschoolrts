using System.Globalization;

public static class StringExtensions
{
    public static string TitleCase(this string text)
    {
        var textInfo = new CultureInfo("en-US", false).TextInfo;
        var test = textInfo.ToTitleCase(text.ToLower());
        return textInfo.ToTitleCase(text.ToLower());
    }  

    public static string Capitalize(this string input, bool everyWord = true, string delimiter = "_")
    {
        if (everyWord)
        {
            var words = input.Split(delimiter);
            var result = words[0].Capitalize(false); ;
            for (int i = 1; i < words.Length; i++)
            {
                result += delimiter;
                result += words[i].Capitalize(false);                
                
            }

            return result;
        }
        else
        {
            return string.Concat(input[0].ToString().ToUpper(), input.ToLower().Substring(1, input.Length - 1));
        }        
    }
}