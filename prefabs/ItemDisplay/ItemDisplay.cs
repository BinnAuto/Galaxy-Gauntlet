using GalaxyGauntlet.scripts;

public enum DisplayMode
{
	Gameplay,
	ArchipelagoSingleUseItems,
	ArchipelagoTracker
}

public partial class ItemDisplay : Node
{
	[Export]
	private DisplayMode DisplayMode;

	private Sprite2D RedKey;

	private Label RedKeyCount;

	private Sprite2D BlueKey;

	private Label BlueKeyCount;

	private Sprite2D YellowKey;

	private Label YellowKeyCount;

	private Sprite2D GreenKey;

	private Label GreenKeyCount;

	private Sprite2D IceSkates;

	private Sprite2D SuctionBoots;

	private Sprite2D FireBoots;

	private Sprite2D Flippers;

	private Sprite2D SecretEye;

	private Sprite2D Helmet;

	private Godot.Color UnlockedModulateColor = Godot.Color.Color8(255, 255, 255, 255);

	private Godot.Color LockedModulateColor = Godot.Color.Color8(255, 255, 255, 60);


    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
	{
		RedKey = (Sprite2D)GetNode(nameof(RedKey));
		RedKeyCount = (Label)RedKey.GetNode("Count");

		BlueKey = (Sprite2D)GetNode(nameof(BlueKey));
		BlueKeyCount = (Label)BlueKey.GetNode("Count");

		YellowKey = (Sprite2D)GetNode(nameof(YellowKey));
		YellowKeyCount = (Label)YellowKey.GetNode("Count");

		GreenKey = (Sprite2D)GetNode(nameof(GreenKey));
		GreenKeyCount = (Label)GreenKey.GetNode("Count");

		IceSkates = (Sprite2D)GetNode(nameof(IceSkates));

		SuctionBoots = (Sprite2D)GetNode(nameof(SuctionBoots));

		FireBoots = (Sprite2D)GetNode(nameof(FireBoots));

		Flippers = (Sprite2D)GetNode(nameof(Flippers));

		SecretEye = (Sprite2D)GetNode(nameof(SecretEye));

		Helmet = (Sprite2D)GetNode(nameof(Helmet));
		
		RenderItemList();
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		RenderItemList();
	}


	private void RenderItemList()
	{
		switch(DisplayMode)
		{
			case DisplayMode.Gameplay:
				RedKey.Visible = GameData.Level.PlayerHasGameItem(GGConstants.ByteCodes.Entities.RedKey);
				BlueKey.Visible = GameData.Level.PlayerHasGameItem(GGConstants.ByteCodes.Entities.BlueKey);
				YellowKey.Visible = GameData.Level.PlayerHasGameItem(GGConstants.ByteCodes.Entities.YellowKey);
				GreenKey.Visible = GameData.Level.PlayerHasGameItem(GGConstants.ByteCodes.Entities.GreenKey);
				IceSkates.Visible = GameData.Level.PlayerHasGameItem(GGConstants.ByteCodes.Entities.IceSkates);
				SuctionBoots.Visible = GameData.Level.PlayerHasGameItem(GGConstants.ByteCodes.Entities.SuctionBoots);
				FireBoots.Visible = GameData.Level.PlayerHasGameItem(GGConstants.ByteCodes.Entities.FireBoots);
				Flippers.Visible = GameData.Level.PlayerHasGameItem(GGConstants.ByteCodes.Entities.Flippers);
				SecretEye.Visible = GameData.Level.PlayerHasGameItem(GGConstants.ByteCodes.Entities.SecretEye);
				Helmet.Visible = GameData.Level.PlayerHasGameItem(GGConstants.ByteCodes.Entities.Helmet);
				
				RedKeyCount.Text = GameData.Level.ItemCount(GGConstants.ByteCodes.Entities.RedKey).ToString();
				BlueKeyCount.Text = GameData.Level.ItemCount(GGConstants.ByteCodes.Entities.BlueKey).ToString();
				YellowKeyCount.Text = GameData.Level.ItemCount(GGConstants.ByteCodes.Entities.YellowKey).ToString();
				GreenKeyCount.Text = GameData.Level.ItemCount(GGConstants.ByteCodes.Entities.GreenKey).ToString();
				break;

			case DisplayMode.ArchipelagoSingleUseItems:
                RedKey.Visible = GameData.PlayerHasPendingItem(GGConstants.ByteCodes.Entities.RedKey);
                BlueKey.Visible = GameData.PlayerHasPendingItem(GGConstants.ByteCodes.Entities.BlueKey);
                YellowKey.Visible = GameData.PlayerHasPendingItem(GGConstants.ByteCodes.Entities.YellowKey);
                GreenKey.Visible = GameData.PlayerHasPendingItem(GGConstants.ByteCodes.Entities.GreenKey);
                IceSkates.Visible = GameData.PlayerHasPendingItem(GGConstants.ByteCodes.Entities.IceSkates);
                SuctionBoots.Visible = GameData.PlayerHasPendingItem(GGConstants.ByteCodes.Entities.SuctionBoots);
                FireBoots.Visible = GameData.PlayerHasPendingItem(GGConstants.ByteCodes.Entities.FireBoots);
                Flippers.Visible = GameData.PlayerHasPendingItem(GGConstants.ByteCodes.Entities.Flippers);
                SecretEye.Visible = GameData.PlayerHasPendingItem(GGConstants.ByteCodes.Entities.SecretEye);
                Helmet.Visible = GameData.PlayerHasPendingItem(GGConstants.ByteCodes.Entities.Helmet);
                
				RedKeyCount.Text = GameData.PendingItemCount(GGConstants.ByteCodes.Entities.RedKey).ToString();
                BlueKeyCount.Text = GameData.PendingItemCount(GGConstants.ByteCodes.Entities.BlueKey).ToString();
                YellowKeyCount.Text = GameData.PendingItemCount(GGConstants.ByteCodes.Entities.YellowKey).ToString();
                GreenKeyCount.Text = GameData.PendingItemCount(GGConstants.ByteCodes.Entities.GreenKey).ToString();
				break;

			case DisplayMode.ArchipelagoTracker:
				RedKey.Modulate = GameData.Archipelago.RedKeyUnlocked
					? UnlockedModulateColor : LockedModulateColor;
                BlueKey.Modulate = GameData.Archipelago.BlueKeyUnlocked
                    ? UnlockedModulateColor : LockedModulateColor;
                YellowKey.Modulate = GameData.Archipelago.YellowKeyUnlocked
                    ? UnlockedModulateColor : LockedModulateColor;
                GreenKey.Modulate = GameData.Archipelago.GreenKeyUnlocked
                    ? UnlockedModulateColor : LockedModulateColor;
                IceSkates.Modulate = GameData.Archipelago.IceSkatesUnlocked
                    ? UnlockedModulateColor : LockedModulateColor;
                SuctionBoots.Modulate = GameData.Archipelago.SuctionBootsUnlocked
                    ? UnlockedModulateColor : LockedModulateColor;
                FireBoots.Modulate = GameData.Archipelago.FireBootsUnlocked
                    ? UnlockedModulateColor : LockedModulateColor;
                Flippers.Modulate = GameData.Archipelago.FlippersUnlocked
                    ? UnlockedModulateColor : LockedModulateColor;
                SecretEye.Modulate = GameData.Archipelago.SecretEyeUnlocked
                    ? UnlockedModulateColor : LockedModulateColor;
                Helmet.Modulate = GameData.Archipelago.HelmetUnlocked
                    ? UnlockedModulateColor : LockedModulateColor;

                RedKeyCount.Text = string.Empty;
                BlueKeyCount.Text = string.Empty;
                YellowKeyCount.Text = string.Empty;
                GreenKeyCount.Text = string.Empty;
                break;
        }
	}
}
