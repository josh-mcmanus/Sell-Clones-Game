using Godot;
using System;

public partial class DraggingTest : Area2D
{
	[Signal]
	public delegate void UpgradeSiteClickedEventHandler(Area2D area);
	
	[Signal]
	public delegate void UpgradeSiteReleasedEventHandler();
	
	
	public override void _Ready()
	{
		InputEvent += OnInputEvent;
	}
	
	
	// Sends signal when object is clicked and unclicked
	private void OnInputEvent(Node viewport, InputEvent @event, long shapeIdx)
	{
		if (@event.IsActionPressed("left_click"))
		{
			EmitSignal(SignalName.UpgradeSiteClicked, this);
		}
		else if (@event.IsActionReleased("left_click"))
		{
			EmitSignal(SignalName.UpgradeSiteReleased);
		}
	}
}
