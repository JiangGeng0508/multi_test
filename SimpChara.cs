using Godot;
using System;

public partial class SimpChara : CharacterBody2D
{
	[Export]
	public Vector2 axis = Vector2.Zero;//控制向量
	//按下标识，防止空键抬起
	private bool[] PressFlag = [false, false, false, false];// 0000 UP DOWN LEFT RIGHT
	public override void _Input(InputEvent @event)
	{
		if (!IsMultiplayerAuthority())
		{
			return;
		}
		//控制
		if (@event is InputEventKey eventKey)
		{
			axis = Input.GetVector( "Left", "Right","Up", "Down");
		}
	}
	public override void _PhysicsProcess(double delta)
	{
		if(IsMultiplayerAuthority())
		{
			Move(axis);
		}
	}
	[Rpc(MultiplayerApi.RpcMode.Authority,CallLocal = false)]
	public void Move(Vector2 dir)
	{
		Velocity = dir * 100f;
		MoveAndSlide();
	}
	public override void _Notification(int what)
	{
		//失焦时清除控制向量，并将按键标志位清空
		if (what == MainLoop.NotificationApplicationFocusOut)
		{
			axis = Vector2.Zero;
		}
	}
}
