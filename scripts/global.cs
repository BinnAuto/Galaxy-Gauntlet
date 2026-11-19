global using Godot;
global using GalaxyGauntlet.Common;
global using System.IO;
using GalaxyGauntlet.scripts;
using System.Runtime.CompilerServices;

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


    #region Assert Methods

    public static void AssertTrue(bool value, string message)
    {
        if(false == value)
        {
            OnAssertionFailed(message);
        }
    }


    public static void AssertEqual(bool expected, bool actual, string message)
    {
        if(expected != actual)
        {
            OnAssertionFailed(message);
        }
    }


    public static void AssertEqual(Vector2I expected, Vector2I actual, string message)
    {
        if (expected != actual)
        {
            OnAssertionFailed(message);
        }
    }


    public static void OnAssertionFailed(string message)
    {
        GD.Print($"[ASSERTION FAILED] - {message}");
    }

    #endregion


    public static bool IsDiagonal(this Vector2I direction)
    {
        return direction.X != 0 && direction.Y != 0;
    }


    public static Vector2I XOR(this Vector2I a, Vector2I b)
    {
        Vector2I result = Vector2I.Zero;
        if(a.X != b.X && (a.X == -1 || b.X == -1))
        {
            result += new Vector2I(-1, 0);
        }
        if (a.X != b.X && (a.X == 1 || b.X == 1))
        {
            result += new Vector2I(1, 0);
        }
        if (a.Y != b.Y && (a.Y == -1 || b.Y == -1))
        {
            result += new Vector2I(0, -1);
        }
        if (a.Y != b.Y && (a.Y == 1 || b.Y == 1))
        {
            result += new Vector2I(0, 1);
        }
        return result;
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
}