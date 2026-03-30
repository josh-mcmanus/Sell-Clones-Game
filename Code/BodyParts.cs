using Godot;
using System;


// Script used by all body part Area2D's that are children of the Clone Scene
// Making up the main internal and external body parts
// Functions run on clicking on / hovering over a body part

public partial class BodyParts : Area2D
{
	// On Click
	[Signal]
	public delegate void UpgradeSiteClickedEventHandler(Area2D area);
	
	[Signal]
	public delegate void UpgradeSiteReleasedEventHandler();
	
	// On Hover
	[Signal]
	public delegate void BodyPartHoverEventHandler(Area2D area);
	
	[Signal]
	public delegate void BodyPartHoverExitEventHandler(Area2D area);
	
	[Signal]
	public delegate void TorsoHoverEventHandler();
	
	[Signal]
	public delegate void TorsoHoverExitEventHandler();
	
	
	
	private bool isHovering = false;
	
	private Area2D torsoArea;
	
	
	public override void _Ready()
	{
		InputEvent += OnInputEvent;
		MouseEntered += OnMouseEntered;
		MouseExited += OnMouseExited;
	}
	
	
	// Overrites the InputEvent, emits a signal when clicking on an Area2D
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
	
	
	private void OnMouseEntered()
	{
		if (Name == "Torso")
		{
			EmitSignal(SignalName.TorsoHover);
		}
		else
		{
			EmitSignal(SignalName.BodyPartHover, this);
		}
	}
	
	private void OnMouseExited()
	{
		if (Name == "Torso")
		{
			EmitSignal(SignalName.TorsoHoverExit);
		}
		else
		{
			EmitSignal(SignalName.BodyPartHoverExit, this);
		}
		
	}
	
}
