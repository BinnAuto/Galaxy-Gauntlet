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
					EntityOrientation.North => Constants.SpriteCoordinates.Player_N,
					EntityOrientation.East => Constants.SpriteCoordinates.Player_E,
					EntityOrientation.South => Constants.SpriteCoordinates.Player_S,
					EntityOrientation.West => Constants.SpriteCoordinates.Player_W,
					_ => throw new NotImplementedException()
				};
			}
		}

		public override bool CanWalkOnGravel => true;


		public override void ProcessTick(DateTime timestamp)
		{
			_lastProcessTime = timestamp;
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

			Vector2I newCoordinate = Coordinate;

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
				GameData.PlayerItems.Add(currentItem);
			}

			if (currentItem is ChipItem chip1)
			{
				GameData.OnChipCollected(chip1);
			}

			#endregion

			var currentTile = GameData.GetMapTile(Coordinate);

			#region Force Floor 

			if (currentTile is MapForceFloorTile forceFloor && false == GameData.PlayerHasGameItem(Constants.ByteCodes.Entities.SuctionBoots))
			{
				GameData.AddToSlipList(this);
				var forceFloorInfluence = forceFloor.GetInfluence();
				var aggregateInput = forceFloorInfluence + input;
				aggregateInput = new(
					Mathf.Max(Mathf.Min(aggregateInput.X, 1), -1)
					, Mathf.Max(Mathf.Min(aggregateInput.Y, 1), -1)
				);
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
				Orientation = iceTile.SetOrientation(Orientation);
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
				GameData.PlayerItems.Add(item);
			}

			if(item is ChipItem chip2)
			{
				GameData.OnChipCollected(chip2);
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
