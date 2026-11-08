using GalaxyGauntlet.scripts.MapEntities.Shared;
using System;

namespace GalaxyGauntlet.scripts.MapEntities
{
	public class BlueTankMonster(Vector2I coordinate) : MapMob(coordinate)
	{
		public override int DataCode => GGConstants.ByteCodes.Entities.BlueTank;

		public override string Name => "Blue Tank";

		private bool _isStopped = false;

		public override Vector2I TextureCoordinate
		{
			get
			{
				return Orientation switch
				{
					EntityOrientation.North => GGConstants.SpriteCoordinates.BlueTank_N,
					EntityOrientation.East => GGConstants.SpriteCoordinates.BlueTank_E,
					EntityOrientation.South => GGConstants.SpriteCoordinates.BlueTank_S,
					EntityOrientation.West => GGConstants.SpriteCoordinates.BlueTank_W,
					_ => throw new NotImplementedException()
				};
			}
        }


        public override MapMob CreateCopy()
        {
            var entityCoordinate = Coordinate.Clone();
            BlueTankMonster result = new(entityCoordinate)
            {
                Orientation = Orientation
            };
            return result;
        }


        public override void ProcessTick()
        {
            if(_isStopped)
			{
				return;
            }

            if (CheckIfOnTrap(Forward))
            {
				// Tanks are not permanently stopped if on a trap
                return;
            }

            var currentTile = GameData.Map.GetTile(Coordinate);

            #region Force Floor

            if (currentTile is MapForceFloorTile forceFloor)
            {
                GameData.Map.AddToSlipList(this);
                var forceFloorInfluence = forceFloor.GetInfluence();
                var forceCoordinate = ProposeMove(forceFloorInfluence);
                SetOrientationAndCoordinate(forceFloorInfluence, forceCoordinate);
                return;
            }

            #endregion

            #region Ice Floor

            if (currentTile is MapIceTile iceTile)
            {
                GameData.Map.AddToSlipList(this);
                Orientation = iceTile.SetEntityOrientation(Orientation);
                var iceCoordinate = ProposeMove(Forward);
                if (iceCoordinate == Coordinate)
                {
                    ReverseOrientation();
                    Orientation = iceTile.SetEntityOrientation(Orientation);
                    iceCoordinate = ProposeMove(Forward);
                }
                SetCoordinate(iceCoordinate);
                return;
            }

            #endregion

            GameData.Map.RemoveFromSlipList(this);

            var previousCoordinate = Coordinate.Clone();
			base.ProcessTick();
			if(previousCoordinate == Coordinate)
			{
				_isStopped = true;
			}
        }


		public void AboutFace()
		{
			_isStopped = false;
			ReverseOrientation();
		}
	}
}
