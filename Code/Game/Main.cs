using Godot;
using System;
using System.Collections.Generic;
using System.Linq;


// Class of the main game scene

public partial class Main : Control
{
	private const string TAG = "Main";
	
	[Export]
	private Godot.Collections.Dictionary<UpgradesTab, UpgradeManager> upgradeManagers {get; set;} = new();
	
	[Export]
	private CanvasLayer canvasLayer;
	
	private double timer = 0;
	private double baseCost = 50;
	private double nodeCostMultiplier = 1.8f;
	
	private List<UpgradeProgressBar> progressBars = [];
	private List<UpgradesTab> upgradeTabs = [];
	
	
	
	public override void _Ready()
	{
		foreach (Node node in GetTree().GetNodesInGroup("UpgradeTabButtons"))
		{
			if (node is ListUpgrade button)
			{
				button.UpgradePartListItemClicked += AddUpgradePart;
			}
		}
		
		foreach (Node node in GetTree().GetNodesInGroup("UpgradeProgressBar"))
		{
			if (node is UpgradeProgressBar barParent)
			{
				progressBars.Add(barParent);
				
				StyleBoxFlat style = new()
				{
					BgColor = Utils.nodeColors[barParent.progressBarType]
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
	private void AddUpgradePart(ListUpgrade upgradeToAdd)
	{
		Utils.Log(TAG, "Part added from list");
		
		foreach (var manager in upgradeManagers)
		{
			Utils.Log(TAG, $"Key: {manager.Key}, Value: {manager.Value}");
		}
		
		var partDirectory = Utils.GetUpgradePartDirectory(upgradeToAdd.part);
		
		GenericPart part;
		
		// Add upgrade only to matching area type
		foreach (var upgradeManager in upgradeManagers)
		{
			if (Utils.UpgradePartCategories[upgradeToAdd.part] != upgradeManager.Key.type)
				continue;
			
			part = upgradeManager.Value.AddPartToScene(
				partDirectory, 
				Utils.Side.None, 
				upgradeToAdd.tier
			);
			
			part.part = upgradeToAdd.part;
		}
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
			
			var lowestLevelUpgradePart = Utils.GetLowestLevelUpgradeInCategory(nodeType);
			
			if(lowestLevelUpgradePart == null)
				continue;
			
			// Enough points
			var cost = CalculateUpgradeCost(nodeType);
			
			if (DataManager.saveData.ResearchPoints[nodeType] >= cost)
			{
				DataManager.saveData.ResearchPoints[nodeType] -= cost;
				DataManager.saveData.UnlockedUpgrades[lowestLevelUpgradePart.Value] += 1;
				
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
				button.UpgradePartListItemClicked -= AddUpgradePart;
				button.UpgradePartListItemClicked += AddUpgradePart;
			}
		}
	}
	
	
	// Calculate the next upgrade cost from the current number unlocked
	// Cost is the same for all upgrades in a tier
	private double CalculateUpgradeCost(Utils.NodeType type)
	{
		List<Utils.UpgradePart> parts = [];

		foreach (var part in Enum.GetValues<Utils.UpgradePart>())
		{
			if(part == Utils.UpgradePart.Torso)
				continue;
			
			if (Utils.UpgradePartCategories[part] == type)
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
