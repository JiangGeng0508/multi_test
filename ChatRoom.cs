using Godot;
using System;

public partial class ChatRoom : Control
{
	[Signal]
	public delegate void SaySignalEventHandler(string name, string message);
	public string NickName { get; set; } = "Nameless";
	public string Message { get; set; }

	[Rpc(MultiplayerApi.RpcMode.AnyPeer)]
	public void Say(string name, string message)
	{
		GD.Print($"{name}: {message}");
		EmitSignal(nameof(SaySignal), name, message);
	}
	public void Enter(string message)
	{
		EmitSignal(nameof(SaySignal), "Self", message);
		Rpc(nameof(Say), NickName, message);
		GetNode<LineEdit>("Message").Clear();
	}
	public void UpdateName(string name)
	{
		NickName = name;
	}
	public void Join()
	{
		Rpc(nameof(Say), "System", $"{NickName} joined the chat room.");
		EmitSignal(nameof(SaySignal), "System", $"You Joined the chat room.");
	}
}
