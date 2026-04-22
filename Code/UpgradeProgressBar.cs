using Godot;
using System;

public partial class UpgradeProgressBar : Control
{
	private Tween tween;
	
	[Export]
	public Utils.NodeType progressBarType;
	
	[Export]
	public ProgressBar progressBar;
	
	
	public override void _Ready()
	{
		AddToGroup("UpgradeProgressBar");
	}
}
