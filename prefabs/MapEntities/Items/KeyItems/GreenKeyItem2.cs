using GalaxyGauntlet.scripts;

public partial class GreenKeyItem2(Vector2I coordinate) : KeyItem2(coordinate)
{
	public override string MapObjectName => "Green Key";

	public override int EntityID => GGConstants.ByteCodes.Entities.GreenKey;

	public override int SpriteFrameIndex => GGConstants.SpriteFrameIndexes.GreenKey;


    public override void _Process(double delta)
    {
        base._Process(delta);
        Modulate = GameData.Archipelago.GreenKeyUnlocked
            ? Godot.Color.Color8(255, 255, 255, 255)
            : Godot.Color.Color8(255, 255, 255, 60);
    }
}
