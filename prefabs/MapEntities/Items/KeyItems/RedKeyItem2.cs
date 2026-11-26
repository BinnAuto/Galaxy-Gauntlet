using GalaxyGauntlet.scripts;

public partial class RedKeyItem2(Vector2I coordinate) : KeyItem2(coordinate)
{
	public override string MapObjectName => "Red Key";

	public override int EntityID => GGConstants.ByteCodes.Entities.RedKey;

	public override int SpriteFrameIndex => GGConstants.SpriteFrameIndexes.RedKey;


    public override void _Process(double delta)
    {
        base._Process(delta);
        Modulate = GameData.Archipelago.RedKeyUnlocked
            ? Godot.Color.Color8(255, 255, 255, 255)
            : Godot.Color.Color8(255, 255, 255, 60);
    }
}
