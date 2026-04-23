using Godot;
using System;

public partial class ListUpgrade : Button
{
	[Signal]
	public delegate void BodyPartListItemClickedEventHandler(ListUpgrade item);
	
	[Export]
	public TextureRect icon;
	
	[Export]
	public Label label;
	
	public Utils.BodyPart bodyPart;
	public Utils.UpgradeTier level;
	

	public override void _Pressed()
	{
		EmitSignal(SignalName.BodyPartListItemClicked, this);
	}

	
	
}
