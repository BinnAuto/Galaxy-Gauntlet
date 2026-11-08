using GalaxyGauntlet.scripts.MapEntities.Shared;

namespace GalaxyGauntlet.scripts.MapEntities
{
	public class DirtBlockEntity(Vector2I coordinate) : MapMob(coordinate)
	{
		public override int DataCode => GGConstants.ByteCodes.Entities.DirtBlock;

		public override string Name => "Dirt Block";

		public override bool CanMoveSelf => false;

		public override bool DiesToFire => false;

		public override Vector2I TextureCoordinate
		{
			get
			{
				return GameData.Level.XRayMode
					? GGConstants.SpriteCoordinates.DirtBlock_XRay
					: GGConstants.SpriteCoordinates.DirtBlock;
			}
		}


		public override MapMob CreateCopy()
		{
			var entityCoordinate = Coordinate.Clone();
			DirtBlockEntity result = new(entityCoordinate)
			{
				Orientation = Orientation
			};
			return result;
		}
	}
}
