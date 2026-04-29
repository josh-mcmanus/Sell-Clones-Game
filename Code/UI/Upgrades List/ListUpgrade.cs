using Godot;
using System;

public partial class ListUpgrade : Button
{
	[Signal]
	public delegate void UpgradePartListItemClickedEventHandler(ListUpgrade item);
	
	[Export]
	public TextureRect icon;
	
	[Export]
	public Label label;
	
	public Utils.UpgradePart part;
	public Utils.UpgradeTier tier;
	

	public override void _Pressed()
	{
		EmitSignal(SignalName.UpgradePartListItemClicked, this);
	}

	
	
}
