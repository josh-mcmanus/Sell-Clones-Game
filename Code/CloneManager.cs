using Godot;
using System;

public partial class CloneManager : Node2D
{
	private Vector2 currentGlobalMousePosition = Vector2.Zero;
	private Vector2 localMouseOffset = Vector2.Zero;
	
	private Area2D currentArea = null;
	
	private bool moveObject = false;
	
	
	[Export]
	private Node2D clone;
	
	public override void _Ready()
	{
		// Only grab the top most object in when selection is overlapping
		GetViewport().PhysicsObjectPickingSort = true;
		GetViewport().PhysicsObjectPickingFirstOnly = true;
		
		
		// Links all children signals in BodyParts.cs
		foreach (Node node in clone.GetChildren())
		{
			if (node is BodyParts part)
			{
				part.UpgradeSiteClicked += OnUpgradeSiteClicked;
				
				part.BodyPartHover += BodyPartHover;
				part.BodyPartHoverExit += BodyPartHoverExit;
				
				part.TorsoHover += OnTorsoHover;
				part.TorsoHoverExit += OnTorsoHoverExit;
			}
		}
		
		
		// Temp testing dragging
		AddBodyPartToScene("res://Scenes/Body Parts/l_arm.tscn");
		AddBodyPartToScene("res://Scenes/Body Parts/l_arm.tscn");
		AddBodyPartToScene("res://Scenes/Body Parts/l_arm.tscn");
	}
	
	
	public override void _Process(double delta)
	{
		if (currentArea == null)
		{
			return;
		}
		if (moveObject)
		{
			currentArea.GlobalPosition = currentGlobalMousePosition - localMouseOffset;
		}
	}
	
	
	public override void _Input(InputEvent @event)
	{	
		if (@event is InputEventMouseMotion motion)
		{
			currentGlobalMousePosition = motion.Position;
		}
	}
	
	
	private void OnBodyPartClicked(Area2D area)
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
	
	
	private void OnBodyPartReleased()
	{
		moveObject = false;
	}
	
	
	// Adds a body part as child to the scene, draggable
	// for adding onto the clone
	// Righ click on body part scene e.g. "l_arm.tscn" > Copy Path > paste as sceneDirectory
	private void AddBodyPartToScene(string sceneDirectory)
	{
		// load and add body part to scene
		var bodyPartScene = GD.Load<PackedScene>(sceneDirectory);
		
		BodyParts part = bodyPartScene.Instantiate<BodyParts>();
		
		CallDeferred(Node.MethodName.AddChild, part);

		// link signals to functions
		part.UpgradeSiteClicked += OnBodyPartClicked;
		part.UpgradeSiteReleased += OnBodyPartReleased;
	}
	
	
	
	
	
	// part removed
	// hover -> click -> stop hover
	private void RemoveBodyPart(Area2D area)
	{
		// player clicks on attached body part
		
		// player moves mouse outside of area
		
		// function is called
		
		// remove body part from clone 
			// delete from file
			// create new body part object that is clicked on at same position on mouse
	}
	
	// click appropriate type -> hover -> unclick
	// part added
	private void AddBodyPartToClone(Area2D area)
	{
		// check if player is holding part
		
		// player hovers over area
		
		// player releases mouse
		
		// call this function
		
		// check that both parts match
		
		// if they do, add it
			// set the position to a set location
			// lerp it to that position
			// save the setup to a file
	}
	
	// other checks like there needs to be higher part in higherarchy
	// cant add hand without arm
	private void IsValidBodyPartPlacement(Area2D areaToCheck, Area2D areaHolding)
	{
		// is hierarchy preserved?
		
		// i dont know how to do that
		
		// e.g. is hand < forearm < arm
	}
	
	
	
	// Called when an area is clicked
	private void OnUpgradeSiteClicked(Area2D area)
	{
		GD.Print(area.Name);
	}
	
	
	// Called when an area is hovered over with the mouse
	private void BodyPartHover(Area2D area)
	{
		GD.Print($"Hovering over {area.Name}");
	}
	
	// Called when an area is stopped hovering over with the mouse
	private void BodyPartHoverExit(Area2D area)
	{
		GD.Print($"Stopped hovering over {area.Name}");
	}
	
	
	// Called when the torso is hovered over with the mouse
	private void OnTorsoHover()
	{
		GD.Print("Hovering over torso");
	}
	
	// Called when the torso is stopped hovering over with the mouse
	private void OnTorsoHoverExit()
	{
		GD.Print("Stopped hovering over torso");
	}
}
