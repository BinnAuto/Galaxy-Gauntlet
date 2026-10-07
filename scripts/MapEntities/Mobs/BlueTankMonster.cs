using GalaxyGauntlet.scripts.MapEntities.Shared;
using System;

namespace GalaxyGauntlet.scripts.MapEntities
{
	public class BlueTankMonster(Vector2I coordinate) : MapMob(coordinate)
	{
		public override int DataCode => Constants.ByteCodes.Entities.BlueTank;

		public override string Name => "Blue Tank";

		private bool _isStopped = false;

		public override Vector2I TextureCoordinate
		{
			get
			{
				return Orientation switch
				{
					EntityOrientation.North => Constants.SpriteCoordinates.BlueTank_N,
					EntityOrientation.East => Constants.SpriteCoordinates.BlueTank_E,
					EntityOrientation.South => Constants.SpriteCoordinates.BlueTank_S,
					EntityOrientation.West => Constants.SpriteCoordinates.BlueTank_W,
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

            var currentTile = GameData.GetMapTile(Coordinate);

            #region Force Floor

            if (currentTile is MapForceFloorTile forceFloor)
            {
                GameData.AddToSlipList(this);
                var forceFloorInfluence = forceFloor.GetInfluence();
                var forceCoordinate = ProposeMove(forceFloorInfluence);
                SetOrientationAndCoordinate(forceFloorInfluence, forceCoordinate);
                return;
            }

            #endregion

            #region Ice Floor

            if (currentTile is MapIceTile iceTile)
            {
                GameData.AddToSlipList(this);
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

            GameData.RemoveFromSlipList(this);

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
