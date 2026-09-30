using GalaxyGauntlet.scripts;
using GalaxyGauntlet.scripts.MapEntities;
using System;

public partial class MapDisplay : Node
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


	public void RenderMapData()
	{
		FloorTiles.Clear();
		ItemTiles.Clear();
		PlayerTiles.Clear();
		MobTiles.Clear();
		SceneryTiles.Clear();

		int viewportSize = GameData.MapViewportSize;
		if(viewportSize % 2 == 0)
		{
			viewportSize = Mathf.Max(3, viewportSize - 1);
		}

		int cameraCenterOffset = (viewportSize - 1) / 2;
		var cameraCenter = GameData.PlayerCoordinate;
		cameraCenter.X = Mathf.Min(Mathf.Max(cameraCenterOffset, cameraCenter.X), GameData.MapDimensions.X - cameraCenterOffset - 1);
		cameraCenter.Y = Mathf.Min(Mathf.Max(cameraCenterOffset, cameraCenter.Y), GameData.MapDimensions.Y - cameraCenterOffset - 1);

		for(int i = cameraCenter.X - cameraCenterOffset; i < cameraCenter.X + cameraCenterOffset + 1; i++)
		{
			for(int j = cameraCenter.Y - cameraCenterOffset; j < cameraCenter.Y + cameraCenterOffset + 1; j++)
			{
				Vector2I mapCoordinate = new(i - (cameraCenter.X - cameraCenterOffset), j - (cameraCenter.Y - cameraCenterOffset));

				if (GameData.MapScenery[i, j] != null)
				{
					SceneryTiles.SetCell(mapCoordinate, 0, GameData.MapScenery[i, j].TextureCoordinate);
				}

				if (GameData.MapMobs[i, j] != null)
				{
					MobTiles.SetCell(mapCoordinate, 0, GameData.MapMobs[i, j].TextureCoordinate);
				}

				if (GameData.MapPlayers[i, j] != null)
				{
					PlayerTiles.SetCell(mapCoordinate, 0, GameData.MapPlayers[i, j].TextureCoordinate);
				}

				if (GameData.MapItems[i, j] != null)
				{
					ItemTiles.SetCell(mapCoordinate, 0, GameData.MapItems[i, j].TextureCoordinate);
				}

				if (GameData.MapTiles[i, j] != null)
				{
					FloorTiles.SetCell(mapCoordinate, 0, GameData.MapTiles[i, j].TextureCoordinate);
					if (GameData.MapTiles[i, j] is CloneMachineTile)
					{
						SceneryTiles.SetCell(mapCoordinate, 0, Constants.SpriteCoordinates.CloneMachineScenery);
					}
				}
			}
		}
		GameData.RerenderMap = false;
	}
}
