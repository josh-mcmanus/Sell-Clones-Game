using Godot;
using System;
using System.Linq;

public partial class UpgradesTab : Control
{
	[Export]
	private Utils.NodeType areaType;
	
	[Export]
	public VBoxContainer listArea;
	
	
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		// check unlocked upgrades
		
		// populate upgrades tab with items
		
		// loop through unlocks
		
		
		// Add the upgrade to
			// VBox container
		
		// Add listUpgrade to
			// Foldable Container inside vbox container above
		
		
		// Set name as upgrade 1, upgrade 2, etc.
		// then move to next and add new vbox container
		
		
		
		// Populate list based on unlocked upgrades
		
		foreach(var unlockedUpgrade in DataManager.saveData.UnlockedUpgrades)
		{
			var upgradeType = Utils.bodyPartCategories[unlockedUpgrade.Key];
			
			if (upgradeType != areaType)
				return;
			
			if (unlockedUpgrade.Value > 0)
			{
				// Initialize new foldable body part list
				GD.Print(unlockedUpgrade.Key);
				
				var foldableListScene = GD.Load<PackedScene>("res://Scenes/foldable_body_part_list.tscn");
				var foldableListInstance = foldableListScene.Instantiate();
				
				var foldableList = (FoldableBodyPartList) foldableListInstance;
				foldableList.Title = $"{unlockedUpgrade.Key}";
				
				
				for (int i = 1; i < unlockedUpgrade.Value + 1; i++)
				{
					// Add list upgrade
					var listUpgradeScene = GD.Load<PackedScene>("res://Scenes/list_upgrade.tscn");
					var listUpgradeInstance = listUpgradeScene.Instantiate();
					
					var listUpgrade = (ListUpgrade) listUpgradeInstance;
					
					
					// TODO: Add descritption on hover, flavour text
					// TODO: Set icon
					// TODO: Change lvl to flavour text. e.g. Syntetic part, cybernetic part. instead of lvl 1,2...
					listUpgrade.label.Text = $"Lvl {i} {unlockedUpgrade.Key}";
					
					listUpgrade.bodyPart = unlockedUpgrade.Key;
					listUpgrade.level = (Utils.UpgradeTier) i;
					listUpgrade.AddToGroup("UpgradeTabButtons");
					
					// listUpgrade.BodyPartListItemClicked += AddBodyPartToScene;
					
					foldableList.listContainer.AddChild(listUpgradeInstance);
				}
				
				listArea.AddChild(foldableList);
			}
			
			
			// if (unlockedUpgrade.Key == areaType)
			// {
				
			// }
		}
		
	}
	
	
	// Button Clicked
	// Add to corresponding body part to scene
	// private void AddBodyPartToScene(ListUpgrade item)
	// {
	// 	GD.Print(item.label.Text);
		
	// 	// CloneManager.AddBodyPartToScene();
		
	// }
	
	
	

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
