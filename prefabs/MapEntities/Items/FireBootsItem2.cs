public partial class FireBootsItem2(Vector2I coordinate) : MapItem2(coordinate)
{
	public override string MapObjectName => "Fire Boots";

	public override int EntityID => GGConstants.ByteCodes.Entities.FireBoots;

	public override int SpriteFrameIndex => GGConstants.SpriteFrameIndexes.FireBoots;
}
