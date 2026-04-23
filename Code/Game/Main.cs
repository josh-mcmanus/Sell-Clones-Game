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
	private double baseCost = 20;
	private double nodeCostMultiplier = 1.5f;
	
	private List<UpgradeProgressBar> progressBars = [];
	private List<UpgradesTab> upgradeTabs = [];
	
	
	
	public override void _Ready()
	{
		foreach (Node node in GetTree().GetNodesInGroup("UpgradeTabButtons"))
		{
			if (node is ListUpgrade button)
			{
				button.BodyPartListItemClicked += AddBodyPart;
			}
		}
		
		foreach (Node node in GetTree().GetNodesInGroup("UpgradeProgressBar"))
		{
			if (node is UpgradeProgressBar barParent)
			{
				progressBars.Add(barParent);
				
				StyleBoxFlat style = new()
				{
					BgColor = Utils.colorDictionary[barParent.progressBarType]
				};
				
				barParent.progressBar.AddThemeStyleboxOverride("fill", style);
			}
		}
		
		foreach (Node node in GetTree().GetNodesInGroup("UpgradesTabs"))
		{
			if (node is UpgradesTab tab)
			{
				upgradeTabs.Add(tab);
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
	
	
	
	public override void _Process(double delta)
	{
		timer += delta;

		if (timer >= 1.0)
		{
			CheckUpgradeStatus();
			
			timer -= 1.0;
		}
		
		// Update progress bar progress
		foreach (var bar in progressBars)
		{
			var progress = GetUpgradeProgress(bar.progressBarType);
			bar.progressBar.Value = Mathf.Lerp(bar.progressBar.Value, progress, 0.15f);
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
			var cost = CalculateUpgradeCost(nodeType);
			
			if (DataManager.saveData.ResearchPoints[nodeType] >= cost)
			{
				DataManager.saveData.ResearchPoints[nodeType] -= cost;
				DataManager.saveData.UnlockedUpgrades[lowestLevelBodyPart.Value] += 1;
				
				UpdateUpgradesTab(nodeType);
			}
		}
	}
	
	
	// Update dropdown upgrades tab UI with newly unlocked upgrades
	private void UpdateUpgradesTab(Utils.NodeType type)
	{
		foreach (var tab in upgradeTabs)
		{
			if(tab.areaType != type)
				continue;
			
			tab.InitializeUpgradesTab();
		}
		
		// Reconnect buttons
		foreach (Node node in GetTree().GetNodesInGroup("UpgradeTabButtons"))
		{
			if (node is ListUpgrade button)
			{
				button.BodyPartListItemClicked += AddBodyPart;
			}
		}
	}
	
	
	// Calculate the next upgrade cost from the current number unlocked
	// Cost is the same for all upgrades in a tier
	private double CalculateUpgradeCost(Utils.NodeType type)
	{
		List<Utils.BodyPart> parts = [];

		foreach (var part in Enum.GetValues<Utils.BodyPart>())
		{
			if(part == Utils.BodyPart.Torso)
				continue;
			
			if (Utils.bodyPartCategories[part] == type)
			{
				parts.Add(part);
			}
		}
		
		var partToUpgrade = Utils.GetLowestLevelUpgradeInCategory(type);
		int level = DataManager.saveData.UnlockedUpgrades[partToUpgrade.Value];
		int index = parts.IndexOf(partToUpgrade.Value);
		
		double cost = Math.Round(
			baseCost * 
			Math.Pow(nodeCostMultiplier, level) * 
			(1 + index * 0.2)
		);
		
		return cost;
	}
	
	
	// Calculate the current progress of the specified progress bar
	private double GetUpgradeProgress(Utils.NodeType type)
	{
		double current = DataManager.saveData.ResearchPoints[type];
		double cost = CalculateUpgradeCost(type);

		if (cost <= 0)
			return 0;
		
		return Math.Clamp(current / cost, 0.0, 1.0);
	}
	
	
	
	
}
