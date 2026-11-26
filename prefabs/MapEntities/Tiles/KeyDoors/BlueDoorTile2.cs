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

    public override bool CanCreatureOpenDoor(MapLynxCreature2 creature, bool consumeItem)
    {
        if (creature is not LynxPlayer2)
        {
            return false;
        }

        int keyID = GGConstants.ByteCodes.Entities.BlueKey;
        return consumeItem
            ? creature.ConsumeItem(keyID)
            : creature.HasItem(keyID);
    }
}
