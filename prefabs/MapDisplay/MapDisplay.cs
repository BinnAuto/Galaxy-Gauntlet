using GalaxyGauntlet.scripts;
using GalaxyGauntlet.scripts.MapEntities;
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


	public void RenderMapData()
	{
		FloorTiles.Clear();
		ItemTiles.Clear();
		PlayerTiles.Clear();
		MobTiles.Clear();
		SceneryTiles.Clear();

		SetGlobalPosition();
		int viewportSize = GameData.Level.MapViewportSize;
		if(viewportSize % 2 == 0)
		{
			viewportSize = Mathf.Max(3, viewportSize - 1);
		}

		int cameraCenterOffset = (viewportSize - 1) / 2;
		var cameraCenter = GameData.Level.PlayerCoordinate;
		cameraCenter.X = Mathf.Min(Mathf.Max(cameraCenterOffset, cameraCenter.X), GameData.Map.Dimensions.X - cameraCenterOffset - 1);
		cameraCenter.Y = Mathf.Min(Mathf.Max(cameraCenterOffset, cameraCenter.Y), GameData.Map.Dimensions.Y - cameraCenterOffset - 1);

		for(int i = cameraCenter.X - cameraCenterOffset; i < cameraCenter.X + cameraCenterOffset + 1; i++)
		{
			for(int j = cameraCenter.Y - cameraCenterOffset; j < cameraCenter.Y + cameraCenterOffset + 1; j++)
			{
				Vector2I mapCoordinate = new(i - (cameraCenter.X - cameraCenterOffset), j - (cameraCenter.Y - cameraCenterOffset));

				if (GameData.Map.Scenery[i, j] != null)
				{
					SceneryTiles.SetCell(mapCoordinate, 0, GameData.Map.Scenery[i, j].TextureCoordinate);
				}

				if (GameData.Map.Mobs[i, j] != null)
				{
					MobTiles.SetCell(mapCoordinate, 0, GameData.Map.Mobs[i, j].TextureCoordinate);
				}

				if (GameData.Map.Players[i, j] != null)
				{
					PlayerTiles.SetCell(mapCoordinate, 0, GameData.Map.Players[i, j].TextureCoordinate);
				}

				if (GameData.Map.Items[i, j] != null)
				{
					ItemTiles.SetCell(mapCoordinate, 0, GameData.Map.Items[i, j].TextureCoordinate);
				}

				if (GameData.Map.Tiles[i, j] != null)
				{
					FloorTiles.SetCell(mapCoordinate, 0, GameData.Map.Tiles[i, j].TextureCoordinate);
					if (GameData.Map.Tiles[i, j] is CloneMachineTile)
					{
						SceneryTiles.SetCell(mapCoordinate, 0, Constants.SpriteCoordinates.CloneMachineScenery);
					}
				}
			}
		}
		GameData.Map.Rerender = false;
	}


	public void SetGlobalPosition()
	{
		int viewportSize = GameData.Level.MapViewportSize;
		int deltaFromRegular = viewportSize - GameData.Level.StandardMapViewportSize;
		Position = new Vector2I(144, 144) - (16 * deltaFromRegular * Vector2I.One);
	}
}
