public partial class MapTile2(Vector2I coordinate) : MapObject2(coordinate)
{
    public override void _Ready()
    {
        base._Ready();
        ZIndex = 0;
    }
}
