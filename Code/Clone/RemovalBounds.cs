using Godot;
using System;

public partial class RemovalBounds : Area2D
{
	[Export]
	public Utils.BodyPart bodyPart;
	
	[Export]
	public Utils.Side side;
	
	
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
