using Godot;
using System;

public partial class AttachedPartData
{
	// Save data for individual attached part
	// Associates the attachment site "Part" and "Side" to an attached part "Level"
	
	// Only exists when there is an attachment, meaning only levels 1-6 can exist in this context
	
	
	public Utils.UpgradePart Part {get; set;}
	public Utils.Side Side {get; set;}
	public Utils.UpgradeTier Tier {get; set;}
	
	public AttachedPartData(){}
	
	public AttachedPartData(Utils.UpgradePart part, Utils.Side side, Utils.UpgradeTier tier)
	{
		Part = part;
		Side = side;
		Tier = tier;
	}
	
	
	
}
