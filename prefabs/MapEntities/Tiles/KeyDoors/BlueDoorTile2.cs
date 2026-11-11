public partial class BlueDoorTile2(Vector2I coordinate) : KeyDoorTile2(coordinate)
{
	public override string MapObjectName => "Blue Door";

	public override int EntityID => GGConstants.ByteCodes.Entities.BlueDoor;

	public override int SpriteFrameIndex => GGConstants.SpriteFrameIndexes.BlueDoor;

	public override bool CanOpen(MapCreature2 creature)
	{
		if (creature is not Player2)
		{
			return false;
		}

		return creature.UseItem(new BlueKeyItem2(Vector2I.Zero));
	}
}
