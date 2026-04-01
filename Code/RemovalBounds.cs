using Godot;
using System;
using static CloneManager;

public partial class RemovalBounds : Area2D
{
	[Export]
	public BodyPartEnum bodyPartEnum;
	
	
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
