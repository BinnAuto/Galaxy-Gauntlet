using GalaxyGauntlet.scripts;

public partial class ChipItem2(Vector2I coordinate) : MapItem2(coordinate)
{
	public override string MapObjectName => "Chip";

	public override int EntityID => GGConstants.ByteCodes.Entities.Chip;

	public override int SpriteFrameIndex => GGConstants.SpriteFrameIndexes.Chip;

	public string GetArchipelagoLocationName()
	{
		return $"Map {GameData.Level.CurrentLevelIndex + 1} - {GameData.Level.Name} Chip {Coordinate}";
	}
}
