using GalaxyGauntlet.scripts;

public partial class BlueWallTile2(bool isReal, Vector2I coordinate) : MapTile2(coordinate)
{
	public bool IsReal = isReal;

	public override string MapObjectName => IsReal
		? "Blue Wall"
		: "Fake Blue Wall";

	public override int EntityID => IsReal
		? GGConstants.ByteCodes.Entities.BlueWall
		: GGConstants.ByteCodes.Entities.FakeBlueWall;

	public override int SpriteFrameIndex
	{
		get
		{
			if(IsReal)
			{
				return GGConstants.ByteCodes.Entities.BlueWall;
			}
			return GameData.Level.XRayMode
				? GGConstants.ByteCodes.Entities.Wall
				: GGConstants.ByteCodes.Entities.BlueWall;
		}
	}
}
