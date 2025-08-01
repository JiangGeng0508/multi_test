using Godot;
using System;

public partial class StartMenu : Control
{
	public void SimpChat() => GetTree().ChangeSceneToFile("res://SimpChat.tscn");
	public void MoveTest() => GetTree().ChangeSceneToFile("res://MoveTest.tscn");
}
