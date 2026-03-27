using Godot;
using System;

public partial class BodyParts : Area2D
{
	
	[Signal]
	public delegate void UpgradeSiteClickedEventHandler(Area2D area);
	
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
	
	private void OnInputEvent(Node viewport, InputEvent @event, long shapeIdx)
	{
		if (Name == "Torso")
		{
			return;
		}
		
		if (!@event.IsActionPressed("left_click"))
		{
			return;
		}
		
		EmitSignal(SignalName.UpgradeSiteClicked, this);
	}
	
	private void OnMouseEntered()
	{
		if (Name.Equals("Torso"))
		{
			EmitSignal(SignalName.TorsoHover);
		}
	}
	
	private void OnMouseExited()
	{
		if (Name.Equals("Torso"))
		{
			EmitSignal(SignalName.TorsoHoverExit);
		}
	}
	
}
