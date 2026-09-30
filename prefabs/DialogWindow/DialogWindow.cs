public enum DialogButtonType
{
	YesNo,
	OK
}

public partial class DialogWindow : Node2D
{
	#region Child Nodes

	private Button InputBlocker;

	private Label Content;

	private Label Title;

	private Node2D YesNoButtonContainer;

	private UIButton YesButton;

	private UIButton NoButton;

	private Node2D OKButtonContainer;

	private UIButton OKButton;

	#endregion


	#region Delegates and Events

	public delegate void OnButtonPressedHandler();

	public event OnButtonPressedHandler OKButtonPressed = () => { };

	public event OnButtonPressedHandler YesButtonPressed = () => { };

	public event OnButtonPressedHandler NoButtonPressed = () => { };

	#endregion


	#region Godot Overrides

	public override void _Ready()
	{
		try
		{
			Visible = false;
			InputBlocker = (Button)GetNode(nameof(InputBlocker));

			Content = (Label)GetNode(nameof(Content));

			Title = (Label)GetNode(nameof(Title));
			Title.Text = Constants.GameName;

			YesNoButtonContainer = (Node2D)GetNode(nameof(YesNoButtonContainer));
			YesButton = (UIButton)YesNoButtonContainer.GetNode(nameof(YesButton));
			YesButton.Pressed += OnYesButtonPressed;
			NoButton = (UIButton)YesNoButtonContainer.GetNode(nameof(NoButton));
			NoButton.Pressed += OnNoButtonPressed;
			YesNoButtonContainer.Visible = false;

			OKButtonContainer = (Node2D)GetNode(nameof(OKButtonContainer));
			OKButton = (UIButton)OKButtonContainer.GetNode(nameof(OKButton));
			OKButton.Pressed += OnOKButtonPressed;
			OKButtonContainer.Visible = false;
		}
		catch(System.Exception e)
		{
			FileLogger.LogException("Error readying Dialog window", e);
		}
	}


	public override void _Process(double delta)
	{
		if(Visible && Input.IsActionJustPressed("player_ok") && OKButtonContainer.Visible)
		{
			OnOKButtonPressed();
		}
	}

	#endregion


	public void ShowDialog(string content, DialogButtonType dialogButtonType)
	{
		ShowDialog(content, Constants.GameName, dialogButtonType);
	}


	public void ShowDialog(string content, string title, DialogButtonType dialogButtonType)
	{
		Content.SetSize(
			new(Mathf.Max(135, Content.Size.X), Content.Size.Y)
		);
		Content.Text = content;
		if (false == string.IsNullOrEmpty(title))
		{
			Title.Text = title;
		}
		YesNoButtonContainer.Visible = (dialogButtonType == DialogButtonType.YesNo);
		OKButtonContainer.Visible = (dialogButtonType == DialogButtonType.OK);
		SetDialogPosition();
		Visible = true;
	}


	public void HideDialog()
	{
		Content.SetSize(Vector2.One);
		Visible = false;
	}


	public void ClearAllButtonEvents()
	{
		ClearOKButtonEvents();
		ClearYesButtonPressedEvents();
		ClearNoButtonPressedEvents();
	}


	public void ClearOKButtonEvents()
	{
		if (OKButtonPressed is null)
		{
			OKButtonPressed = () => { };
			return;
		}

		foreach (var @delegate in OKButtonPressed.GetInvocationList())
		{
			OKButtonPressed -= (OnButtonPressedHandler)@delegate;
		}
	}


	public void ClearYesButtonPressedEvents()
	{
		if (YesButtonPressed is null)
		{
			YesButtonPressed = () => { };
			return;
		}

		foreach (var @delegate in YesButtonPressed.GetInvocationList())
		{
			YesButtonPressed -= (OnButtonPressedHandler)@delegate;
		}
	}


	public void ClearNoButtonPressedEvents()
	{
		if (NoButtonPressed is null)
		{
			NoButtonPressed = () => { };
			return;
		}

		foreach (var @delegate in NoButtonPressed.GetInvocationList())
		{
			NoButtonPressed -= (OnButtonPressedHandler)@delegate;
		}
	}


	#region Event Handlers

	private void OnOKButtonPressed()
	{
		OKButtonPressed.Invoke();
	}


	private void OnYesButtonPressed()
	{
		YesButtonPressed.Invoke();
	}


	private void OnNoButtonPressed()
	{
		NoButtonPressed.Invoke();
	}

	#endregion


	#region Helper Methods

	private void SetDialogPosition()
	{
		var windowSize = GetWindow().Size;
		var contentSize = Content.GetLabelSize();
		YesNoButtonContainer.Position = new(
			(contentSize.X / 2)
			, contentSize.Y - 30
		);
		OKButtonContainer.Position = new(
			(contentSize.X / 2)
			, contentSize.Y - 30
		);
		Position = (windowSize - contentSize) / 2;

		InputBlocker.GlobalPosition = Vector2.Zero;
		InputBlocker.SetSize(windowSize);

		Title.Position = Vector2.Zero;
		Title.SetSize(new(contentSize.X, Title.Size.Y));
	}

	#endregion
}
