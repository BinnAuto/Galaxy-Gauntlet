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
                if(GameData.GameMode == GameMode.Archipelago)
                {
                    string locationName = GetArchipelagoLocationName();
                    long locationId = GameData.GetAPLocationId(locationName);
                    var item = GameData.GetAPItemTypeAtLocation(locationId);
                    if(item is null)
                    {
                        return Constants.SpriteCoordinates.Chip;
                    }
                    if(GameData.IsAPLocationChecked(locationId))
                    {
                        return Constants.SpriteCoordinates.Chip;
                    }
                    return item switch
                    {
                        ItemFlags.Advancement => Constants.SpriteCoordinates.ProgressiveArchipelagoItem,
                        ItemFlags.Trap => Constants.SpriteCoordinates.ArchipelagoTrap,
                        ItemFlags.None => Constants.SpriteCoordinates.ArchipelagoFiller,
                        ItemFlags.NeverExclude => Constants.SpriteCoordinates.ArchipelagoItem,
                        _ => Constants.SpriteCoordinates.ArchipelagoFiller
                    };
                }

                return Constants.SpriteCoordinates.Chip;
            }
        }


        public string GetArchipelagoLocationName()
        {
            return $"{GameData.LevelName} Chip {Coordinate}";
        }
    }
}
