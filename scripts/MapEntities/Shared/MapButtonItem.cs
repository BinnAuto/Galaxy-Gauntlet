namespace GalaxyGauntlet.scripts.MapEntities.Shared
{
    public class MapButtonItem(Vector2I coordinate, Color color) : MapItem(coordinate)
    {
        public Color Color { get; private set; } = color;

        public bool NewlyPressed { get; private set; } = false;

        protected int? CurrentEntityIDPressing = null;

        public void ProcessTick()
        {
            int? currentEntityId = null;
            var mapMob = GameData.GetMapMob(Coordinate);
            var mapPlayer = GameData.GetMapPlayer(Coordinate);
            currentEntityId = mapMob?.MobID ?? mapPlayer?.MobID;
            if (false == currentEntityId.HasValue)
            {
                currentEntityId = mapPlayer?.MobID;
            }
            if(currentEntityId is null)
            {
                NewlyPressed = false;
                CurrentEntityIDPressing = null;
                OnUnpressed();
                return;
            }
            else if(NewlyPressed)
            {
                OnHeld();
            }

            if(CurrentEntityIDPressing is null)
            {
                NewlyPressed = true;
                OnPressed();
            }
            CurrentEntityIDPressing = currentEntityId.Value;
        }


        public virtual void OnPressed() { }


        public virtual void OnUnpressed() { }


        public virtual void OnHeld() { }
    }
}
