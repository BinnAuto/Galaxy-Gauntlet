using GalaxyGauntlet.scripts;

public partial class IceSkatesItem2(Vector2I coordinate) : MapItem2(coordinate)
{
	public override string MapObjectName => "Ice Skates";

	public override int EntityID => GGConstants.ByteCodes.Entities.IceSkates;

	public override int SpriteFrameIndex => GGConstants.SpriteFrameIndexes.IceSkates;

    public override void _Process(double delta)
    {
        base._Process(delta);
        Modulate = GameData.Archipelago.IceSkatesUnlocked
            ? Godot.Color.Color8(255, 255, 255, 255)
            : Godot.Color.Color8(255, 255, 255, 60);
    }
}
