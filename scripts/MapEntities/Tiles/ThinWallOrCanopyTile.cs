using GalaxyGauntlet.scripts.MapEntities.Shared;

namespace GalaxyGauntlet.scripts.MapEntities
{
    public class ThinWallOrCanopyTile(Vector2I coordinate, byte orientation) : MapTile(coordinate)
    {
        public override int DataCode => Constants.ByteCodes.Entities.ThinWallOrCanopy;

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
                if((_orientation & Constants.ByteCodes.ThinWallBits.North) == Constants.ByteCodes.ThinWallBits.North)
                {
                    orientations += "N";
                }
                if ((_orientation & Constants.ByteCodes.ThinWallBits.East) == Constants.ByteCodes.ThinWallBits.East)
                {
                    orientations += "E";
                }
                if ((_orientation & Constants.ByteCodes.ThinWallBits.South) == Constants.ByteCodes.ThinWallBits.South)
                {
                    orientations += "S";
                }
                if ((_orientation & Constants.ByteCodes.ThinWallBits.West) == Constants.ByteCodes.ThinWallBits.West)
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
                    0b00000 or 0b10000 => Constants.SpriteCoordinates.ThinWall_None,
                    0b00001 => Constants.SpriteCoordinates.ThinWall_N,
                    0b00010 => Constants.SpriteCoordinates.ThinWall_E,
                    0b00100 => Constants.SpriteCoordinates.ThinWall_S,
                    0b01000 => Constants.SpriteCoordinates.ThinWall_W,
                    0b00011 => Constants.SpriteCoordinates.ThinWall_NE,
                    0b00101 => Constants.SpriteCoordinates.ThinWall_NS,
                    0b00110 => Constants.SpriteCoordinates.ThinWall_SE,
                    0b00111 => Constants.SpriteCoordinates.ThinWall_NES,
                    0b01001 => Constants.SpriteCoordinates.ThinWall_NW,
                    0b01010 => Constants.SpriteCoordinates.ThinWall_EW,
                    0b01011 => Constants.SpriteCoordinates.ThinWall_NEW,
                    0b01100 => Constants.SpriteCoordinates.ThinWall_SW,
                    0b01101 => Constants.SpriteCoordinates.ThinWall_NSW,
                    0b01110 => Constants.SpriteCoordinates.ThinWall_ESW,
                    0b01111 => Constants.SpriteCoordinates.ThinWall_NESW,
                    _ => Constants.SpriteCoordinates.ThinWall_None
                };
            }
        }


        public bool AllowExit(Vector2I direction)
        {
            if(direction == Vector2I.Zero)
            {
                return true;
            }

            if (direction.Y < 0 && ((_orientation & Constants.ByteCodes.ThinWallBits.North) == Constants.ByteCodes.ThinWallBits.North))
            {
                return false;
            }
            if(direction.Y > 0 && ((_orientation & Constants.ByteCodes.ThinWallBits.South) == Constants.ByteCodes.ThinWallBits.South))
            {
                return false;
            }
            if (direction.X > 0 && ((_orientation & Constants.ByteCodes.ThinWallBits.East) == Constants.ByteCodes.ThinWallBits.East))
            {
                return false;
            }
            if (direction.X < 0 && ((_orientation & Constants.ByteCodes.ThinWallBits.West) == Constants.ByteCodes.ThinWallBits.West))
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

            if (direction.Y < 0 && ((_orientation & Constants.ByteCodes.ThinWallBits.South) == Constants.ByteCodes.ThinWallBits.South))
            {
                return false;
            }
            if (direction.Y > 0 && ((_orientation & Constants.ByteCodes.ThinWallBits.North) == Constants.ByteCodes.ThinWallBits.North))
            {
                return false;
            }
            if (direction.X > 0 && ((_orientation & Constants.ByteCodes.ThinWallBits.West) == Constants.ByteCodes.ThinWallBits.West))
            {
                return false;
            }
            if (direction.X < 0 && ((_orientation & Constants.ByteCodes.ThinWallBits.East) == Constants.ByteCodes.ThinWallBits.East))
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
