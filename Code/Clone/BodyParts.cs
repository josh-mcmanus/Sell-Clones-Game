using Godot;
using System;


// Script used by all body part Area2D's that are children of the Clone Scene
// Making up the main internal and external body parts
// Functions run on clicking on / hovering over a body part

public partial class BodyParts : Area2D
{
	
	[Export]
	public Utils.BodyPart bodyPart;
	
	[Export]
	public Utils.Side side;
	
	[Export]
	public Utils.UpgradeTier tier;
	
	
	// On Click
	[Signal]
	public delegate void UpgradeSiteClickedEventHandler(BodyParts bodyPart);
	
	[Signal]
	public delegate void UpgradeSiteReleasedEventHandler();
	
	[Signal]
	public delegate void TorsoClickEventHandler();
	
	// On Hover
	[Signal]
	public delegate void BodyPartHoverEventHandler(BodyParts bodyPart);
	
	[Signal]
	public delegate void BodyPartHoverExitEventHandler(BodyParts bodyPart);
	
	[Signal]
	public delegate void TorsoHoverEventHandler();
	
	[Signal]
	public delegate void TorsoHoverExitEventHandler();
	
	
	private const string TAG = "BodyParts";
	
	
	private bool isHovering = false;
	public bool isAttachmentArea = false;
	
	public Sprite2D sprite2D;
	private Area2D torsoArea;
	
	
	public override void _Ready()
	{
		InputEvent += OnInputEvent;
		
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
			Utils.Log(TAG, Name);
			
			if (isAttachmentArea)
				return;
			
			if(bodyPart == Utils.BodyPart.Torso)
			{
				EmitSignal(SignalName.TorsoClick);
				return;
			}
			
			EmitSignal(SignalName.UpgradeSiteClicked, this);
		}
		
		if (@event.IsActionReleased("left_click"))
		{
			EmitSignal(SignalName.UpgradeSiteReleased);
		}
	}
	
	
	
}
