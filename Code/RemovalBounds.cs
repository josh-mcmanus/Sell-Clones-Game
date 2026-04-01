using Godot;
using System;

public partial class RemovalBounds : Area2D
{
	[Export]
	public Utils.BodyPartEnum bodyPartEnum;
	
	
	[Signal]
	public delegate void MouseExitedBoundsEventHandler(RemovalBounds removalBounds);
	
	
	public override void _Ready()
	{
		MouseExited += OnMouseExitBounds;
	}
	
	private void OnMouseExitBounds()
	{
		EmitSignal(SignalName.MouseExitedBounds, this);
	}
	
}
