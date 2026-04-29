using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public partial class CloneManager : UpgradeManager
{
	public override void _Process(double delta)
	{
		base._Process(delta);
		HandlePartFlipping();
	}
}
