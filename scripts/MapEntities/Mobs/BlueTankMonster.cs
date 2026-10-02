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
