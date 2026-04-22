using Godot;
using System;
using System.Collections.Generic;
using System.Linq;


// Class of the main game scene

public partial class Main : Control
{
	private const string TAG = "Main";
	
	[Export]
	private CloneManager cloneManager;
	
	private double timer = 0;
	private double researchCost = 10;
	private double nodeCostMultiplier = 1.5f;
	
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		foreach (Node node in GetTree().GetNodesInGroup("UpgradeTabButtons"))
		{
			if (node is ListUpgrade button)
			{
				button.BodyPartListItemClicked += AddBodyPart;
			}
		}
	}
	
	
	// Add body part on list item click
	private void AddBodyPart(ListUpgrade listUpgrade)
	{
		var partDirectory = Utils.GetBodyPartDirectory(listUpgrade.bodyPart);
		
		// TODO: add something to vary the placement,
		// set starting position to somewhere at bottom of tab
		// randomly variate by some amount inside of total area
		// set side based on position
		// Do inside of the main addbodyparttoscene function maybe
		BodyParts part = cloneManager.AddBodyPartToScene(partDirectory, Utils.Side.Left);
		part.bodyPart = listUpgrade.bodyPart;
	}
	
	
	
	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		timer += delta;

		if (timer >= 1.0)
		{
			CheckUpgradeStatus();
			
			timer -= 1.0;
		}
	}
	
	
	
	// Check if any new upgrades should be unlocked
	private void CheckUpgradeStatus()
	{
		foreach (var nodeType in Enum.GetValues<Utils.NodeType>())
		{
			if (nodeType == Utils.NodeType.Clone)
				continue;
			
			var lowestLevelBodyPart = Utils.GetLowestLevelUpgradeInCategory(nodeType);
			
			if(lowestLevelBodyPart == null)
				continue;
			
			// Enough points
			if (DataManager.saveData.ResearchPoints[nodeType] >= researchCost)
			{
				Utils.Log(TAG, $"Type: {nodeType}");
				Utils.Log(TAG, $"points: {DataManager.saveData.ResearchPoints[nodeType]}");
				Utils.Log(TAG, $"Research Cost: {researchCost}");
				
				
				DataManager.saveData.ResearchPoints[nodeType] -= researchCost;
				DataManager.saveData.UnlockedUpgrades[lowestLevelBodyPart.Value] += 1;
				
				researchCost = Math.Round(researchCost * nodeCostMultiplier);
				
				Utils.Log(TAG, $"New Points: {DataManager.saveData.ResearchPoints[nodeType]}");
				Utils.Log(TAG, $"New upgrade unlocked! {lowestLevelBodyPart} level {DataManager.saveData.UnlockedUpgrades[lowestLevelBodyPart.Value]}");
				
				UpdateUpgradesTab(nodeType);
				
				Utils.Log(TAG, "=========================\n");
			}
		}
	}
	
	
	// Update drop upgrades tab UI with new unlocked upgrades
	private void UpdateUpgradesTab(Utils.NodeType type)
	{
		var upgradeTabs = GetTree().GetNodesInGroup("UpgradesTabs");

		foreach (var node in upgradeTabs)
		{
			var tab = (UpgradesTab)node;
			
			if(tab.areaType != type)
				continue;
			
			tab.InitializeUpgradesTab();
		}
	}
	
	
	// calculates the next upgrade cost from the current number unlocked
	private int CalculateUpgradeCost(int numberOfUpgradesUnlocked)
	{
		return -1;
	}
	
	
	// Torso never unlock
	
	// look through all types
	
	
	
	
	
	
	
}
