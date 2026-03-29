using Godot;
using System;

public partial class DragTest2 : Node2D
{
	private Vector2 currentGlobalMousePosition = Vector2.Zero;
	private Vector2 localMouseOffset = Vector2.Zero;
	
	private Area2D currentArea = null;
	
	private bool moveObject = false;
	
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		// Pickup objects by the highest z index
		GetViewport().PhysicsObjectPickingSort = true;
		GetViewport().PhysicsObjectPickingFirstOnly = true;
		
		
		// Links all children signals in BodyParts.cs
		// To this manager / parent class functions
		foreach (Node node in GetChildren())
		{
			if (node is DraggingTest part)
			{
				part.UpgradeSiteClicked += OnUpgradeSiteClicked;
				part.UpgradeSiteReleased += OnUpgradeSiteReleased;
			}
		}
	}
	
	
	
	public override void _Process(double delta)
	{
		if (currentArea == null)
		{
			return;
		}
		if (moveObject)
		{
			currentArea.Position = currentGlobalMousePosition - localMouseOffset;
		}
	}
	
	
	public override void _Input(InputEvent @event)
	{	
		if (@event is InputEventMouseMotion motion)
		{
			currentGlobalMousePosition = motion.Position;
		}
	}
	
	
	private void OnUpgradeSiteClicked(Area2D area)
	{
		GD.Print($"Clicked object: {area.Name}");
		
		if (currentArea == null)
		{
			currentArea = area;
			currentArea.ZIndex = 1;
		}
		else
		{
			// Set previous object to behind
			currentArea.ZIndex = 0;
		}
		
		
		localMouseOffset = area.GetLocalMousePosition();
		currentArea = area;
		moveObject = true;
		
		// Set current object to in front
		currentArea.ZIndex = 1;
	}
	
	
	private void OnUpgradeSiteReleased()
	{
		moveObject = false;
	}
	
}
