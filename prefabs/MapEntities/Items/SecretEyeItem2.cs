public partial class SecretEyeItem2(Vector2I coordinate) : MapItem2(coordinate)
{
	public override string MapObjectName => "Secret Eye";

	public override int EntityID => GGConstants.ByteCodes.Entities.SecretEye;

	public override int SpriteFrameIndex => GGConstants.SpriteFrameIndexes.SecretEye;
}
