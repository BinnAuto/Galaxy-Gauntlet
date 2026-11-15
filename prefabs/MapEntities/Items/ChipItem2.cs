using Archipelago.MultiClient.Net.Enums;
using GalaxyGauntlet.scripts;

public partial class ChipItem2(Vector2I coordinate) : MapItem2(coordinate)
{
	public override string MapObjectName => "Chip";

	public override int EntityID => GGConstants.ByteCodes.Entities.Chip;

	public override int SpriteFrameIndex => GGConstants.SpriteFrameIndexes.Chip;

    public override void _Ready()
    {
        base._Ready();

        if(GameData.GameMode == GameMode.Archipelago)
        {
            string locationName = GetArchipelagoLocationName();
            long locationId = GameData.Archipelago.GetLocationIdByName(locationName);
            var item = GameData.Archipelago.GetItemTypeAtLocation(locationId);
            if(item is null || GameData.Archipelago.IsLocationChecked(locationId))
            {
                return;
            }

            int spriteFrameIndex = item switch
            {
                ItemFlags.Advancement => GGConstants.ArchipelagoItemSpriteIndexes.Progressive,
                ItemFlags.Trap => GGConstants.ArchipelagoItemSpriteIndexes.Trap,
                ItemFlags.None => GGConstants.ArchipelagoItemSpriteIndexes.Filler,
                ItemFlags.NeverExclude => GGConstants.ArchipelagoItemSpriteIndexes.Helpful,
                _ => GGConstants.ArchipelagoItemSpriteIndexes.Filler
            };
            AnimatedSprite2D archipelagoSprite = new()
            {
                SpriteFrames = (SpriteFrames)GD.Load("res://sprite_frames/archipelago_tile_sprites.tres"),
                TextureFilter = TextureFilterEnum.Nearest,
                ZIndex = ZIndex + 1,
                Frame = spriteFrameIndex
            };
            AddChild(archipelagoSprite);
            archipelagoSprite.Position = Vector2.Zero;
        }
    }


	public string GetArchipelagoLocationName()
	{
		return $"Map {GameData.Level.CurrentLevelIndex + 1} - {GameData.Level.Name} Chip {Coordinate}";
	}
}
