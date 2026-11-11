public partial class GreenDoorTile2(Vector2I coordinate) : KeyDoorTile2(coordinate)
{
	public override string MapObjectName => "Green Door";

	public override int EntityID => GGConstants.ByteCodes.Entities.GreenDoor;

	public override int SpriteFrameIndex => GGConstants.SpriteFrameIndexes.GreenDoor;

	public override bool CanOpen(MapCreature2 creature)
	{
		if (creature is not Player2)
		{
			return false;
		}

		return creature.HasItem(new GreenKeyItem2(Vector2I.Zero));
	}
}
