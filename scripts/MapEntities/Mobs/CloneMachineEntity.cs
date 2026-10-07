using GalaxyGauntlet.scripts.MapEntities.Shared;
using System;

namespace GalaxyGauntlet.scripts.MapEntities.Mobs
{
    public class CloneMachineEntity(Vector2I coordinate, int entityDataCode) : MapMob(coordinate)
    {
        public override int DataCode => entityDataCode;

        public override string Name => "Clone Machine Mob";

        public bool CreateClone = false;

        public override Vector2I TextureCoordinate => DataCode switch
        {
            Constants.ByteCodes.Entities.Blob => Constants.SpriteCoordinates.Blob,
            Constants.ByteCodes.Entities.BlueTank => Orientation switch
            {
                EntityOrientation.North => Constants.SpriteCoordinates.BlueTank_N,
                EntityOrientation.East => Constants.SpriteCoordinates.BlueTank_E,
                EntityOrientation.South => Constants.SpriteCoordinates.BlueTank_S,
                EntityOrientation.West => Constants.SpriteCoordinates.BlueTank_W,
                _ => throw new NotImplementedException()
            },
            Constants.ByteCodes.Entities.Bug => Orientation switch
            {
                EntityOrientation.North => Constants.SpriteCoordinates.Bug_N,
                EntityOrientation.East => Constants.SpriteCoordinates.Bug_E,
                EntityOrientation.South => Constants.SpriteCoordinates.Bug_S,
                EntityOrientation.West => Constants.SpriteCoordinates.Bug_W,
                _ => throw new NotImplementedException()
            },
            Constants.ByteCodes.Entities.DirtBlock => Constants.SpriteCoordinates.DirtBlock,
            Constants.ByteCodes.Entities.Fireball => Orientation switch
            {
                EntityOrientation.North => Constants.SpriteCoordinates.Fireball_N,
                EntityOrientation.East => Constants.SpriteCoordinates.Fireball_E,
                EntityOrientation.South => Constants.SpriteCoordinates.Fireball_S,
                EntityOrientation.West => Constants.SpriteCoordinates.Fireball_W,
                _ => throw new NotImplementedException()
            },
            Constants.ByteCodes.Entities.Glider => Orientation switch
            {
                EntityOrientation.North => Constants.SpriteCoordinates.Glider_N,
                EntityOrientation.East => Constants.SpriteCoordinates.Glider_E,
                EntityOrientation.South => Constants.SpriteCoordinates.Glider_S,
                EntityOrientation.West => Constants.SpriteCoordinates.Glider_W,
                _ => throw new NotImplementedException()
            },
            Constants.ByteCodes.Entities.Paramecium => Orientation switch
            {
                EntityOrientation.North => Constants.SpriteCoordinates.Paramecium_N,
                EntityOrientation.East => Constants.SpriteCoordinates.Paramecium_E,
                EntityOrientation.South => Constants.SpriteCoordinates.Paramecium_S,
                EntityOrientation.West => Constants.SpriteCoordinates.Paramecium_W,
                _ => throw new NotImplementedException()
            },
            Constants.ByteCodes.Entities.Teeth => Orientation switch
            {
                EntityOrientation.North => Constants.SpriteCoordinates.Teeth_N,
                EntityOrientation.East => Constants.SpriteCoordinates.Teeth_E,
                EntityOrientation.South => Constants.SpriteCoordinates.Teeth_S,
                EntityOrientation.West => Constants.SpriteCoordinates.Teeth_W,
                _ => throw new NotImplementedException()
            },
            Constants.ByteCodes.Entities.Walker => Orientation switch
            {
                EntityOrientation.North => Constants.SpriteCoordinates.Walker_N,
                EntityOrientation.East => Constants.SpriteCoordinates.Walker_E,
                EntityOrientation.South => Constants.SpriteCoordinates.Walker_S,
                EntityOrientation.West => Constants.SpriteCoordinates.Walker_W,
                _ => throw new NotImplementedException()
            },
            _ => throw new NotImplementedException()
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
            GameData.AddMapMob(mob);
            CreateClone = false;
        }
    }
}
