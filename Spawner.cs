using Godot;
using System;

public partial class Spawner : MultiplayerSpawner
{
	[Export]
	public PackedScene CharaScene { get; set; }
	public override void _Ready()
	{
		Multiplayer.PeerConnected += SpawnChara;
	}
	public void SpawnChara(long peerId = 1)
	{
		GD.Print($"Spawn {peerId}");
		var chara = CharaScene.Instantiate<SimpChara>();
		chara.Name = $"SimpChara{peerId}";
		chara.Position = new Vector2(GD.Randf() * 1000f - 500f, GD.Randf() * 500f - 250f);
		chara.AddChild(new Label { Text = peerId.ToString() });
		chara.SetMultiplayerAuthority((int)peerId);
		AddChild(chara);
	}
	public Node SpawnM(Variant data)
	{
		var arr = data.AsStringArray();
		GD.Print($"Spawn {arr[0]}");
		var chara = CharaScene.Instantiate<SimpChara>();
		chara.Name = arr[0];
		chara.Position = new Vector2(GD.Randf() * 1000f - 500f, GD.Randf() * 500f - 250f);
		chara.AddChild(new Label { Text = arr[1] });
		chara.SetMultiplayerAuthority(arr[1].ToInt());
		return chara;
	}

}
