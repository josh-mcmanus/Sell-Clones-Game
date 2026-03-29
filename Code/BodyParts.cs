using Godot;
using System;


// Script used by all body part Area2D's that are children of the Clone Scene
// Making up the main internal and external body parts
// Functions run on clicking on / hovering over a body part

public partial class BodyParts : Area2D
{
	
	[Signal]
	public delegate void UpgradeSiteClickedEventHandler(Area2D area);
	
	// [Signal]
	// public delegate void UpgradeSiteReleasedEventHandler();
	
	[Signal]
	public delegate void TorsoHoverEventHandler();
	
	[Signal]
	public delegate void TorsoHoverExitEventHandler();
	
	
	public override void _Ready()
	{
		InputEvent += OnInputEvent;
		MouseEntered += OnMouseEntered;
		MouseExited += OnMouseExited;
	}
	
	
	// Overrites the InputEvent
	// Emits a signal when clicking on an Area2D
	// Except the torso
	private void OnInputEvent(Node viewport, InputEvent @event, long shapeIdx)
	{
		if (Name == "Torso")
		{
			return;
		}
		
		if (@event.IsActionPressed("left_click"))
		{
			EmitSignal(SignalName.UpgradeSiteClicked, this);
		}
		// else if (@event.IsActionReleased("left_click"))
		// {
		// 	EmitSignal(SignalName.UpgradeSiteReleased);
		// }
	}
	
	
	// Emits a signal when the torso is hovered over
	// To be used to play the opening animation
	private void OnMouseEntered()
	{
		if (Name.Equals("Torso"))
		{
			EmitSignal(SignalName.TorsoHover);
		}
	}
	
	// To be used to play the closing animation
	private void OnMouseExited()
	{
		if (Name.Equals("Torso"))
		{
			EmitSignal(SignalName.TorsoHoverExit);
		}
	}
	
}
