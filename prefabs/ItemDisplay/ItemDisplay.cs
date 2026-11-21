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

	private AnimatedSprite2D RedKey;

	private Label RedKeyCount;

	private AnimatedSprite2D BlueKey;

	private Label BlueKeyCount;

	private AnimatedSprite2D YellowKey;

	private Label YellowKeyCount;

	private AnimatedSprite2D GreenKey;

	private Label GreenKeyCount;

	private AnimatedSprite2D IceSkates;

	private AnimatedSprite2D SuctionBoots;

	private AnimatedSprite2D FireBoots;

	private AnimatedSprite2D Flippers;

	private AnimatedSprite2D SecretEye;

	private AnimatedSprite2D Helmet;

	private Godot.Color UnlockedModulateColor = Godot.Color.Color8(255, 255, 255, 255);

	private Godot.Color LockedModulateColor = Godot.Color.Color8(255, 255, 255, 60);


    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
	{
		RedKey = (AnimatedSprite2D)GetNode(nameof(RedKey));
		RedKey.SpriteFrames = GameData.SpriteFrames;
		RedKey.Animation = "default";
		RedKey.TextureFilter = CanvasItem.TextureFilterEnum.Nearest;
		RedKey.Frame = GGConstants.SpriteFrameIndexes.RedKey;
		RedKeyCount = (Label)RedKey.GetNode("Count");

		BlueKey = (AnimatedSprite2D)GetNode(nameof(BlueKey));
        BlueKey.SpriteFrames = GameData.SpriteFrames;
        BlueKey.Animation = "default";
        BlueKey.TextureFilter = CanvasItem.TextureFilterEnum.Nearest;
        BlueKey.Frame = GGConstants.SpriteFrameIndexes.BlueKey;
        BlueKeyCount = (Label)BlueKey.GetNode("Count");

		YellowKey = (AnimatedSprite2D)GetNode(nameof(YellowKey));
        YellowKey.SpriteFrames = GameData.SpriteFrames;
        YellowKey.Animation = "default";
        YellowKey.TextureFilter = CanvasItem.TextureFilterEnum.Nearest;
        YellowKey.Frame = GGConstants.SpriteFrameIndexes.YellowKey;
        YellowKeyCount = (Label)YellowKey.GetNode("Count");

		GreenKey = (AnimatedSprite2D)GetNode(nameof(GreenKey));
        GreenKey.SpriteFrames = GameData.SpriteFrames;
        GreenKey.Animation = "default";
        GreenKey.TextureFilter = CanvasItem.TextureFilterEnum.Nearest;
        GreenKey.Frame = GGConstants.SpriteFrameIndexes.GreenKey;
        GreenKeyCount = (Label)GreenKey.GetNode("Count");

		IceSkates = (AnimatedSprite2D)GetNode(nameof(IceSkates));
        IceSkates.SpriteFrames = GameData.SpriteFrames;
        IceSkates.Animation = "default";
        IceSkates.TextureFilter = CanvasItem.TextureFilterEnum.Nearest;
        IceSkates.Frame = GGConstants.SpriteFrameIndexes.IceSkates;

        SuctionBoots = (AnimatedSprite2D)GetNode(nameof(SuctionBoots));
        SuctionBoots.SpriteFrames = GameData.SpriteFrames;
        SuctionBoots.Animation = "default";
        SuctionBoots.TextureFilter = CanvasItem.TextureFilterEnum.Nearest;
        SuctionBoots.Frame = GGConstants.SpriteFrameIndexes.SuctionBoots;

        FireBoots = (AnimatedSprite2D)GetNode(nameof(FireBoots));
        FireBoots.SpriteFrames = GameData.SpriteFrames;
        FireBoots.Animation = "default";
        FireBoots.TextureFilter = CanvasItem.TextureFilterEnum.Nearest;
        FireBoots.Frame = GGConstants.SpriteFrameIndexes.FireBoots;

        Flippers = (AnimatedSprite2D)GetNode(nameof(Flippers));
        Flippers.SpriteFrames = GameData.SpriteFrames;
        Flippers.Animation = "default";
        Flippers.TextureFilter = CanvasItem.TextureFilterEnum.Nearest;
        Flippers.Frame = GGConstants.SpriteFrameIndexes.Flippers;

        SecretEye = (AnimatedSprite2D)GetNode(nameof(SecretEye));
        SecretEye.SpriteFrames = GameData.SpriteFrames;
        SecretEye.Animation = "default";
        SecretEye.TextureFilter = CanvasItem.TextureFilterEnum.Nearest;
        SecretEye.Frame = GGConstants.SpriteFrameIndexes.SecretEye;

        Helmet = (AnimatedSprite2D)GetNode(nameof(Helmet));
        Helmet.SpriteFrames = GameData.SpriteFrames;
        Helmet.Animation = "default";
        Helmet.TextureFilter = CanvasItem.TextureFilterEnum.Nearest;
        Helmet.Frame = GGConstants.SpriteFrameIndexes.Helmet;

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
