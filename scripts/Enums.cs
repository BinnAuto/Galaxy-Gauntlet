namespace GalaxyGauntlet.scripts
{
	public enum EntityOrientation
	{
		North = GGConstants.ByteCodes.OrientationBytes.North,
		East = GGConstants.ByteCodes.OrientationBytes.East,
		South = GGConstants.ByteCodes.OrientationBytes.South,
		West = GGConstants.ByteCodes.OrientationBytes.West
	}


	public enum ItemDisplayMode
	{
		Gameplay,
		Archipelago
	}


	public enum Color
	{
		Red,
		Blue,
		Yellow,
		Green,
		Brown,
		Gray,
		Pink,
		Black
	}


	public enum IceTileType
	{
		Standard,
		Barrier_NE,
		Barrier_NW,
		Barrier_SE,
		Barrier_SW
	}
}
