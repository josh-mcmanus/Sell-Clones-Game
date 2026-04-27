using Godot;
using System;
using System.Collections.Generic;

public partial class CloneManager : Node2D
{
	[Export]
	private Node2D clone;
	
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
	private float offsetPosition;
	private float centerPosition;
	
	
	
	public override void _Ready()
	{
		// Only grab the top most object in when selection is overlapping
		// NOTE: This activates for the entire game project
		GetViewport().PhysicsObjectPickingSort = true;
		GetViewport().PhysicsObjectPickingFirstOnly = true;
		
		centerPosition = GetViewport().GetVisibleRect().Size.X / 2;
		
		// Links all children signals in BodyParts.cs
		foreach (Node node in clone.GetChildren())
		{
			if (node is BodyParts part)
			{
				if (part.isAttachmentArea)
				{
					part.sprite2D.Visible = false; // Attachment area default to hidden
				}
				part.UpgradeSiteClicked += OnUpgradeSiteClicked;
				
				part.TorsoHover += OnTorsoHover;
				part.TorsoHoverExit += OnTorsoHoverExit;
			}
		}
	}
	
	
	public override void _Process(double delta)
	{
		if (currentBodyPart == null)
		{
			return;
		}
		
		if (moveObject)
		{
			currentBodyPart.GlobalPosition = currentGlobalMousePosition + localMouseOffset;
		}
		
		HandleAttachment();
		HandleFlipping();
	}
	
	private void HandleAttachment()
	{
		var overlap = currentBodyPart.GetOverlappingAreas();
		
		isAttachmentOk = false;
		attachmentArea = null;
		
		foreach (var area in overlap)
		{
			// Valid
			if (area is BodyParts attachmentPart 
				&& attachmentPart.isAttachmentArea 
				&& attachmentPart.bodyPart == currentBodyPart.bodyPart
				&& attachmentPart.side == currentBodyPart.side)
			{
				isAttachmentOk = true;
				attachmentArea = attachmentPart;
				break;
			}
		}
	}
	
	
	// Set side based on mouse position
	private void HandleFlipping()
	{
		if (moveObject && currentBodyPart != null)
		{
			if (currentGlobalMousePosition.X < centerPosition) // mouse is left of center
			{
				currentBodyPart.side = Utils.Side.Left;
				currentBodyPart.Scale = new Vector2(1, 1);
			}
			else
			{
				currentBodyPart.side = Utils.Side.Right;
				currentBodyPart.Scale = new Vector2(-1, 1);
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
		var removalBoundsScene = GD.Load<PackedScene>("res://Scenes/Clone/removal_bounds.tscn");
		RemovalBounds removalBounds = removalBoundsScene.Instantiate<RemovalBounds>();
		AddChild(removalBounds);
		
		// Properties
		activeRemovalBounds = removalBounds;
		removalBounds.GlobalPosition = currentGlobalMousePosition;
		removalBounds.bodyPart = bodyPart.bodyPart;
		removalBounds.side = bodyPart.side;
		removalBounds.tier = bodyPart.tier;
		removalBounds.MouseExitedBounds += BodyPartRemovalBoundsExit;
	}
	
	
	// Activates when a body part is clicked on
	// For moving the selected body part around
		// NOTE: OnUpgradeSiteClicked is similar, and also calls this function, but also does other things relevant for ONLY the upgrade sites. 
		// e.g. the sites on the body where you can add upgrades to
	private void OnBodyPartClicked(BodyParts area)
	{
		Utils.Log(TAG, $"Clicked object type: {area.bodyPart}");
		
		// torso cant move
		if (area.bodyPart == Utils.BodyPart.Torso)
		{
			return;
		}
		
		if (currentBodyPart == null)
		{
			currentBodyPart = area;
			currentBodyPart.ZIndex = zIndex;
		}
		
		localMouseOffset = area.GlobalPosition - currentGlobalMousePosition;
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
		
		currentBodyPart = null;
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
		
		string partDirectory = "";
		
		// Get file directory
		try
		{
			partDirectory = Utils.GetBodyPartDirectory(removalBounds.bodyPart);
		}
		catch
		{
			ResetAndDisableMovement();
		}
		
		
		// Create new body part object
		BodyParts bodyPart = AddBodyPartToScene(partDirectory, removalBounds.side, removalBounds.tier);
		bodyPart.bodyPart = removalBounds.bodyPart;
		bodyPart.tier = removalBounds.tier;
		
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
		BodyParts attachmentArea = AddBodyPartToScene(partDirectory, removalBounds.side, removalBounds.tier);
		attachmentArea.GlobalPosition = initialPosition;
		attachmentArea.sprite2D.Visible = false;
		
		attachmentArea.isAttachmentArea = true;
		attachmentArea.bodyPart = removalBounds.bodyPart;
		attachmentArea.side = removalBounds.side;
		attachmentArea.tier = removalBounds.tier;
		
	}
	
	
	
	
	// Adds a draggable body part to the scene
	// NOTE: To get "sceneDirectory" - right click on body part scene in the file system in Godot, e.g. "shoulder.tscn" > Copy Path > paste as sceneDirectory
	public BodyParts AddBodyPartToScene(string sceneDirectory, Utils.Side side, Utils.UpgradeTier tier)
	{
		// Load and add to scene
		var bodyPartScene = GD.Load<PackedScene>(sceneDirectory);
		var part = bodyPartScene.Instantiate<BodyParts>();
		AddChild(part);
		
		// Flip on right
		if (side == Utils.Side.Right)
		{
			part.Scale = new Vector2(-1, 1);
		}
		
		part.side = side;
		part.tier = tier;
		
		var sprite = part.GetNode<Sprite2D>("Sprite2D");
		sprite.Modulate = Utils.bodyPartUpgradeTierColors[tier];
		
		// link signals to functions
		part.UpgradeSiteClicked += OnBodyPartClicked;
		part.UpgradeSiteReleased += OnBodyPartReleased;
		
		zIndex++;
		part.ZIndex = zIndex;
		
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
