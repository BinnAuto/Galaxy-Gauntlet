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
					EntityOrientation.North => GameData.Archipelago.PlayerSpriteIndex switch
					{
						Constants.Archipelago.PlayerSpriteIndexes.Player => Constants.SpriteCoordinates.Player_N,
						Constants.Archipelago.PlayerSpriteIndexes.Teeth => Constants.SpriteCoordinates.Teeth_N,
						Constants.Archipelago.PlayerSpriteIndexes.BlueTank => Constants.SpriteCoordinates.BlueTank_N,
						Constants.Archipelago.PlayerSpriteIndexes.Glider => Constants.SpriteCoordinates.Glider_N,
						Constants.Archipelago.PlayerSpriteIndexes.Bug => Constants.SpriteCoordinates.Bug_N,
						Constants.Archipelago.PlayerSpriteIndexes.Paramecium => Constants.SpriteCoordinates.Paramecium_N,
						Constants.Archipelago.PlayerSpriteIndexes.Fireball => Constants.SpriteCoordinates.Fireball_N,
                        Constants.Archipelago.PlayerSpriteIndexes.Walker => Constants.SpriteCoordinates.Walker_N,
                        _ => Constants.SpriteCoordinates.Player_N
					},
					EntityOrientation.East => GameData.Archipelago.PlayerSpriteIndex switch
					{
						Constants.Archipelago.PlayerSpriteIndexes.Player => Constants.SpriteCoordinates.Player_E,
						Constants.Archipelago.PlayerSpriteIndexes.Teeth => Constants.SpriteCoordinates.Teeth_E,
						Constants.Archipelago.PlayerSpriteIndexes.BlueTank => Constants.SpriteCoordinates.BlueTank_E,
						Constants.Archipelago.PlayerSpriteIndexes.Glider => Constants.SpriteCoordinates.Glider_E,
						Constants.Archipelago.PlayerSpriteIndexes.Bug => Constants.SpriteCoordinates.Bug_E,
						Constants.Archipelago.PlayerSpriteIndexes.Paramecium => Constants.SpriteCoordinates.Paramecium_E,
						Constants.Archipelago.PlayerSpriteIndexes.Fireball => Constants.SpriteCoordinates.Fireball_E,
                        Constants.Archipelago.PlayerSpriteIndexes.Walker => Constants.SpriteCoordinates.Walker_E,
                        _ => Constants.SpriteCoordinates.Player_E
					},
					EntityOrientation.South => GameData.Archipelago.PlayerSpriteIndex switch
					{
						Constants.Archipelago.PlayerSpriteIndexes.Player => Constants.SpriteCoordinates.Player_S,
						Constants.Archipelago.PlayerSpriteIndexes.Teeth => Constants.SpriteCoordinates.Teeth_S,
						Constants.Archipelago.PlayerSpriteIndexes.BlueTank => Constants.SpriteCoordinates.BlueTank_S,
						Constants.Archipelago.PlayerSpriteIndexes.Glider => Constants.SpriteCoordinates.Glider_S,
						Constants.Archipelago.PlayerSpriteIndexes.Bug => Constants.SpriteCoordinates.Bug_S,
						Constants.Archipelago.PlayerSpriteIndexes.Paramecium => Constants.SpriteCoordinates.Paramecium_S,
						Constants.Archipelago.PlayerSpriteIndexes.Fireball => Constants.SpriteCoordinates.Fireball_S,
						Constants.Archipelago.PlayerSpriteIndexes.Walker => Constants.SpriteCoordinates.Walker_S,
						_ => Constants.SpriteCoordinates.Player_S
					},
					EntityOrientation.West => GameData.Archipelago.PlayerSpriteIndex switch
					{
						Constants.Archipelago.PlayerSpriteIndexes.Player => Constants.SpriteCoordinates.Player_W,
						Constants.Archipelago.PlayerSpriteIndexes.Teeth => Constants.SpriteCoordinates.Teeth_W,
						Constants.Archipelago.PlayerSpriteIndexes.BlueTank => Constants.SpriteCoordinates.BlueTank_W,
						Constants.Archipelago.PlayerSpriteIndexes.Glider => Constants.SpriteCoordinates.Glider_W,
						Constants.Archipelago.PlayerSpriteIndexes.Bug => Constants.SpriteCoordinates.Bug_W,
						Constants.Archipelago.PlayerSpriteIndexes.Paramecium => Constants.SpriteCoordinates.Paramecium_W,
						Constants.Archipelago.PlayerSpriteIndexes.Fireball => Constants.SpriteCoordinates.Fireball_W,
                        Constants.Archipelago.PlayerSpriteIndexes.Walker => Constants.SpriteCoordinates.Walker_W,
                        _ => Constants.SpriteCoordinates.Player_W
					},
					_ => throw new NotImplementedException()
				};
			}
		}

		public override bool CanWalkOnGravel => true;


		public override void ProcessTick()
		{
			if(false == GameData.Level.AcceptingPlayerInput)
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
			if (GameData.Level.IsConfused)
			{
				input *= -1;
			}

			#region Process Item collection

			var currentItem = GameData.Map.GetItem(Coordinate);
			if ((currentItem is RedKeyItem && GameData.Archipelago.RedKeyUnlocked)
				|| (currentItem is BlueKeyItem && GameData.Archipelago.BlueKeyUnlocked)
				|| (currentItem is YellowKeyItem && GameData.Archipelago.YellowKeyUnlocked)
				|| (currentItem is GreenKeyItem && GameData.Archipelago.GreenKeyUnlocked)
				|| (currentItem is IceSkatesItem && GameData.Archipelago.IceSkatesUnlocked)
				|| (currentItem is FireBootsItem && GameData.Archipelago.FireBootsUnlocked)
				|| (currentItem is FlippersItem && GameData.Archipelago.FlippersUnlocked)
				|| (currentItem is SuctionBootsItem && GameData.Archipelago.SuctionBootsUnlocked)
			)
			{
				GameData.Map.RemoveItem(Coordinate);
				GameData.Level.AddItem(currentItem);
			}

			if (currentItem is ChipItem chip1)
			{
				GameData.OnChipCollected(chip1);
			}

			#endregion

			Vector2I newCoordinate = Coordinate;
			var currentTile = GameData.Map.GetTile(Coordinate);

			#region Force Floor 

			if (currentTile is MapForceFloorTile forceFloor && false == GameData.Level.PlayerHasGameItem(Constants.ByteCodes.Entities.SuctionBoots))
			{
				GameData.Map.AddToSlipList(this);
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

			if(currentTile is MapIceTile iceTile && false == GameData.Level.PlayerHasGameItem(Constants.ByteCodes.Entities.IceSkates))
			{
				GameData.Map.AddToSlipList(this);
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

			GameData.Map.RemoveFromSlipList(this);
			newCoordinate = ProposeMove(input);
			SetOrientation(input);
			if(newCoordinate == Coordinate)
			{
				return;
			}

			SetOrientationAndCoordinate(input, newCoordinate);

			var item = GameData.Map.GetItem(Coordinate);
			if((item is RedKeyItem && GameData.Archipelago.RedKeyUnlocked)
				|| (item is BlueKeyItem && GameData.Archipelago.BlueKeyUnlocked)
				|| (item is YellowKeyItem && GameData.Archipelago.YellowKeyUnlocked)
				|| (item is GreenKeyItem && GameData.Archipelago.GreenKeyUnlocked)
				|| (item is IceSkatesItem && GameData.Archipelago.IceSkatesUnlocked)
				|| (item is FireBootsItem && GameData.Archipelago.FireBootsUnlocked)
				|| (item is FlippersItem && GameData.Archipelago.FlippersUnlocked)
				|| (item is SuctionBootsItem && GameData.Archipelago.SuctionBootsUnlocked)
			)
			{
				GameData.Map.RemoveItem(Coordinate);
				GameData.Level.AddItem(item);
			}

			if(item is ChipItem chip2)
			{
				GameData.OnChipCollected(chip2);
			}

			currentTile = GameData.Map.GetTile(Coordinate);
			if((currentTile is MapIceTile && false == GameData.Level.PlayerHasGameItem(Constants.ByteCodes.Entities.IceSkates))
				|| (currentTile is MapForceFloorTile && false == GameData.Level.PlayerHasGameItem(Constants.ByteCodes.Entities.SuctionBoots)))
			{
				GameData.Map.AddToSlipList(this);
			}
		}


		public override void SetCoordinate(Vector2I newCoordinate)
		{
			if(Coordinate == newCoordinate)
			{
				return;
			}

			GameData.Map.RemovePlayer(Coordinate);
			Coordinate = newCoordinate;
			GameData.Map.Players[newCoordinate.X, newCoordinate.Y] = this;
			GameData.Level.PlayerCoordinate = newCoordinate;
			GameData.Map.Rerender = true;
		}
	}
}
