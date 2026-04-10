using Godot;
using System;

public partial class HoverInfoLabel : Label
{
	[Export]
	public LabelType labelType;
	
	public enum LabelType
	{
		InfoLabel,
		WarningLabel
	}
}
