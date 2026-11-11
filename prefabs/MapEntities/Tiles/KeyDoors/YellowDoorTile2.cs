public partial class YellowDoorTile2(Vector2I coordinate) : KeyDoorTile2(coordinate)
{
	public override string MapObjectName => "Yellow Door";

	public override int EntityID => GGConstants.ByteCodes.Entities.YellowDoor;

	public override int SpriteFrameIndex => GGConstants.SpriteFrameIndexes.YellowDoor;


	public override bool CanOpen(MapCreature2 creature)
	{
		if(creature is not Player2)
		{
			return false;
		}

		return creature.UseItem(new YellowKeyItem2(Vector2I.Zero));
	}
}
