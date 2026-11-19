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

    public override bool CanCreatureOpenDoor(MapLynxCreature2 creature, bool consumeItem)
    {
        if (creature is not LynxPlayer2)
        {
            return false;
        }

		int keyID = GGConstants.ByteCodes.Entities.YellowKey;
		return consumeItem
			? creature.ConsumeItem(keyID)
			: creature.HasItem(keyID);
    }
}
