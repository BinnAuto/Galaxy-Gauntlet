using Archipelago.MultiClient.Net.Enums;
using GalaxyGauntlet.scripts;

public partial class ChipItem2(Vector2I coordinate) : MapItem2(coordinate)
{
	public override string MapObjectName => "Chip";

	public override int EntityID => GGConstants.ByteCodes.Entities.Chip;

	public override int SpriteFrameIndex
    {
        get
        {
            return (ArchipelagoSprite is null)
                ? GGConstants.SpriteFrameIndexes.Chip
                : GGConstants.SpriteFrameIndexes.ExtraChip;
        }
    }

    private AnimatedSprite2D ArchipelagoSprite = null;

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

            int spriteFrameIndex = GGConstants.ArchipelagoItemSpriteIndexes.Useful;
            switch(GameData.Archipelago.ChipLocationDisplay)
            {
                case GGConstants.Archipelago.ChipLocationDisplayIndexes.True:
                    spriteFrameIndex = item switch
                    {
                        ItemFlags.Advancement => GGConstants.ArchipelagoItemSpriteIndexes.Progression,
                        ItemFlags.Trap => GGConstants.ArchipelagoItemSpriteIndexes.Trap,
                        ItemFlags.None => GGConstants.ArchipelagoItemSpriteIndexes.Filler,
                        ItemFlags.NeverExclude => GGConstants.ArchipelagoItemSpriteIndexes.Useful,
                        _ => GGConstants.ArchipelagoItemSpriteIndexes.Filler
                    };
                    break;

                case GGConstants.Archipelago.ChipLocationDisplayIndexes.Vague:
                    break;

                case GGConstants.Archipelago.ChipLocationDisplayIndexes.Hidden:
                    return;
            }

            ArchipelagoSprite = new()
            {
                SpriteFrames = GameData.ArchipelagoSpriteFrames,
                TextureFilter = TextureFilterEnum.Nearest,
                ZAsRelative = true,
                Frame = spriteFrameIndex
            };
            AddChild(ArchipelagoSprite);
            ArchipelagoSprite.Position = Vector2.Zero;
        }
    }


	public string GetArchipelagoLocationName()
	{
		return $"Map {GameData.Level.CurrentLevelIndex + 1} - {GameData.Level.Name} Chip {Coordinate}";
	}
}
