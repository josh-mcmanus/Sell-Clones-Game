using Godot;
using System;
using System.Threading.Tasks;

public partial class ResearchNode : Node2D
{
	[Export]
	Timer timer;
	
	[Export]
	ColorRect complete;
	
	[Signal]
	public delegate void ResearchUnitCompleteEventHandler();
	
	
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		timer.Timeout += OnTimerTimout;
	}
	
	
	private async void OnTimerTimout()
	{
		// Complete 1 research unit
		EmitSignal(SignalName.ResearchUnitComplete);
		
		// Finished research, play complete animation
		complete.Visible = true;
		
		// Wait 0.2 seconds (200 milliseconds)
		await Task.Delay(200);
		
		// Set back
		complete.Visible = false;
	}
	
	
}
