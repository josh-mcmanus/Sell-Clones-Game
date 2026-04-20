using Godot;
using System;
using System.Collections.Generic;
using System.Linq;


// Class of the main game scene

public partial class Main : Control
{
	[Export]
	private CloneManager cloneManager;
	
	
	// [Export]
	// private UpgradesTab brainUpgradesTab;
	
	// [Export]
	// private UpgradesTab limbUpgradesTab;
	
	// [Export]
	// private UpgradesTab organUpgradesTab;
	
	// [Export]
	// private UpgradesTab tankUpgradesTab;
	
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		// Assign list buttons click function
		
		foreach (Node node in GetTree().GetNodesInGroup("UpgradeTabButtons"))
		{
			if (node is ListUpgrade button)
			{
				button.BodyPartListItemClicked += AddBodyPart;
			}
			
			// var list = (FoldableBodyPartList) upgradesTab.listArea.;
			
			// if (label.labelType == HoverInfoLabel.LabelType.InfoLabel)
			// {
			// 	label.QueueFree();
			// }
		}
		
		
		// foreach (Node node in GetChildren())
		// {
		// 	if (node is UpgradesTab upgradesTab)
		// 	{
		// 		 upgradesTab.GetChild
				
		// 		// sprite2D = sprite;
		// 	}
		// }
		
		
		
		
		// Get and populate list of upgrades
		
		// loop through all nodes
			// when get to the upgradeslist
			// go through unlockedupgrades
				// if type matches, add to list, initilaize list item node
				// else skip
		
		
		// foreach (Node node in GetChildren())
		// {
		// 	if (node is BodyParts part)
		// 	{
		// 		part.UpgradeSiteClicked += OnUpgradeSiteClicked;
		// 		// part.TorsoClick += OnTorsoClick;
				
		// 		part.TorsoHover += OnTorsoHover;
		// 		part.TorsoHoverExit += OnTorsoHoverExit;
		// 	}
		// }
		
		
		
		// DataManager.saveData.UnlockedUpgrades
	}
	
	
	// Add body part on list item click
	private void AddBodyPart(ListUpgrade listUpgrade)
	{
		// TEMP FOR NOW
		//TODO: Figure out some way of side switching if its on the left or right, or something else
		
		var partDirectory = Utils.GetBodyPartDirectory(listUpgrade.bodyPart);
		
		BodyParts part = cloneManager.AddBodyPartToScene(partDirectory, Utils.Side.Left);
		part.bodyPart = listUpgrade.bodyPart;
		
	}
	
	
	
	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		// Check for enough research points
		
		// if enough, unlock next upgrade
			// -> go through all upgrades once
			// -> then upgrade to next level
		
		
		CheckUpgradeStatus();
	}
	
	
	
	// Checks if any new upgrades have been unlocked
	private void CheckUpgradeStatus()
	{
		
		
		
		// get research from save data
		// loop through
		// check each type
		
		// if enough, add new upgrade
			// increase upgrade price
						// way to caluclate / store upgrade price
		
		
		// price scales up
		// e.g. 10 limb upgrades = 10,000 upgrade points
		// loop through all upgrades, count up the total number of unlocks
			// savedata.unlockedupgrades counter for all of the same type
	}
	
	
	// calculates the next upgrade cost from the current number unlocked
	private int CalculateUpgradeCost(int numberOfUpgradesUnlocked)
	{
		return -1;
	}
	
	
	// Torso never unlock
	
	// look through all types
	
	
	
	
	
	
	
}
