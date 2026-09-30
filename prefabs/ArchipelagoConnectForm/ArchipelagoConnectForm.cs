using GalaxyGauntlet.scripts;
using System;
using System.Threading.Tasks;

public partial class ArchipelagoConnectForm : Node2D
{
	#region Child Nodes

	LineEdit GameName;

	LineEdit Server;

	LineEdit Port;

	LineEdit SlotName;

	LineEdit Password;

	UIButton ConnectButton;

	UIButton CancelButton;

	Label ErrorText;

	#endregion


	#region Delegates and Events

	public delegate void OnConnectedHandler();

	public delegate void OnCancelledHandler();

	public event OnConnectedHandler Connected = () => { };

	public event OnCancelledHandler Cancelled = () => { };

	#endregion

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		try
		{
			GameName = (LineEdit)GetNode(nameof(GameName) + "/" + nameof(GameName));
			Server = (LineEdit)GetNode(nameof(Server) + "/" + nameof(Server));
			Port = (LineEdit)GetNode(nameof(Port) + "/" + nameof(Port));
			SlotName = (LineEdit)GetNode(nameof(SlotName) + "/" + nameof(SlotName));
			Password = (LineEdit)GetNode(nameof(Password) + "/" + nameof(Password));
		
			ConnectButton = (UIButton)GetNode(nameof(ConnectButton));
			ConnectButton.Pressed += async () => { await OnConnect(); };

			CancelButton = (UIButton)GetNode(nameof(CancelButton));
			CancelButton.Pressed += OnCancel;

			ErrorText = (Label)GetNode(nameof(ErrorText));
		}
		catch(Exception e)
		{
			FileLogger.LogException("Error readying Archipelago connect form", e);
		}
	}


	#region Event Handlers

	private async Task OnConnect()
	{
		try
		{
			ErrorText.Text = string.Empty;
			ValidateForm();
			string gameName = GameName.Text;
			string server = Server.Text;
			int port = GetPortNumber();
			string slotName = SlotName.Text;
			string password = Password.Text;
			await GameData.ConnectToArchipelago(gameName, server, port, slotName, password);
			Connected.Invoke();
		}
		catch(Exception e)
		{
			ErrorText.Text = e.Message;
		}
	}


	private void OnCancel()
	{
		Cancelled.Invoke();
	}

	#endregion


	private void ValidateForm()
	{
		if (string.IsNullOrEmpty(GameName.Text))
		{
			throw new("Game Name is missing");
		}
		if (string.IsNullOrEmpty(Server.Text))
		{
			throw new("Server is missing");
		}
		if (string.IsNullOrEmpty(SlotName.Text))
		{
			throw new("Slot Name is missing");
		}
	}


	private int GetPortNumber()
	{
		string server = Server.Text;
		int port;
		if(server.Contains(':'))
		{
			// Parse port number from server
			string portSection = server.Split(":")[1];
			if(false == int.TryParse(portSection, out port))
			{
				throw new("Invalid port number");
			}
		}
		else
		{
			if(string.IsNullOrEmpty(Port.Text))
			{
				throw new("Port is missing");
			}
			if(false == int.TryParse(Port.Text, out port))
			{
				throw new("Invalid port number");
			}
		}
		if (port < 1 || port > 65535)
		{
			throw new("Invalid port number");
		}
		return port;
	}
}
