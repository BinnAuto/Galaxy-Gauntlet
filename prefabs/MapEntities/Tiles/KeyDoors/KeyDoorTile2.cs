public partial class KeyDoorTile2(Vector2I coordinate) : MapTile2(coordinate)
{
	/// <summary>
	/// Determines if the given creature can open the key door
	/// </summary>
	public virtual bool CanOpen(MapCreature2 creature)
	{
		return false;
	}

    /// <summary>
    /// Determines if the given creature can open the key door
    /// </summary>
    public virtual bool CanCreatureOpenDoor(MapLynxCreature2 creature, bool consumeItem)
	{
		return false;
	}
}
