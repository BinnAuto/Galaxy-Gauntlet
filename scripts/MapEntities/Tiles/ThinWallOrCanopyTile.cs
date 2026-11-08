using GalaxyGauntlet.scripts.MapEntities.Shared;

namespace GalaxyGauntlet.scripts.MapEntities
{
    public class ThinWallOrCanopyTile(Vector2I coordinate, byte orientation) : MapTile(coordinate)
    {
        public override int DataCode => GGConstants.ByteCodes.Entities.ThinWallOrCanopy;

        public override string Name
        {
            get
            {
                if(_orientation == 0)
                {
                    return "Undefined Thin Wall or Canopy";
                }
                if(_orientation == 0x10)
                {
                    return "Canopy";
                }
                string orientations = string.Empty;
                if((_orientation & GGConstants.ByteCodes.ThinWallBits.North) == GGConstants.ByteCodes.ThinWallBits.North)
                {
                    orientations += "N";
                }
                if ((_orientation & GGConstants.ByteCodes.ThinWallBits.East) == GGConstants.ByteCodes.ThinWallBits.East)
                {
                    orientations += "E";
                }
                if ((_orientation & GGConstants.ByteCodes.ThinWallBits.South) == GGConstants.ByteCodes.ThinWallBits.South)
                {
                    orientations += "S";
                }
                if ((_orientation & GGConstants.ByteCodes.ThinWallBits.West) == GGConstants.ByteCodes.ThinWallBits.West)
                {
                    orientations += "W";
                }
                return $"Thin Wall ({orientations})";
            }
        }


        public override Vector2I TextureCoordinate
        {
            get
            {
                return _orientation switch
                {
                    0b00000 or 0b10000 => GGConstants.SpriteCoordinates.ThinWall_None,
                    0b00001 => GGConstants.SpriteCoordinates.ThinWall_N,
                    0b00010 => GGConstants.SpriteCoordinates.ThinWall_E,
                    0b00100 => GGConstants.SpriteCoordinates.ThinWall_S,
                    0b01000 => GGConstants.SpriteCoordinates.ThinWall_W,
                    0b00011 => GGConstants.SpriteCoordinates.ThinWall_NE,
                    0b00101 => GGConstants.SpriteCoordinates.ThinWall_NS,
                    0b00110 => GGConstants.SpriteCoordinates.ThinWall_SE,
                    0b00111 => GGConstants.SpriteCoordinates.ThinWall_NES,
                    0b01001 => GGConstants.SpriteCoordinates.ThinWall_NW,
                    0b01010 => GGConstants.SpriteCoordinates.ThinWall_EW,
                    0b01011 => GGConstants.SpriteCoordinates.ThinWall_NEW,
                    0b01100 => GGConstants.SpriteCoordinates.ThinWall_SW,
                    0b01101 => GGConstants.SpriteCoordinates.ThinWall_NSW,
                    0b01110 => GGConstants.SpriteCoordinates.ThinWall_ESW,
                    0b01111 => GGConstants.SpriteCoordinates.ThinWall_NESW,
                    _ => GGConstants.SpriteCoordinates.ThinWall_None
                };
            }
        }


        public bool AllowExit(Vector2I direction)
        {
            if(direction == Vector2I.Zero)
            {
                return true;
            }

            if (direction.Y < 0 && ((_orientation & GGConstants.ByteCodes.ThinWallBits.North) == GGConstants.ByteCodes.ThinWallBits.North))
            {
                return false;
            }
            if(direction.Y > 0 && ((_orientation & GGConstants.ByteCodes.ThinWallBits.South) == GGConstants.ByteCodes.ThinWallBits.South))
            {
                return false;
            }
            if (direction.X > 0 && ((_orientation & GGConstants.ByteCodes.ThinWallBits.East) == GGConstants.ByteCodes.ThinWallBits.East))
            {
                return false;
            }
            if (direction.X < 0 && ((_orientation & GGConstants.ByteCodes.ThinWallBits.West) == GGConstants.ByteCodes.ThinWallBits.West))
            {
                return false;
            }
            return true;
        }


        public bool AllowEntry(Vector2I direction)
        {
            if(direction == Vector2I.Zero)
            {
                return true;
            }

            if (direction.Y < 0 && ((_orientation & GGConstants.ByteCodes.ThinWallBits.South) == GGConstants.ByteCodes.ThinWallBits.South))
            {
                return false;
            }
            if (direction.Y > 0 && ((_orientation & GGConstants.ByteCodes.ThinWallBits.North) == GGConstants.ByteCodes.ThinWallBits.North))
            {
                return false;
            }
            if (direction.X > 0 && ((_orientation & GGConstants.ByteCodes.ThinWallBits.West) == GGConstants.ByteCodes.ThinWallBits.West))
            {
                return false;
            }
            if (direction.X < 0 && ((_orientation & GGConstants.ByteCodes.ThinWallBits.East) == GGConstants.ByteCodes.ThinWallBits.East))
            {
                return false;
            }

            return true;
        }

        private byte _orientation = orientation;

        public void SetOrientation(byte orientation)
        {
            _orientation = orientation;
        }
    }
}
