using GalaxyGauntlet.scripts.MapEntities.Shared;
using Archipelago.MultiClient.Net.Enums;

namespace GalaxyGauntlet.scripts.MapEntities
{
    public class ChipItem(int dataCode, Vector2I coordinate) : MapItem(coordinate)
    {
        public override int DataCode => dataCode;

        public override string Name => "Chip";

        public override Vector2I TextureCoordinate
        {
            get
            {
                if(GameData.Level.GameMode == GameMode.Archipelago)
                {
                    string locationName = GetArchipelagoLocationName();
                    long locationId = GameData.Archipelago.GetLocationIdByName(locationName);
                    var item = GameData.Archipelago.GetItemTypeAtLocation(locationId);
                    if(item is null)
                    {
                        return GGConstants.SpriteCoordinates.Chip;
                    }
                    if(GameData.Archipelago.IsLocationChecked(locationId))
                    {
                        return GGConstants.SpriteCoordinates.Chip;
                    }
                    return item switch
                    {
                        ItemFlags.Advancement => GGConstants.SpriteCoordinates.ProgressiveArchipelagoItem,
                        ItemFlags.Trap => GGConstants.SpriteCoordinates.ArchipelagoTrap,
                        ItemFlags.None => GGConstants.SpriteCoordinates.ArchipelagoFiller,
                        ItemFlags.NeverExclude => GGConstants.SpriteCoordinates.ArchipelagoItem,
                        _ => GGConstants.SpriteCoordinates.ArchipelagoFiller
                    };
                }

                return GGConstants.SpriteCoordinates.Chip;
            }
        }


        public string GetArchipelagoLocationName()
        {
            return $"Map {GameData.Level.CurrentLevelIndex+1} - {GameData.Level.Name} Chip {Coordinate}";
        }
    }
}
