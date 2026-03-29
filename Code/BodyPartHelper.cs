using Godot;
using System;
using System.Collections.Generic;

public partial class BodyPartHelper : Node
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
	
	public static BodyPartEnum GetBodyPartEnumFromString(string input)
	{
		if (!string.IsNullOrEmpty(input))
		{
			throw new ArgumentException("Input cannot be null or empty");
		}
		
		if(Enum.TryParse(input, out BodyPartEnum result))
		{
			return result;
		}
		
		throw new ArgumentException($"Invalid input string: {input}");
	}
	
	
	public enum BodyPartEnum
	{
		LArm,
		LForearm,
		LHand,
		
		RArm,
		RForearm,
		RHand,
		
		
		LLeg,
		LCalf,
		LFoot,
		
		RLeg,
		RCalf,
		RFoot,
		
		
		Torso,
		Heart,
		Lung,
		Kidney,
		Liver
	}
	
	
	// Dictionary<BodyPartEnum, Vector2> bodyPartArea
	
	
}
