using Godot;
using System;
using System.Collections.Generic;
using System.Data;

public partial class CloneManager : Node2D
{
	private Vector2 currentGlobalMousePosition = Vector2.Zero;
	private Vector2 localMouseOffset = Vector2.Zero;
	
	private Area2D currentArea = null;
	private int zIndex = 0;
	
	private bool moveObject = false;
	private bool isAttachedPartSelected = false;
	
	private BodyParts newlyAddedBodyPart;
	
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
				part.UpgradeSiteReleased += OnUpgradeSiteReleased;
				
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
	
	
	// When a lone unconnected body part is clicked
	// unrelated to when a connected body part is clicked
	private void OnBodyPartClicked(Area2D area)
	{
		GD.Print($"Clicked object: {area.Name}");
		
		if (currentArea == null)
		{
			currentArea = area;
			currentArea.ZIndex = zIndex;
		}
		
		localMouseOffset = area.GetLocalMousePosition();
		currentArea = area;
		moveObject = true;
		
		zIndex++;
		currentArea.ZIndex = zIndex;
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
		
		newlyAddedBodyPart = part;
		
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
	// For removing body parts
	private void OnUpgradeSiteClicked(Area2D area)
	{
		// store current object data
		currentArea = area;
		isAttachedPartSelected = true;
		moveObject = true;
		
		localMouseOffset = area.GetLocalMousePosition();
		
		
		// Initialize area2d
		var removalBoundsScene = GD.Load<PackedScene>("res://Scenes/removal_bounds.tscn");
		
		RemovalBounds removalBounds = removalBoundsScene.Instantiate<RemovalBounds>();
		
		// this should be deferred, because it might supposedly cause issues if done like this, because this is using a signal
		// then assign the child with a new function called inside the deferred setting the global position as the mouse position
		AddChild(removalBounds);
		
		removalBounds.GlobalPosition = currentGlobalMousePosition;
		
		removalBounds.MouseExitedBounds += MouseExitedRemovalBounds;
		
		GD.Print(area.Name);
	}
	
	
	// when the mouse is moved outside this area, part is removed
	private void MouseExitedRemovalBounds()
	{
		BodyPartHoverExit(currentArea);
	}
	
	
	private void OnUpgradeSiteReleased()
	{
		ResetAndDisableMovement();
	}
	
	
	private void ResetAndDisableMovement()
	{
		moveObject = false;
		isAttachedPartSelected = false;
		newlyAddedBodyPart = null;
		currentArea = null;
	}
	
	
	// Called when an area is hovered over with the mouse
	private void BodyPartHover(Area2D area)
	{
		GD.Print($"Hovering over {area.Name}");
	}
	
	
	// Called when an area is stopped hovering over with the mouse
	// Removes body part, when mouse is dragged far enough away, part is disconnected
	private void BodyPartHoverExit(Area2D area)
	{
		// Clicked down on body part for removal
		if (!isAttachedPartSelected)
		{
			GD.Print("test2");
			return;
		}
		
		
		GD.Print("test3");
		// TODO: some code here that saves to file

		
		// get directory from dictionary
		if (!bodyParts.ContainsKey(area.Name))
		{
			ResetAndDisableMovement();
			throw new ArgumentNullException($"Key: {area.Name}, could not be found in bodyParts dictionary");
		}
		
		var partDirectory = bodyParts.GetValueOrDefault(area.Name);
		
		// create new body part object
		AddBodyPartToScene(partDirectory);
		
		// move to mouse position
		newlyAddedBodyPart.GlobalPosition = currentGlobalMousePosition - localMouseOffset;
		
		// click onto it (follow the mouse untill unclick signal)
		currentArea = newlyAddedBodyPart;
		moveObject = true;
		
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
	
	
	
	private readonly Dictionary<string, string> bodyParts = new()
	{
		{"LArm",	"res://Scenes/Body Parts/l_arm.tscn"},
		
		// remaining unfinished
		// {"LForearm","res://Scenes/Body Parts/"}, 
		// {"LHand",	"res://Scenes/Body Parts/"},
		// {"RArm",	"res://Scenes/Body Parts/"},
		// {"RForearm","res://Scenes/Body Parts/"},
		// {"RHand",	"res://Scenes/Body Parts/"},
		// {"LLeg",	"res://Scenes/Body Parts/"},
		// {"LCalf",	"res://Scenes/Body Parts/"},
		// {"LFoot",	"res://Scenes/Body Parts/"},
		// {"RLeg",	"res://Scenes/Body Parts/"},
		// {"RCalf",	"res://Scenes/Body Parts/"},
		// {"RFoot",	"res://Scenes/Body Parts/"},
		// {"Torso",	"res://Scenes/Body Parts/"},
		// {"Heart",	"res://Scenes/Body Parts/"},
		// {"Lung",	"res://Scenes/Body Parts/"},
		// {"Kidney",	"res://Scenes/Body Parts/"},
		// {"Liver",	"res://Scenes/Body Parts/"}
	};
	
	
	
	
}
