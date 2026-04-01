using Godot;
using System;


// Script used by all body part Area2D's that are children of the Clone Scene
// Making up the main internal and external body parts
// Functions run on clicking on / hovering over a body part

public partial class BodyParts : Area2D
{
	
	[Export]
	public Utils.BodyPartEnum bodyPartEnum;
	
	
	// On Click
	[Signal]
	public delegate void UpgradeSiteClickedEventHandler(BodyParts bodyPart);
	
	[Signal]
	public delegate void UpgradeSiteReleasedEventHandler();
	
	// On Hover
	[Signal]
	public delegate void BodyPartHoverEventHandler(BodyParts bodyPart);
	
	[Signal]
	public delegate void BodyPartHoverExitEventHandler(BodyParts bodyPart);
	
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
		if (@event.IsActionReleased("left_click"))
		{
			EmitSignal(SignalName.UpgradeSiteReleased);
		}
	}
	
	
	private void OnMouseEntered()
	{
		if (bodyPartEnum == Utils.BodyPartEnum.Torso)
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
		if (bodyPartEnum == Utils.BodyPartEnum.Torso)
		{
			EmitSignal(SignalName.TorsoHoverExit);
		}
		else
		{
			EmitSignal(SignalName.BodyPartHoverExit, this);
		}
		
	}
	
}
