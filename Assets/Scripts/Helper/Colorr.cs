using UnityEngine;
using System.Collections.Generic;

public static class Colorr
{

    public static Color Orange = CreateColor(243, 154, 36);
    public static Color Red = Color.red;
    public static Color DarkRed = CreateColor(147, 28, 16);
    public static Color Blue = Color.blue;
    public static Color LightBlue = CreateColor(25, 161, 229);
    public static Color DarkBlue = CreateColor(19, 78, 142);
    public static Color Black = Color.black;
    public static Color White = Color.white;
    public static Color Cyan = Color.cyan;
    public static Color GrassGreen = CreateColor(55, 173, 57);
    public static Color Green = Color.green;
    public static Color Yellow = Color.yellow;
    public static Color Gray = Color.gray;
    public static Color Pink = CreateColor(224, 66, 198);
    public static Color Purple = CreateColor(128, 27, 197);


    private static Color CreateColor(int r, int g, int b, int a = 255) => new Color(r / 255f, g / 255f, b / 255f, a / 255f);

    public static Color SetA(this Color c, float a) => new Color(c.r / 255f, c.g / 255f, c.b / 255f, a);

}