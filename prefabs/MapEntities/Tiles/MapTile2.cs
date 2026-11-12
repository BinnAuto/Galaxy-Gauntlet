public class FloorStateFlags
{
    public const int ButtonDown = 0x01;
    public const int Cloning = 0x02;
    public const int Broken = 0x04;
    public const int HasMutant = 0x08;
    public const int Marker = 0x10;
}

public partial class MapTile2(Vector2I coordinate) : MapObject2(coordinate)
{
    public int FloorState = 0;

    public override void _Ready()
    {
        base._Ready();
        ZIndex = 0;
    }


    public bool HasFloorState(int value)
    {
        return (FloorState & value) == value;
    }
}
