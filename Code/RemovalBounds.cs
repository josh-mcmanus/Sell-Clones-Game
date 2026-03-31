using Godot;
using System;

public partial class RemovalBounds : Area2D
{
	[Signal]
	public delegate void MouseExitedBoundsEventHandler(Area2D area);
	
	
	public override void _Ready()
	{
		MouseExited += OnMouseExitBounds;
	}
	
	private void OnMouseExitBounds()
	{
		EmitSignal(SignalName.MouseExitedBounds, this);
	}
	
}
