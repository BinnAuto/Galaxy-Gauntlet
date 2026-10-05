using GalaxyGauntlet.scripts;

public partial class ItemDisplay : Node
{
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
		
		RenderItemList();
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if(GameData.RerenderItemList)
		{
			RenderItemList();
		}
	}


	private void RenderItemList()
	{
		RedKey.Visible = GameData.PlayerHasGameItem(Constants.ByteCodes.Entities.RedKey);
		RedKeyCount.Text = GameData.ItemCount(Constants.ByteCodes.Entities.RedKey).ToString();

		BlueKey.Visible = GameData.PlayerHasGameItem(Constants.ByteCodes.Entities.BlueKey);
		BlueKeyCount.Text = GameData.ItemCount(Constants.ByteCodes.Entities.BlueKey).ToString();

		YellowKey.Visible = GameData.PlayerHasGameItem(Constants.ByteCodes.Entities.YellowKey);
		YellowKeyCount.Text = GameData.ItemCount(Constants.ByteCodes.Entities.YellowKey).ToString();

		GreenKey.Visible = GameData.PlayerHasGameItem(Constants.ByteCodes.Entities.GreenKey);
		GreenKeyCount.Text = GameData.ItemCount(Constants.ByteCodes.Entities.GreenKey).ToString();

		IceSkates.Visible = GameData.PlayerHasGameItem(Constants.ByteCodes.Entities.IceSkates);
		SuctionBoots.Visible = GameData.PlayerHasGameItem(Constants.ByteCodes.Entities.SuctionBoots);
		FireBoots.Visible = GameData.PlayerHasGameItem(Constants.ByteCodes.Entities.FireBoots);
		Flippers.Visible = GameData.PlayerHasGameItem(Constants.ByteCodes.Entities.Flippers);

		GameData.RerenderItemList = false;
	}
}
