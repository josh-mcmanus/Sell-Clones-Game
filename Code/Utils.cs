using Godot;
using System;
using System.Collections.Generic;

public partial class Utils : Node
{
	private const string TAG = "Utils";
	
	public static void Log(string TAG, string msg)
	{
		GD.Print($"[{TAG}] {msg}");
	}
	
	
	
	// For adding new body parts
	// File locations for body parts to load from
	public static readonly Dictionary<BodyPartEnum, string> bodyPartsDictionary = new()
	{
		{BodyPartEnum.LArm,	"res://Scenes/Body Parts/l_arm.tscn"},
		
		// remaining unfinished
		// {BodyPartEnum.LForearm,	"res://Scenes/Body Parts/"}, 
		// {BodyPartEnum.LHand,	"res://Scenes/Body Parts/"},
		// {BodyPartEnum.RArm,		"res://Scenes/Body Parts/"},
		// {BodyPartEnum.RForearm,	"res://Scenes/Body Parts/"},
		// {BodyPartEnum.RHand,	"res://Scenes/Body Parts/"},
		// {BodyPartEnum.LLeg,		"res://Scenes/Body Parts/"},
		// {BodyPartEnum.LCalf,	"res://Scenes/Body Parts/"},
		// {BodyPartEnum.LFoot,	"res://Scenes/Body Parts/"},
		// {BodyPartEnum.RLeg,		"res://Scenes/Body Parts/"},
		// {BodyPartEnum.RCalf,	"res://Scenes/Body Parts/"},
		// {BodyPartEnum.RFoot,	"res://Scenes/Body Parts/"},
		// {BodyPartEnum.Torso,	"res://Scenes/Body Parts/"},
		// {BodyPartEnum.Heart,	"res://Scenes/Body Parts/"},
		// {BodyPartEnum.Lung,		"res://Scenes/Body Parts/"},
		// {BodyPartEnum.Kidney,	"res://Scenes/Body Parts/"},
		// {BodyPartEnum.Liver,	"res://Scenes/Body Parts/"}
	};
	
	
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
	
}
