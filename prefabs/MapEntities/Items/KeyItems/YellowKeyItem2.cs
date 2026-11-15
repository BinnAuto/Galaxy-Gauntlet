using GalaxyGauntlet.scripts;

public partial class YellowKeyItem2(Vector2I coordinate) : KeyItem2(coordinate)
{
	public override string MapObjectName => "Yellow Key";

	public override int EntityID => GGConstants.ByteCodes.Entities.YellowKey;

	public override int SpriteFrameIndex => GGConstants.SpriteFrameIndexes.YellowKey;

    public override void _Process(double delta)
    {
        base._Process(delta);
        Modulate = GameData.Archipelago.YellowKeyUnlocked
            ? Godot.Color.Color8(255, 255, 255, 255)
            : Godot.Color.Color8(255, 255, 255, 60);
    }
}
