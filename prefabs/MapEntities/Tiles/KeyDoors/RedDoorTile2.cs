public partial class RedDoorTile2(Vector2I coordinate) : KeyDoorTile2(coordinate)
{
	public override string MapObjectName => "Red Door";

	public override int EntityID => GGConstants.ByteCodes.Entities.RedDoor;

	public override int SpriteFrameIndex => GGConstants.SpriteFrameIndexes.RedDoor;

	public override bool CanOpen(MapCreature2 creature)
	{
		if (creature is not Player2)
		{
			return false;
		}

		return creature.UseItem(new RedKeyItem2(Vector2I.Zero));
	}
}
