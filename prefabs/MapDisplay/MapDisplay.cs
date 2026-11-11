using GalaxyGauntlet.scripts;
using System;

public partial class MapDisplay : Node2D
{
	private TileMapLayer FloorTiles;
	private TileMapLayer ItemTiles;
	private TileMapLayer PlayerTiles;
	private TileMapLayer MobTiles;
	private TileMapLayer SceneryTiles;

	#region Godot Overrides

	public override void _Ready()
	{
		try
		{
			FloorTiles = (TileMapLayer)GetNode(nameof(FloorTiles));
			ItemTiles = (TileMapLayer)GetNode(nameof(ItemTiles));
			PlayerTiles = (TileMapLayer)GetNode(nameof(PlayerTiles));
			MobTiles = (TileMapLayer)GetNode(nameof(MobTiles));
			SceneryTiles = (TileMapLayer)GetNode(nameof(SceneryTiles));
		}
		catch(Exception e)
		{
			FileLogger.LogException("Error readying map display", e);
		}
	}

	#endregion


	public void SetGlobalPosition()
	{
		int viewportSize = GameData.Level.MapViewportSize;
		int deltaFromRegular = viewportSize - GameData.Level.StandardMapViewportSize;
		Position = new Vector2I(144, 144) - (16 * deltaFromRegular * Vector2I.One);
	}
}
