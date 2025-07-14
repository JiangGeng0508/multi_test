using Godot;
using System;

public partial class MessegeList : ItemList
{
	public void AddMessage(string message, string name)
	{
		AddItem(name + ": " + message);
	}
	public void Refresh()
	{
		Clear();
	}
}
