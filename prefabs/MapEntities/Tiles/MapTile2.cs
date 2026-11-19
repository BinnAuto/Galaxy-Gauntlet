public class FloorStateFlags
{
    #region MSCC Flags

    public const int ButtonDown = 0x01;
    public const int Cloning = 0x02;
    public const int Broken = 0x04;
    public const int HasMutant = 0x08;
    public const int Marker = 0x10;

    #endregion


    #region Lynx Flags

    public const int Claimed = 0x40;
    public const int Animated = 0x20;
    public const int Beartrap = 0x01;
    public const int Teleport = 0x02;

    #endregion
}

public partial class MapTile2(Vector2I coordinate) : MapObject2(coordinate)
{
    public int FloorState = 0;

    public override void _Ready()
    {
        base._Ready();
        ZIndex = GGConstants.ZIndexes.Tile;
    }


    public bool HasFloorState(int value)
    {
        return (FloorState & value) == value;
    }


    public void SetFloorState(int value)
    {
        FloorState |= value;
    }


    public void RemoveFloorState(int value)
    {
        FloorState &= ~(value);
    }
}
