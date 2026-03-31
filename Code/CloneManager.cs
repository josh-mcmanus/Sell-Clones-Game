using Godot;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text.RegularExpressions;

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
	// Right click on body part scene e.g. "l_arm.tscn" > Copy Path > paste as sceneDirectory
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
		OnBodyPartClicked(area);
		area.ZIndex = 0;
		
		isAttachedPartSelected = true;
		
		// Initialize new area2d, for body part removal bounds
		var removalBoundsScene = GD.Load<PackedScene>("res://Scenes/removal_bounds.tscn");
		RemovalBounds removalBounds = removalBoundsScene.Instantiate<RemovalBounds>();
		AddChild(removalBounds);
		
		// Properties
		removalBounds.GlobalPosition = currentGlobalMousePosition;
		removalBounds.Name = area.Name;
		removalBounds.MouseExitedBounds += BodyPartHoverExit;
		
		GD.Print(area.Name);
	}
	
	
	// when the mouse is moved outside this area, part is removed
	private void MouseExitedRemovalBounds()
	{
		BodyPartHoverExit(currentArea);
	}
	
	// Mouse stops clicking on the upgrade site / attached body part
	private void OnUpgradeSiteReleased()
	{
		ResetAndDisableMovement();
	}
	
	
	private void ResetAndDisableMovement()
	{
		moveObject = false;
		isAttachedPartSelected = false;
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
		
		
		BodyPartEnum bodyPartEnum = GetBodyPartEnumFromString(area.Name);
		
		// get directory from dictionary
		if (!bodyParts.ContainsKey(bodyPartEnum))
		{
			ResetAndDisableMovement(); // TODO: This needs to reset the position of the clicked body part
			throw new ArgumentNullException($"Key: {bodyPartEnum}, could not be found in bodyParts dictionary");
		}
		
		var partDirectory = bodyParts.GetValueOrDefault(bodyPartEnum);
		
		
		// create new body part object
		AddBodyPartToScene(partDirectory);
		
		// delete old object
		currentArea.QueueFree();
		currentArea = null;
		
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
	
	
	
	private readonly Dictionary<BodyPartEnum, string> bodyParts = new()
	{
		{BodyPartEnum.LArm,	"res://Scenes/Body Parts/l_arm.tscn"},
		
		// remaining unfinished
		// {BodyPartEnum.LForearm,	"res://Scenes/Body Parts/"}, 
		// {BodyPartEnum.LHand,	"res://Scenes/Body Parts/"},
		// {BodyPartEnum.RArm,		"res://Scenes/Body Parts/"},
		// {BodyPartEnum.RForearm,	"res://Scenes/Body Parts/"},
		// {BodyPartEnum.RHand,	"res://Scenes/Body Parts/"},
		// {BodyPartEnum.LLeg,		"res://Scenes/Body Parts/"},
		// {BodyPartEnum.LCalf,	"res://Scenes/Body Parts/"},
		// {BodyPartEnum.LFoot,	"res://Scenes/Body Parts/"},
		// {BodyPartEnum.RLeg,		"res://Scenes/Body Parts/"},
		// {BodyPartEnum.RCalf,	"res://Scenes/Body Parts/"},
		// {BodyPartEnum.RFoot,	"res://Scenes/Body Parts/"},
		// {BodyPartEnum.Torso,	"res://Scenes/Body Parts/"},
		// {BodyPartEnum.Heart,	"res://Scenes/Body Parts/"},
		// {BodyPartEnum.Lung,		"res://Scenes/Body Parts/"},
		// {BodyPartEnum.Kidney,	"res://Scenes/Body Parts/"},
		// {BodyPartEnum.Liver,	"res://Scenes/Body Parts/"}
	};
	
	
	
	public static BodyPartEnum GetBodyPartEnumFromString(string input)
	{
		GD.Print(input);
		
		if (string.IsNullOrEmpty(input))
		{
			throw new ArgumentException("Input cannot be null or empty");
		}
		
		string withoutNumbers = Regex.Replace(input, "[0-9]", "");
		
		if(Enum.TryParse(withoutNumbers, out BodyPartEnum result))
		{
			return result;
		}
		
		throw new ArgumentException($"Invalid input string: {input}");
	}
	
	
	public enum BodyPartEnum
	{
		LArm,
		LForearm,
		LHand,
		
		RArm,
		RForearm,
		RHand,
		
		
		LLeg,
		LCalf,
		LFoot,
		
		RLeg,
		RCalf,
		RFoot,
		
		
		Torso,
		Heart,
		Lung,
		Kidney,
		Liver
	}
	
}
