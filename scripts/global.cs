global using Godot;
global using GalaxyGauntlet.Common;
global using System.IO;
using System.Collections.Generic;
using GalaxyGauntlet.scripts;

public static class Global
{
    public static Vector2I Clone(this Vector2I input)
    {
        return new Vector2I(input.X, input.Y);
    }


    public static Vector2 GetLabelSize(this Label label)
    {
        return label.GetLabelSize(true);
    }




    public static Vector2I ToVector(this EntityOrientation cardinalDirection)
    {
        return cardinalDirection switch
        {
            EntityOrientation.North => new(0, -1),
            EntityOrientation.South => new(0, 1),
            EntityOrientation.East => new(1, 0),
            EntityOrientation.West or _ => new(-1, 0)
        };
    }


    public static Vector2 GetLabelSize(this Label label, bool includeStyleMargins)
    {
        string text = label.Text;
        var styleBox = label.GetThemeStylebox("normal");
        var font = label.GetThemeFont("font");
        int fontSize = label.GetThemeFontSize("font_size");
        var stringSize = font.GetMultilineStringSize(text, fontSize: fontSize);
        Vector2 result = stringSize;
        if (includeStyleMargins)
        {
            Vector2 styleMargins = Vector2.Zero;
            if (styleBox is StyleBoxFlat styleBoxFlat)
            {
                styleMargins = new(
                    styleBoxFlat.BorderWidthLeft + styleBoxFlat.BorderWidthRight + styleBoxFlat.ExpandMarginLeft + styleBoxFlat.ExpandMarginLeft,
                    styleBoxFlat.BorderWidthTop + styleBoxFlat.BorderWidthBottom + styleBoxFlat.ExpandMarginTop + styleBoxFlat.ExpandMarginBottom
                );
            }
            if (styleBox is StyleBoxTexture styleBoxTexture)
            {
                styleMargins = new(
                    styleBoxTexture.ExpandMarginLeft + styleBoxTexture.ExpandMarginLeft,
                    styleBoxTexture.ExpandMarginTop + styleBoxTexture.ExpandMarginBottom
                );
            }
            result += styleMargins;
        }
        return result;
    }


    public static string[] GetLevelList()
    {
        string levelPath = "./levels";
        Directory.CreateDirectory(levelPath);
        int levelIndex = 1;
        List<string> resultList = [];
        while (true)
        {
            string levelFile = $"./levels/map{levelIndex:000}.c2m";
            string fullPath = Path.GetFullPath(levelFile);
            if (false == File.Exists(fullPath))
            {
                break;
            }

            resultList.Add(fullPath);
            levelIndex++;
        }
        return resultList.ToArray();
    }

}