using GalaxyGauntlet.scripts.MapEntities.Shared;
using System;

namespace GalaxyGauntlet.scripts.MapEntities.Mobs
{
	public class InvalidCloneMachineDataCodeException(int dataCode)
		: Exception($"Clone machine");

	public class CloneMachineEntity(Vector2I coordinate, int entityDataCode) : MapMob(coordinate)
	{
		public override int DataCode => entityDataCode;

		public override string Name => "Clone Machine Mob";

		public bool CreateClone = false;

		public override Vector2I TextureCoordinate => DataCode switch
		{
			GGConstants.ByteCodes.Entities.Ball => GGConstants.SpriteCoordinates.Ball,
			GGConstants.ByteCodes.Entities.Blob => GGConstants.SpriteCoordinates.Blob,
			GGConstants.ByteCodes.Entities.BlueTank => Orientation switch
			{
				EntityOrientation.North => GGConstants.SpriteCoordinates.BlueTank_N,
				EntityOrientation.East => GGConstants.SpriteCoordinates.BlueTank_E,
				EntityOrientation.South => GGConstants.SpriteCoordinates.BlueTank_S,
				EntityOrientation.West => GGConstants.SpriteCoordinates.BlueTank_W,
				_ => throw new NotImplementedException()
			},
			GGConstants.ByteCodes.Entities.Bug => Orientation switch
			{
				EntityOrientation.North => GGConstants.SpriteCoordinates.Bug_N,
				EntityOrientation.East => GGConstants.SpriteCoordinates.Bug_E,
				EntityOrientation.South => GGConstants.SpriteCoordinates.Bug_S,
				EntityOrientation.West => GGConstants.SpriteCoordinates.Bug_W,
				_ => throw new NotImplementedException()
			},
			GGConstants.ByteCodes.Entities.DirtBlock => GGConstants.SpriteCoordinates.DirtBlock,
			GGConstants.ByteCodes.Entities.Fireball => Orientation switch
			{
				EntityOrientation.North => GGConstants.SpriteCoordinates.Fireball_N,
				EntityOrientation.East => GGConstants.SpriteCoordinates.Fireball_E,
				EntityOrientation.South => GGConstants.SpriteCoordinates.Fireball_S,
				EntityOrientation.West => GGConstants.SpriteCoordinates.Fireball_W,
				_ => throw new NotImplementedException()
			},
			GGConstants.ByteCodes.Entities.Glider => Orientation switch
			{
				EntityOrientation.North => GGConstants.SpriteCoordinates.Glider_N,
				EntityOrientation.East => GGConstants.SpriteCoordinates.Glider_E,
				EntityOrientation.South => GGConstants.SpriteCoordinates.Glider_S,
				EntityOrientation.West => GGConstants.SpriteCoordinates.Glider_W,
				_ => throw new NotImplementedException()
			},
			GGConstants.ByteCodes.Entities.Paramecium => Orientation switch
			{
				EntityOrientation.North => GGConstants.SpriteCoordinates.Paramecium_N,
				EntityOrientation.East => GGConstants.SpriteCoordinates.Paramecium_E,
				EntityOrientation.South => GGConstants.SpriteCoordinates.Paramecium_S,
				EntityOrientation.West => GGConstants.SpriteCoordinates.Paramecium_W,
				_ => throw new NotImplementedException()
			},
			GGConstants.ByteCodes.Entities.Teeth => Orientation switch
			{
				EntityOrientation.North => GGConstants.SpriteCoordinates.Teeth_N,
				EntityOrientation.East => GGConstants.SpriteCoordinates.Teeth_E,
				EntityOrientation.South => GGConstants.SpriteCoordinates.Teeth_S,
				EntityOrientation.West => GGConstants.SpriteCoordinates.Teeth_W,
				_ => throw new NotImplementedException()
			},
			GGConstants.ByteCodes.Entities.Walker => Orientation switch
			{
				EntityOrientation.North => GGConstants.SpriteCoordinates.Walker_N,
				EntityOrientation.East => GGConstants.SpriteCoordinates.Walker_E,
				EntityOrientation.South => GGConstants.SpriteCoordinates.Walker_S,
				EntityOrientation.West => GGConstants.SpriteCoordinates.Walker_W,
				_ => throw new NotImplementedException()
			},
			_ => GGConstants.SpriteCoordinates.NoSprite
		};


		public override void ProcessTick()
		{
			if(false == CreateClone)
			{
				return;
			}

			var entityCoordinate = Coordinate + Forward;
			MapMob mob = (MapMob)GetMapEntity(DataCode, entityCoordinate);
			mob.Orientation = Orientation;
			GameData.Map.AddMob(mob);
			CreateClone = false;
		}
	}
}
