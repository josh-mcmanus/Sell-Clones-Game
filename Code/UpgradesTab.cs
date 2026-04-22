using Godot;
using System;
using System.Linq;

public partial class UpgradesTab : Control
{
	[Export]
	public Utils.NodeType areaType;
	
	[Export]
	public VBoxContainer listArea;
	
	private PackedScene foldableListScene;
	private PackedScene listUpgradeScene;
	
	
	public override void _Ready()
	{
		foldableListScene = GD.Load<PackedScene>("res://Scenes/foldable_body_part_list.tscn");
		listUpgradeScene = GD.Load<PackedScene>("res://Scenes/list_upgrade.tscn");
		
		AddToGroup("UpgradesTabs");
		
		InitializeUpgradesTab();
	}
	
	
	// Initializes and resets the upgrades tab list
	// For startup and when a new upgrade is unlocked
	public void InitializeUpgradesTab()
	{
		// Restore to empty
		foreach (Node child in listArea.GetChildren())
		{
			child.QueueFree();
		}
		
		// Setup list
		foreach(var unlockedUpgrade in DataManager.saveData.UnlockedUpgrades)
		{
			var upgradeType = Utils.bodyPartCategories[unlockedUpgrade.Key];
			
			if (upgradeType != areaType)
				continue;
			
			if (unlockedUpgrade.Value > 0)
			{
				// Initialize new foldable body part list
				// var foldableListScene = GD.Load<PackedScene>("res://Scenes/foldable_body_part_list.tscn");
				var foldableListInstance = foldableListScene.Instantiate();
				
				var foldableList = (FoldableBodyPartList) foldableListInstance;
				foldableList.Title = $"{unlockedUpgrade.Key}";
				
				
				for (int i = 1; i < unlockedUpgrade.Value + 1; i++)
				{
					// Add list upgrade
					// var listUpgradeScene = GD.Load<PackedScene>("res://Scenes/list_upgrade.tscn");
					var listUpgradeInstance = listUpgradeScene.Instantiate();
					
					var listUpgrade = (ListUpgrade) listUpgradeInstance;
					
					
					// TODO: Add descritption on hover, flavour text
					// TODO: Set icon
					// TODO: Change lvl to flavour text. e.g. Syntetic part, cybernetic part. instead of lvl 1,2...
					listUpgrade.label.Text = $"Lvl {i} {unlockedUpgrade.Key}";
					
					listUpgrade.bodyPart = unlockedUpgrade.Key;
					listUpgrade.level = (Utils.UpgradeTier) i;
					listUpgrade.AddToGroup("UpgradeTabButtons");
					
					foldableList.listContainer.AddChild(listUpgradeInstance);
				}
				
				listArea.AddChild(foldableList);
			}
		}
	}
	
	
}
