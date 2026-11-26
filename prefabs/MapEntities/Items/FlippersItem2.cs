using GalaxyGauntlet.scripts;

public partial class FlippersItem2(Vector2I coordinate) : MapItem2(coordinate)
{
	public override string MapObjectName => "Flippers";

	public override int EntityID => GGConstants.ByteCodes.Entities.Flippers;

	public override int SpriteFrameIndex => GGConstants.SpriteFrameIndexes.Flippers;

    public override void _Process(double delta)
    {
        base._Process(delta);
        Modulate = GameData.Archipelago.FlippersUnlocked
            ? Godot.Color.Color8(255, 255, 255, 255)
            : Godot.Color.Color8(255, 255, 255, 60);
    }
}
