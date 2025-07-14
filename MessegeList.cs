using Godot;
using System;

public partial class MessegeList : ItemList
{
	public void AddMessage(string name, string message)
	{
		AddItem(name + ": " + message);
	}
	public void Refresh()
	{
		Clear();
	}
}
