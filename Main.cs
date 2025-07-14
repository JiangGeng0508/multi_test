using Godot;
using Godot.Collections;

public partial class Main : Control
{
	public ItemList MessageList;
	public ItemList PlayersList;
	public Dictionary<string, int> PlayerIndexList = [];
	public Dictionary<long, string> PlayerPeerList = [];
	public string NickName = "Nameless";
	public int Port = 13677;
	public string Address = "frp-end.com";
	public ENetMultiplayerPeer Peer;

	public override void _Ready()
	{
		MessageList = GetNode<ItemList>("MessageList");
		PlayersList = GetNode<ItemList>("PlayerList");

		Multiplayer.PeerConnected += OnPeerConnected;
		Multiplayer.PeerDisconnected += OnPeerDisconnected;
	}
	public void Host()//<-
	{
		Peer?.Close();
		Peer = new ENetMultiplayerPeer();
		var err = Peer.CreateServer(25565);
		if (err != Error.Ok)
		{
			SendMessage($"Failed to create server: {err}", "System");
			return;
		}
		Multiplayer.MultiplayerPeer = Peer;
		PlayerIndexList.Clear();
		PlayerPeerList.Clear();
		MessageList.Clear();
		PlayersList.Clear();
		NickName = "Server";
		SendMessage("Server created", "System");
		JoinList(NickName, 1);
	}
	public void Join()//<-
	{
		Peer?.Close();
		Peer = new ENetMultiplayerPeer();
		var err = Peer.CreateClient(Address, Port);
		if (err != Error.Ok)
		{
			SendMessage($"Failed to connect to server: {err}", "System");
			return;
		}
		Multiplayer.MultiplayerPeer = Peer;
		PlayerIndexList.Clear();
		PlayerPeerList.Clear();
		MessageList.Clear();
		PlayersList.Clear();
		NickName = "Player"+Multiplayer.GetUniqueId();
		SendMessage($"Welcome {NickName}", "System");
		JoinList(NickName, Multiplayer.GetUniqueId());
		GetNode<Label>("PlayerList/UniqueId").Text = Multiplayer.GetUniqueId().ToString();
	}
	public void OnPeerConnected(long peerId)
	{
		Rpc(nameof(JoinList), NickName, Multiplayer.GetUniqueId());
	}
	public void OnPeerDisconnected(long peerId)
	{
		if (peerId == 1)
		{
			SendMessage("Server closed", "System");
			return;
		}
		SendMessage($"{PlayerPeerList[peerId]} disconnected", "System");
		PlayersList.RemoveItem(PlayerIndexList[PlayerPeerList[peerId]]);
		PlayerPeerList.Remove(peerId);
	}
	[Rpc(MultiplayerApi.RpcMode.AnyPeer)]
	public void JoinList(string name, long peerId)
	{
		if (PlayerPeerList.ContainsKey(peerId))
		{
			return;
		}
		SendMessage($"{name} connected", "System");
		PlayerIndexList[name] = PlayersList.AddItem(name);
		PlayerPeerList[peerId] = name;
	}
	[Rpc(MultiplayerApi.RpcMode.AnyPeer)]
	public void ChangeNickName(long peerId, string NewName)
	{
		if (PlayerPeerList.Values.Contains(NewName))
			return;
		if (PlayerPeerList.TryGetValue(peerId, out string value))
			{
				PlayersList.SetItemText(PlayerIndexList[value], NewName);
				PlayerIndexList[NewName] = PlayerIndexList[value];
				PlayerIndexList.Remove(value);
				PlayerPeerList[peerId] = NewName;
			}
	}
	public void SetAddress(string address)//<-
	{
		Address = address.Split(':')[0];
		Port = int.Parse(address.Split(':')[1]);
	}
	[Rpc(MultiplayerApi.RpcMode.AnyPeer)]
	public void SendMessage(string message, string name)//<-
	{
		MessageList.AddItem($"{name}: {message}");
	}
	public void Refresh()
	{
		MessageList.Clear();
	}
	public void Enter(string message)//<-
	{
		SendMessage(message, "You");
		Rpc(nameof(SendMessage), message, NickName);
		GetNode<LineEdit>("Message").Text = "";
	}
	public void UpdateName(string name)//<-
	{
		Rpc(nameof(ChangeNickName), Multiplayer.GetUniqueId(), name);
		ChangeNickName(Multiplayer.GetUniqueId(), name);
		NickName = name;
	}
	public void Quit()//<-
	{
		GetTree().Quit();
	}
	public void OnPlayerClicked(int index, Vector2 position, int buttonIndex)
	{
		var label = new Label();
		PlayersList.AddChild(label);
		label.Text = PlayersList.GetItemText(index);
		label.Size = new Vector2(100, 20);
		foreach (var item in PlayerPeerList)
		{
			if (item.Value == PlayersList.GetItemText(index))
			{
				label.Text += $"\n({item.Key})";
			}
		}
		label.Position = position;
		GetTree().CreateTimer(1).Timeout += () => label.QueueFree();
	}
}
