using Godot;
using System;

public partial class MainScene : HBoxContainer
{
	[Signal]
	public delegate void JoinSuccessSignalEventHandler();
	[Signal]
	public delegate void LogEventHandler(string message,string name);
	public int Port = 13677;
	public string Address = "frp-end.com";
	public ENetMultiplayerPeer Peer;

	public override void _Ready()
	{
		Multiplayer.PeerConnected += OnPeerConnected;
		Multiplayer.PeerDisconnected += OnPeerDisconnected;
	}
	public void Host()
	{
		if (Peer != null)
		{
			Peer.Close();
		}
		Peer = new ENetMultiplayerPeer();
		Peer.CreateServer(25565);
		Multiplayer.MultiplayerPeer = Peer;
		EmitSignal(nameof(Log),"Server created","System");
	}
	public void Join()
	{
		if (Peer != null)
		{
			Peer.Close();
		}
		Peer = new ENetMultiplayerPeer();
		Peer.CreateClient(Address, Port);
		Multiplayer.MultiplayerPeer = Peer;
	}
	public void OnPeerConnected(long peerId)
	{
		GD.Print($"Peer connected: {peerId}");
		RpcId(peerId, "JoinSuccess");
	}
	[Rpc(MultiplayerApi.RpcMode.AnyPeer)]
	public void JoinSuccess()
	{
		if (Peer.GetUniqueId() == 1)
		{
			return;
		}
		GD.Print($"{Peer.GetUniqueId()} Join success");
		EmitSignal(nameof(JoinSuccessSignal));
	}
	public void OnPeerDisconnected(long peerId)
	{
		GD.Print($"Peer disconnected: {peerId}");
	}
	public void SetAddress(string address)
	{
		Address = address.Split(':')[0];
		Port = int.Parse(address.Split(':')[1]);
		GD.Print($"Address set to {Address}:{Port}");
	}
}
