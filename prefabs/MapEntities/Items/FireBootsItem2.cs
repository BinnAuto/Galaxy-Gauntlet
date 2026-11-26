using GalaxyGauntlet.scripts;

public partial class FireBootsItem2(Vector2I coordinate) : MapItem2(coordinate)
{
	public override string MapObjectName => "Fire Boots";

	public override int EntityID => GGConstants.ByteCodes.Entities.FireBoots;

	public override int SpriteFrameIndex => GGConstants.SpriteFrameIndexes.FireBoots;

    public override void _Process(double delta)
    {
        base._Process(delta);
        Modulate = GameData.Archipelago.FireBootsUnlocked
            ? Godot.Color.Color8(255, 255, 255, 255)
            : Godot.Color.Color8(255, 255, 255, 60);
    }
}
