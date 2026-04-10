using Godot;
using System;
using System.Collections.Generic;

public partial class CloneManager : Node2D
{
	[Export]
	private Node2D clone;
	
	[Export]
	private TorsoMenu torsoMenu;
	
	private const string TAG = "CloneManager";
	
	// Mouse position and offset
	private Vector2 currentGlobalMousePosition = Vector2.Zero;
	private Vector2 localMouseOffset = Vector2.Zero;
	private Vector2 initialPosition = Vector2.Zero;
	
	// Body part removal
	private BodyParts currentBodyPart = null;
	private BodyParts attachmentArea = null;
	private RemovalBounds activeRemovalBounds = null;
	
	// Movement checks
	private bool moveObject = false;
	private bool isAttachedPartSelected = false;
	private bool isSelectedBodyPartAttached = false;
	private bool isAttachmentOk = false;
	private bool isTorsoMenuOpen = false;
	
	private int zIndex = 0;
	
	
	
	public override void _Ready()
	{
		// Only grab the top most object in when selection is overlapping
		// NOTE: This activates for the entire game project
		GetViewport().PhysicsObjectPickingSort = true;
		GetViewport().PhysicsObjectPickingFirstOnly = true;
		
		
		// Links all children signals in BodyParts.cs
		foreach (Node node in clone.GetChildren())
		{
			if (node is BodyParts part)
			{
				part.UpgradeSiteClicked += OnUpgradeSiteClicked;
				part.TorsoClick += OnTorsoClick;
				
				part.TorsoHover += OnTorsoHover;
				part.TorsoHoverExit += OnTorsoHoverExit;
			}
		}
		
		torsoMenu.CloseTorsoMenu += OnTorsoMenuCloseButtonClick;
	}
	
	
	public override void _Process(double delta)
	{
		if (currentBodyPart == null)
		{
			return;
		}
		
		// Only prevent movement if the torso menu is open and the selected body part is not an internal organ
		if (isTorsoMenuOpen && !Utils.IsInternalBodyPart(currentBodyPart.bodyPartEnum))
		{
			return;
		}
		if (moveObject)
		{
			currentBodyPart.GlobalPosition = currentGlobalMousePosition - localMouseOffset;
		}
		
		
		// Detect if held body part is overlapping body part area
		if (moveObject && currentBodyPart == null)
		{
			return;
		}
		
		var overlap = currentBodyPart.GetOverlappingAreas();
		
		isAttachmentOk = false;
		attachmentArea = null;
		
		foreach (var area in overlap)
		{
			// overlapping area is valid
			if (area is BodyParts part && part.isAttachmentArea && part.bodyPartEnum == currentBodyPart.bodyPartEnum)
			{
				isAttachmentOk = true;
				attachmentArea = part;
				break;
			}
		}
	}
	
	
	public override void _Input(InputEvent @event)
	{	
		if (@event is InputEventMouseMotion motion)
		{
			currentGlobalMousePosition = motion.Position;
		}
		
		// Mouse stops clicking on the upgrade site / attached body part
		if (@event.IsActionReleased("left_click"))
		{
			// Only allow movement when the torso menu is closed
			if (isTorsoMenuOpen)
				return;
			
			// Only trigger when object is being actively moved (clicked on), prevent unnecessary calls
			if (moveObject)
			{
				ResetAndDisableMovement();
			}
		}
	}
	
	
	
	
	// Activates when an upgrade site is clicked
	// For removing body parts from the clone
	// NOTE: The same as OnBodyPartClicked, but also initializes the removal process
	private void OnUpgradeSiteClicked(BodyParts bodyPart)
	{
		OnBodyPartClicked(bodyPart);
		bodyPart.ZIndex = 0;
		
		isAttachedPartSelected = true;
		isSelectedBodyPartAttached = true;
		initialPosition = bodyPart.GlobalPosition;
		currentBodyPart = bodyPart;
		
		// Initialize new area2d, for body part removal bounds
		var removalBoundsScene = GD.Load<PackedScene>("res://Scenes/removal_bounds.tscn");
		RemovalBounds removalBounds = removalBoundsScene.Instantiate<RemovalBounds>();
		AddChild(removalBounds);
		
		// Properties
		activeRemovalBounds = removalBounds;
		removalBounds.GlobalPosition = currentGlobalMousePosition;
		removalBounds.bodyPartEnum = bodyPart.bodyPartEnum;
		removalBounds.MouseExitedBounds += BodyPartRemovalBoundsExit;
	}
	
	
	// Activates when a body part is clicked on
	// For moving the selected body part around
	// NOTE: OnUpgradeSiteClicked is similar, and also calls this function, but also does other things relevant for ONLY the upgrade sites. e.g. the sites on the body where you can add upgrades to
	private void OnBodyPartClicked(BodyParts area)
	{
		Utils.Log(TAG, $"Clicked object type: {area.bodyPartEnum}");
		
		if (isTorsoMenuOpen)
		{
			return;
		}
		
		if (currentBodyPart == null)
		{
			currentBodyPart = area;
			currentBodyPart.ZIndex = zIndex;
		}
		
		localMouseOffset = area.GetLocalMousePosition();
		currentBodyPart = area;
		moveObject = true;
		isSelectedBodyPartAttached = false;
		currentBodyPart.InputPickable = false;
		
		zIndex++;
		currentBodyPart.ZIndex = zIndex;
	}
	
	
	// Activates when the player releases left mouse click
	// For attaching the selected body part onto the clone
	private void OnBodyPartReleased()
	{
		if (isAttachmentOk)
		{
			Utils.Log(TAG, "Attachment is ok");
			
			// Move held body part into correct position
			currentBodyPart.GlobalPosition = attachmentArea.GlobalPosition;
			
			// Set click to activate removal process
			currentBodyPart.UpgradeSiteClicked += OnUpgradeSiteClicked;
			
			// Reset for next body part
			isAttachmentOk = false;
			
			// Delete attachment area
			attachmentArea.QueueFree();
			attachmentArea = null;
		}
	}
	
	
	
	
	// Activates when the player left clicks on the torso
	// For opening the torso menu
	private void OnTorsoClick()
	{
		// need to gradually fade out the clone in the background here
		
		// disable all movement outside of the torso menu
		isTorsoMenuOpen = true;
		
		zIndex++;
		torsoMenu.ZIndex = zIndex;
		torsoMenu.Visible = true;
	}
	
	private void OnTorsoMenuCloseButtonClick()
	{
		isTorsoMenuOpen = false;
		
		torsoMenu.Visible = false;
		
		// Activate closing animation
		// Prevents torso staying open, as it isnt triggered
		OnTorsoHoverExit();
	}
	
	
	private void OnTorsoHover()
	{
		if (isTorsoMenuOpen)
		{
			return;
		}
		
		Utils.Log(TAG, "Enter Torso");
		
		// Play torso opening animation
	}
	
	private void OnTorsoHoverExit()
	{
		if (isTorsoMenuOpen)
		{
			return;
		}
		
		Utils.Log(TAG, "Exit Torso");
		
		// Play torso closing animation
	}
	
	
	
	
	// Activates when the mouse exits the removeBounds area
	// For removing body parts
	// Creates identical body part on removal, seperated from clone, deletes old attached part
	private void BodyPartRemovalBoundsExit(RemovalBounds removalBounds)
	{
		// Clicked down on body part for removal
		if (!isAttachedPartSelected)
		{
			return;
		}
		
		// TODO: some code here that saves to file
		
		
		// Get file directory
		if (!Utils.bodyPartsDictionary.ContainsKey(removalBounds.bodyPartEnum))
		{
			ResetAndDisableMovement();
			throw new ArgumentNullException($"Key: {removalBounds.bodyPartEnum}, could not be found in bodyParts dictionary");
		}
		
		var partDirectory = Utils.bodyPartsDictionary.GetValueOrDefault(removalBounds.bodyPartEnum);
		
		
		// Create new body part object
		BodyParts bodyPart = AddBodyPartToScene(partDirectory);
		bodyPart.bodyPartEnum = removalBounds.bodyPartEnum;
		
		// Delete old body part and removalbounds
		currentBodyPart.QueueFree();
		currentBodyPart = null;
		activeRemovalBounds.QueueFree();
		activeRemovalBounds = null;
		
		// Move to current mouse position
		bodyPart.GlobalPosition = currentGlobalMousePosition - localMouseOffset;
		
		// Follow the mouse until unclick signal
		currentBodyPart = bodyPart;
		moveObject = true;
		isAttachedPartSelected = false;
		isSelectedBodyPartAttached = false;
		
		
		// Create bounds area for attaching new body part
		BodyParts attachmentArea = AddBodyPartToScene(partDirectory);
		attachmentArea.GlobalPosition = initialPosition;
		attachmentArea.isAttachmentArea = true;
		attachmentArea.sprite2D.Visible = false;
	}
	
	
	
	
	// Adds a draggable body part to the scene
	// NOTE: To get "sceneDirectory" - right click on body part scene in the file system in Godot, e.g. "l_arm.tscn" > Copy Path > paste as sceneDirectory
	private BodyParts AddBodyPartToScene(string sceneDirectory)
	{
		// Load and add to scene
		var bodyPartScene = GD.Load<PackedScene>(sceneDirectory);
		var part = bodyPartScene.Instantiate<BodyParts>();
		AddChild(part);
		
		// link signals to functions
		part.UpgradeSiteClicked += OnBodyPartClicked;
		part.UpgradeSiteReleased += OnBodyPartReleased;
		
		return part;
	}
	
	
	// Call to reset everything
	private void ResetAndDisableMovement()
	{
		moveObject = false;
		isAttachedPartSelected = false;
		
		if (currentBodyPart != null)
		{
			currentBodyPart.InputPickable = true; // restore movement
		}
		
		if (!isSelectedBodyPartAttached)
		{
			return;
		}
		
		// Position
		currentBodyPart.GlobalPosition = initialPosition;
		currentBodyPart = null;
		initialPosition = Vector2.Zero;
		
		// Mouse
		localMouseOffset = Vector2.Zero;
		
		// Delete Removal Bounds
		activeRemovalBounds.QueueFree();
		activeRemovalBounds = null;
	}
	
	
}
