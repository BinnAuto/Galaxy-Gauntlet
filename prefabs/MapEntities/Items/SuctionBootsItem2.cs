public partial class SuctionBootsItem2(Vector2I coordinate) : MapItem2(coordinate)
{
	public override string MapObjectName => "Suction Boots";

	public override int EntityID => GGConstants.ByteCodes.Entities.SuctionBoots;

	public override int SpriteFrameIndex => GGConstants.SpriteFrameIndexes.SuctionBoots;
}
