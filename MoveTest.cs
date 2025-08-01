using Godot;

public partial class MoveTest : Node2D
{
	[Export]
	public PackedScene CharaScene { get; set; }
	public MultiplayerSpawner Spawner;
	public int Port = 25565;
	public string Address = "127.0.0.1";
	public ENetMultiplayerPeer Peer;

	public override void _Ready()
	{
		Spawner = GetNode<MultiplayerSpawner>("Spawner");
		Spawner.SpawnFunction = new Callable(this, nameof(Spawn));
		Multiplayer.PeerConnected += OnPeerConnected;
	}
	public void Host()//<-
	{
		Peer?.Close();
		Peer = new ENetMultiplayerPeer();
		var err = Peer.CreateServer(25565);
		if (err != Error.Ok)
		{
			return;
		}
		Multiplayer.MultiplayerPeer = Peer;
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
	public void OnPeerConnected(long peerId)
	{
		GetNode<Label>("Id").Text = Multiplayer.GetUniqueId().ToString();
		Rpc(nameof(SpawnChara), peerId);
		// Rpc(nameof(SpawnChara), Multiplayer.GetUniqueId());	
	}
	public void SetAddress(string address)//<-
	{
		Address = address.Split(':')[0];
		Port = int.Parse(address.Split(':')[1]);
	}
	[Rpc(MultiplayerApi.RpcMode.AnyPeer, CallLocal = false)]
	public void SpawnChara(int peerId)
	{
		GD.Print(Multiplayer.GetRemoteSenderId() + " " + peerId);
		Spawner.Spawn(new string[] { $"SimpChara{peerId}", peerId.ToString(), });
	}
	public Node Spawn(Variant data)
	{
		var chara = CharaScene.Instantiate<SimpChara>();
		var arr = data.AsStringArray();
		chara.Name = arr[0];
		chara.SetMultiplayerAuthority(arr[1].ToInt());
		return chara;
	}
}
