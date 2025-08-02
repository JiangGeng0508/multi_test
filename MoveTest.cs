using Godot;

public partial class MoveTest : Node2D
{
	[Export]
	public string SpawnPath { get; set; } = ".";
	public int Port = 25565;
	public string Address = "127.0.0.1";
	public ENetMultiplayerPeer Peer;
	public Spawner Spawner;

	public override void _Ready()
	{
		Multiplayer.PeerConnected += OnPeerConnected;
		Multiplayer.ConnectedToServer += OnConnetToServer;
		Spawner = GetNode<Spawner>("Spawner");
	}
	public void Host()
	{
		Peer?.Close();
		Peer = new ENetMultiplayerPeer();
		var err = Peer.CreateServer(25565);
		if (err != Error.Ok)
		{
			return;
		}
		Multiplayer.MultiplayerPeer = Peer;
		Spawner.SpawnChara(1);
	}
	public void Join()//<-
	{
		Peer?.Close();
		Peer = new ENetMultiplayerPeer();
		var err = Peer.CreateClient(Address, Port);
		if (err != Error.Ok)
		{
			return;
		}
		Multiplayer.MultiplayerPeer = Peer;
	}
	public void SetAddress(string address)//<-
	{
		Address = address.Split(':')[0];
		Port = int.Parse(address.Split(':')[1]);
	}
	public void OnPeerConnected(long peerId)
	{
		GetNode<Label>("Camera2D/Id").Text = Multiplayer.GetUniqueId().ToString();
		Spawner.SpawnChara(peerId);
	}
	public void OnConnetToServer()
	{
		if (Multiplayer.IsServer()) return;

		GD.Print($"Im client and I Spawn {Multiplayer.GetUniqueId()}");
		Spawner.SpawnChara(Multiplayer.GetUniqueId());
	}
	
}
