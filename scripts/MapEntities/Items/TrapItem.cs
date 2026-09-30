using GalaxyGauntlet.scripts.MapEntities.Shared;

namespace GalaxyGauntlet.scripts.MapEntities
{
    public class TrapItem(Vector2I coordinate) : MapItem(coordinate)
    {
        /// <summary>
        /// Determines if the trap can capture entities.
        /// </summary>
        public bool IsActive { get; private set; } = true;

        public override bool NeedsTileSpecification => false;

        public override int DataCode 
        {
            get
            {
                return IsActive
                    ? Constants.ByteCodes.Entities.Trap
                    : Constants.ByteCodes.Entities.OpenTrap;
            }
        }

        public override string Name => "Trap";

        public override Vector2I TextureCoordinate
        {
            get
            {
                return IsActive
                    ? Constants.SpriteCoordinates.ActiveTrap
                    : Constants.SpriteCoordinates.InactiveTrap;
            }
        }


        public void SetState(bool active)
        {
            if(IsActive != active)
            {
                GameData.RerenderMap = true;
            }

            IsActive = active;
        }
    }
}
