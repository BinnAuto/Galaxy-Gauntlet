using GalaxyGauntlet.scripts.MapEntities.Shared;
using System;

namespace GalaxyGauntlet.scripts.MapEntities
{
	public class Player(Vector2I coordinate) : MapMob(coordinate)
	{
		public override int DataCode => GGConstants.ByteCodes.Entities.Player;

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
						GGConstants.Archipelago.PlayerSpriteIndexes.Player => GGConstants.SpriteCoordinates.Player_N,
						GGConstants.Archipelago.PlayerSpriteIndexes.Teeth => GGConstants.SpriteCoordinates.Teeth_N,
						GGConstants.Archipelago.PlayerSpriteIndexes.BlueTank => GGConstants.SpriteCoordinates.BlueTank_N,
						GGConstants.Archipelago.PlayerSpriteIndexes.Glider => GGConstants.SpriteCoordinates.Glider_N,
						GGConstants.Archipelago.PlayerSpriteIndexes.Bug => GGConstants.SpriteCoordinates.Bug_N,
						GGConstants.Archipelago.PlayerSpriteIndexes.Paramecium => GGConstants.SpriteCoordinates.Paramecium_N,
						GGConstants.Archipelago.PlayerSpriteIndexes.Fireball => GGConstants.SpriteCoordinates.Fireball_N,
                        GGConstants.Archipelago.PlayerSpriteIndexes.Walker => GGConstants.SpriteCoordinates.Walker_N,
                        _ => GGConstants.SpriteCoordinates.Player_N
					},
					EntityOrientation.East => GameData.Archipelago.PlayerSpriteIndex switch
					{
						GGConstants.Archipelago.PlayerSpriteIndexes.Player => GGConstants.SpriteCoordinates.Player_E,
						GGConstants.Archipelago.PlayerSpriteIndexes.Teeth => GGConstants.SpriteCoordinates.Teeth_E,
						GGConstants.Archipelago.PlayerSpriteIndexes.BlueTank => GGConstants.SpriteCoordinates.BlueTank_E,
						GGConstants.Archipelago.PlayerSpriteIndexes.Glider => GGConstants.SpriteCoordinates.Glider_E,
						GGConstants.Archipelago.PlayerSpriteIndexes.Bug => GGConstants.SpriteCoordinates.Bug_E,
						GGConstants.Archipelago.PlayerSpriteIndexes.Paramecium => GGConstants.SpriteCoordinates.Paramecium_E,
						GGConstants.Archipelago.PlayerSpriteIndexes.Fireball => GGConstants.SpriteCoordinates.Fireball_E,
                        GGConstants.Archipelago.PlayerSpriteIndexes.Walker => GGConstants.SpriteCoordinates.Walker_E,
                        _ => GGConstants.SpriteCoordinates.Player_E
					},
					EntityOrientation.South => GameData.Archipelago.PlayerSpriteIndex switch
					{
						GGConstants.Archipelago.PlayerSpriteIndexes.Player => GGConstants.SpriteCoordinates.Player_S,
						GGConstants.Archipelago.PlayerSpriteIndexes.Teeth => GGConstants.SpriteCoordinates.Teeth_S,
						GGConstants.Archipelago.PlayerSpriteIndexes.BlueTank => GGConstants.SpriteCoordinates.BlueTank_S,
						GGConstants.Archipelago.PlayerSpriteIndexes.Glider => GGConstants.SpriteCoordinates.Glider_S,
						GGConstants.Archipelago.PlayerSpriteIndexes.Bug => GGConstants.SpriteCoordinates.Bug_S,
						GGConstants.Archipelago.PlayerSpriteIndexes.Paramecium => GGConstants.SpriteCoordinates.Paramecium_S,
						GGConstants.Archipelago.PlayerSpriteIndexes.Fireball => GGConstants.SpriteCoordinates.Fireball_S,
						GGConstants.Archipelago.PlayerSpriteIndexes.Walker => GGConstants.SpriteCoordinates.Walker_S,
						_ => GGConstants.SpriteCoordinates.Player_S
					},
					EntityOrientation.West => GameData.Archipelago.PlayerSpriteIndex switch
					{
						GGConstants.Archipelago.PlayerSpriteIndexes.Player => GGConstants.SpriteCoordinates.Player_W,
						GGConstants.Archipelago.PlayerSpriteIndexes.Teeth => GGConstants.SpriteCoordinates.Teeth_W,
						GGConstants.Archipelago.PlayerSpriteIndexes.BlueTank => GGConstants.SpriteCoordinates.BlueTank_W,
						GGConstants.Archipelago.PlayerSpriteIndexes.Glider => GGConstants.SpriteCoordinates.Glider_W,
						GGConstants.Archipelago.PlayerSpriteIndexes.Bug => GGConstants.SpriteCoordinates.Bug_W,
						GGConstants.Archipelago.PlayerSpriteIndexes.Paramecium => GGConstants.SpriteCoordinates.Paramecium_W,
						GGConstants.Archipelago.PlayerSpriteIndexes.Fireball => GGConstants.SpriteCoordinates.Fireball_W,
                        GGConstants.Archipelago.PlayerSpriteIndexes.Walker => GGConstants.SpriteCoordinates.Walker_W,
                        _ => GGConstants.SpriteCoordinates.Player_W
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

			if (currentTile is MapForceFloorTile forceFloor && false == GameData.Level.PlayerHasGameItem(GGConstants.ByteCodes.Entities.SuctionBoots))
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

			if(currentTile is MapIceTile iceTile && false == GameData.Level.PlayerHasGameItem(GGConstants.ByteCodes.Entities.IceSkates))
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
			if((currentTile is MapIceTile && false == GameData.Level.PlayerHasGameItem(GGConstants.ByteCodes.Entities.IceSkates))
				|| (currentTile is MapForceFloorTile && false == GameData.Level.PlayerHasGameItem(GGConstants.ByteCodes.Entities.SuctionBoots)))
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
