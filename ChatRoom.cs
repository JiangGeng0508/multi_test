using Godot;
using System;

public partial class ChatRoom : Control
{
	[Signal]
	public delegate void SaySignalEventHandler(string message, string name);
	public string NickName { get; set; } = "Nameless";
	public string Message { get; set; }

	[Rpc(MultiplayerApi.RpcMode.AnyPeer)]
	public void Say(string message, string name)
	{
		GD.Print($"{name}: {message}");
		EmitSignal(nameof(SaySignal), message, name);
	}
	public void Enter(string message)
	{
		EmitSignal(nameof(SaySignal), message, "Self");
		Rpc(nameof(Say), message, NickName);
		GetNode<LineEdit>("Message").Clear();
	}
	public void UpdateName(string name)
	{
		NickName = name;
	}
	public void Join()
	{
		Say($"Welcome {NickName}!", "System");
		Rpc(nameof(Say), $"{NickName} connected.", "System");
	}
}
