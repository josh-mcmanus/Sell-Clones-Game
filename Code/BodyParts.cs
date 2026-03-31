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
	public bool isAttachmentArea = false;
	
	public Sprite2D sprite2D;
	private Area2D torsoArea;
	
	
	public override void _Ready()
	{
		InputEvent += OnInputEvent;
		MouseEntered += OnMouseEntered;
		MouseExited += OnMouseExited;
		
		foreach (Node node in GetChildren())
		{
			if (node is Sprite2D sprite)
			{
				sprite2D = sprite;
			}
		}
	}
	
	
	// Overrites the InputEvent, emits a signal when clicking on an Area2D
	private void OnInputEvent(Node viewport, InputEvent @event, long shapeIdx)
	{
		if (@event.IsActionPressed("left_click"))
		{
			if (isAttachmentArea)
				return;
			
			EmitSignal(SignalName.UpgradeSiteClicked, this);
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
