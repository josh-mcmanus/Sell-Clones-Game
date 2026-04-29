using Godot;
using System;

public partial class UpgradePartHoverLabel : Label
{
	[Export]
	private Timer timer;
	
	[Signal]
	public delegate void HoverTimerTimeoutEventHandler(UpgradePartHoverLabel label);
	
	
	public override void _Ready()
	{
		timer.Timeout += OnTimerTimout;
	}
	
	
	
	private void OnTimerTimout()
	{
		EmitSignal(SignalName.HoverTimerTimeout, this);
	}
}
