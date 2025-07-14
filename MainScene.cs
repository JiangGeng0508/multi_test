using Godot;
using System;

public partial class MainScene : HBoxContainer
{
	[Signal]
	public delegate void JoinSuccessEventHandler();
	public int Port = 25565;
	public string Address = "127.0.0.1";
	public ENetMultiplayerPeer Peer;

	public override void _Ready()
	{
		Multiplayer.PeerConnected += OnPeerConnected;
	}
	public void Host()
	{
		if (Peer != null)
		{
			Peer.Close();
		}
		Peer = new ENetMultiplayerPeer();
		Peer.CreateServer(Port);
		Multiplayer.MultiplayerPeer = Peer;
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
		if (peerId == 1)
		return;
		GD.Print($"{Multiplayer.GetUniqueId()} Peer connected: {peerId}");
		EmitSignal(nameof(JoinSuccess));
	}
	public void SetAddress(string address)
	{
		Address = address;
		GD.Print($"Address set to {Address}");
	}
	public void SetPort(string port)
	{
		Port = port.ToInt();
		GD.Print($"Port set to {Port}");
	}
}
