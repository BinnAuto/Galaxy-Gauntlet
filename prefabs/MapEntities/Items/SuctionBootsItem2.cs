using GalaxyGauntlet.scripts;

public partial class SuctionBootsItem2(Vector2I coordinate) : MapItem2(coordinate)
{
	public override string MapObjectName => "Suction Boots";

	public override int EntityID => GGConstants.ByteCodes.Entities.SuctionBoots;

	public override int SpriteFrameIndex => GGConstants.SpriteFrameIndexes.SuctionBoots;

    public override void _Process(double delta)
    {
        base._Process(delta);
        Modulate = GameData.Archipelago.SuctionBootsUnlocked
            ? Godot.Color.Color8(255, 255, 255, 255)
            : Godot.Color.Color8(255, 255, 255, 60);
    }
}
