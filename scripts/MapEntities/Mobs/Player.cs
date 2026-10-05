using GalaxyGauntlet.scripts.MapEntities.Shared;
using System;

namespace GalaxyGauntlet.scripts.MapEntities
{
	public class Player(Vector2I coordinate) : MapMob(coordinate)
	{
		public override int DataCode => Constants.ByteCodes.Entities.Player;

		public override string Name => "Player";

		public override bool DiesToFire => false;

		public override bool DiesToWater => false;

		public override Vector2I TextureCoordinate
		{
			get
			{
				return Orientation switch
				{
					EntityOrientation.North => GameData.PlayerSpriteIndex switch
					{
						Constants.Archipelago.PlayerSpriteIndexes.Player => Constants.SpriteCoordinates.Player_N,
						Constants.Archipelago.PlayerSpriteIndexes.Teeth => Constants.SpriteCoordinates.Teeth_N,
						Constants.Archipelago.PlayerSpriteIndexes.BlueTank => Constants.SpriteCoordinates.BlueTank_N,
						Constants.Archipelago.PlayerSpriteIndexes.Glider => Constants.SpriteCoordinates.Glider_N,
						Constants.Archipelago.PlayerSpriteIndexes.Bug => Constants.SpriteCoordinates.Bug_N,
						Constants.Archipelago.PlayerSpriteIndexes.Paramecium => Constants.SpriteCoordinates.Paramecium_N,
						Constants.Archipelago.PlayerSpriteIndexes.Fireball => Constants.SpriteCoordinates.Fireball_N,
						_ => Constants.SpriteCoordinates.Player_N
					},
					EntityOrientation.East => GameData.PlayerSpriteIndex switch
					{
						Constants.Archipelago.PlayerSpriteIndexes.Player => Constants.SpriteCoordinates.Player_E,
						Constants.Archipelago.PlayerSpriteIndexes.Teeth => Constants.SpriteCoordinates.Teeth_E,
						Constants.Archipelago.PlayerSpriteIndexes.BlueTank => Constants.SpriteCoordinates.BlueTank_E,
						Constants.Archipelago.PlayerSpriteIndexes.Glider => Constants.SpriteCoordinates.Glider_E,
						Constants.Archipelago.PlayerSpriteIndexes.Bug => Constants.SpriteCoordinates.Bug_E,
						Constants.Archipelago.PlayerSpriteIndexes.Paramecium => Constants.SpriteCoordinates.Paramecium_E,
						Constants.Archipelago.PlayerSpriteIndexes.Fireball => Constants.SpriteCoordinates.Fireball_E,
						_ => Constants.SpriteCoordinates.Player_E
					},
					EntityOrientation.South => GameData.PlayerSpriteIndex switch
					{
						Constants.Archipelago.PlayerSpriteIndexes.Player => Constants.SpriteCoordinates.Player_S,
						Constants.Archipelago.PlayerSpriteIndexes.Teeth => Constants.SpriteCoordinates.Teeth_S,
						Constants.Archipelago.PlayerSpriteIndexes.BlueTank => Constants.SpriteCoordinates.BlueTank_S,
						Constants.Archipelago.PlayerSpriteIndexes.Glider => Constants.SpriteCoordinates.Glider_S,
						Constants.Archipelago.PlayerSpriteIndexes.Bug => Constants.SpriteCoordinates.Bug_S,
						Constants.Archipelago.PlayerSpriteIndexes.Paramecium => Constants.SpriteCoordinates.Paramecium_S,
						Constants.Archipelago.PlayerSpriteIndexes.Fireball => Constants.SpriteCoordinates.Fireball_S,
						_ => Constants.SpriteCoordinates.Player_S
					},
					EntityOrientation.West => GameData.PlayerSpriteIndex switch
					{
						Constants.Archipelago.PlayerSpriteIndexes.Player => Constants.SpriteCoordinates.Player_W,
						Constants.Archipelago.PlayerSpriteIndexes.Teeth => Constants.SpriteCoordinates.Teeth_W,
						Constants.Archipelago.PlayerSpriteIndexes.BlueTank => Constants.SpriteCoordinates.BlueTank_W,
						Constants.Archipelago.PlayerSpriteIndexes.Glider => Constants.SpriteCoordinates.Glider_W,
						Constants.Archipelago.PlayerSpriteIndexes.Bug => Constants.SpriteCoordinates.Bug_W,
						Constants.Archipelago.PlayerSpriteIndexes.Paramecium => Constants.SpriteCoordinates.Paramecium_W,
						Constants.Archipelago.PlayerSpriteIndexes.Fireball => Constants.SpriteCoordinates.Fireball_W,
						_ => Constants.SpriteCoordinates.Player_W
					},
					_ => throw new NotImplementedException()
				};
			}
		}

		public override bool CanWalkOnGravel => true;


		public override void ProcessTick()
		{
			if(false == GameData.AcceptingPlayerInput)
			{
				return;
			}

			Vector2I input = Vector2I.Zero;
			if (Input.IsActionPressed("player_up"))
			{
				input = new(0, -1);
			}
			if (Input.IsActionPressed("player_down"))
			{
				input = new(0, 1);
			}
			if (Input.IsActionPressed("player_left"))
			{
				input = new(-1, 0);
			}
			if (Input.IsActionPressed("player_right"))
			{
				input = new(1, 0);
			}

			#region Process Item collection

			var currentItem = GameData.GetMapItem(Coordinate);
			if ((currentItem is RedKeyItem && GameData.RedKeyUnlocked)
				|| (currentItem is BlueKeyItem && GameData.BlueKeyUnlocked)
				|| (currentItem is YellowKeyItem && GameData.YellowKeyUnlocked)
				|| (currentItem is GreenKeyItem && GameData.GreenKeyUnlocked)
				|| (currentItem is IceSkatesItem && GameData.IceSkatesUnlocked)
				|| (currentItem is FireBootsItem && GameData.FireBootsUnlocked)
				|| (currentItem is FlippersItem && GameData.FlippersUnlocked)
				|| (currentItem is SuctionBootsItem && GameData.SuctionBootsUnlocked)
			)
			{
				GameData.RemoveMapItem(Coordinate);
				GameData.AddItem(currentItem);
			}

			if (currentItem is ChipItem chip1)
			{
				GameData.OnChipCollected(chip1);
			}

			#endregion

			Vector2I newCoordinate = Coordinate;
			var currentTile = GameData.GetMapTile(Coordinate);

			#region Force Floor 

			if (currentTile is MapForceFloorTile forceFloor && false == GameData.PlayerHasGameItem(Constants.ByteCodes.Entities.SuctionBoots))
			{
				GameData.AddToSlipList(this);
				var forceFloorInfluence = forceFloor.GetInfluence();
				var aggregateInput = forceFloorInfluence + input;
				newCoordinate = ProposeMove(aggregateInput);
				if(newCoordinate == Coordinate)
				{
					newCoordinate = ProposeMove(forceFloorInfluence);
					if(newCoordinate == Coordinate)
					{
						newCoordinate = ProposeMove(input);
					}
				}
				SetOrientationAndCoordinate(aggregateInput, newCoordinate);
				return;
			}

			#endregion

			#region Ice

			if(currentTile is MapIceTile iceTile && false == GameData.PlayerHasGameItem(Constants.ByteCodes.Entities.IceSkates))
			{
				GameData.AddToSlipList(this);
				Orientation = iceTile.SetEntityOrientation(Orientation);
				newCoordinate = ProposeMove(Forward);
				if(newCoordinate == Coordinate)
				{
					ReverseOrientation();
					newCoordinate = ProposeMove(Forward);
				}
				SetCoordinate(newCoordinate);
				return;
			}

			#endregion

			GameData.RemoveFromSlipList(this);
			newCoordinate = ProposeMove(input);
			if(newCoordinate == Coordinate)
			{
				return;
			}

			SetOrientationAndCoordinate(input, newCoordinate);

			var item = GameData.GetMapItem(Coordinate);
			if((item is RedKeyItem && GameData.RedKeyUnlocked)
				|| (item is BlueKeyItem && GameData.BlueKeyUnlocked)
				|| (item is YellowKeyItem && GameData.YellowKeyUnlocked)
				|| (item is GreenKeyItem && GameData.GreenKeyUnlocked)
				|| (item is IceSkatesItem && GameData.IceSkatesUnlocked)
				|| (item is FireBootsItem && GameData.FireBootsUnlocked)
				|| (item is FlippersItem && GameData.FlippersUnlocked)
				|| (item is SuctionBootsItem && GameData.SuctionBootsUnlocked)
			)
			{
				GameData.RemoveMapItem(Coordinate);
				GameData.AddItem(item);
			}

			if(item is ChipItem chip2)
			{
				GameData.OnChipCollected(chip2);
			}

			currentTile = GameData.GetMapTile(Coordinate);
			if((currentTile is MapIceTile && false == GameData.PlayerHasGameItem(Constants.ByteCodes.Entities.IceSkates))
				|| (currentTile is MapForceFloorTile && false == GameData.PlayerHasGameItem(Constants.ByteCodes.Entities.SuctionBoots)))
			{
				GameData.AddToSlipList(this);
			}
		}


		public override void SetCoordinate(Vector2I newCoordinate)
		{
			if(Coordinate == newCoordinate)
			{
				return;
			}

			GameData.RemoveMapPlayer(Coordinate);
			Coordinate = newCoordinate;
			GameData.MapPlayers[newCoordinate.X, newCoordinate.Y] = this;
			GameData.PlayerCoordinate = newCoordinate;
			GameData.RerenderMap = true;
		}
	}
}
