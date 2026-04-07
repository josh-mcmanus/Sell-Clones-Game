using Godot;
using System;
using System.Text.Json;

public partial class DataManager : Node
{
	private const string TAG = "DataManager";
	
	public static DataManager Instance {get; private set;}
	public static PlayerData saveData = new PlayerData();
	
	public static string gameVersion = "0.3.0 Date: 07/04/26";
	public static string saveDataFileLocation = "user://save_data.json";
	
	private static readonly JsonSerializerOptions jsonOptions = new()
	{ 
		WriteIndented = true, 
	};
	
	
	
	public override void _Ready()
	{
		Instance = this;
		
		// Add timer for autosave
		Timer timer = new()
		{
			WaitTime = 60,
			Autostart = true,
			OneShot = false
		};
		
		timer.Timeout += AutoSave;
		
		AddChild(timer);
		
		Load();		
	}
	
	
	// Autosave every minute
	private void AutoSave()
	{
		Utils.Log(TAG, "Autosave");
		Save();
	}
	
	
	
	private static void Save()
	{
		using var file = FileAccess.Open(saveDataFileLocation, FileAccess.ModeFlags.Write);
		var jsonString = string.Empty;
		
		jsonString = JsonSerializer.Serialize(saveData, jsonOptions);
		
		file.StoreString(jsonString);
		
		Utils.Log(TAG, "Save complete");
	}
	
	
	
	public static void Load()
	{
		if (!DoesFileExist())
		{
			Utils.Error(TAG, "Save data file does not exist, creating new file");
			
			saveData = new PlayerData();
			Save();
		}
		
		using var file = FileAccess.Open(saveDataFileLocation, FileAccess.ModeFlags.Read);
		var jsonString = file.GetAsText();
		
		saveData = JsonSerializer.Deserialize<PlayerData>(jsonString);
	}
	
	
	public static bool DoesFileExist()
	{
		if (FileAccess.FileExists(saveDataFileLocation))
		{
			return true;
		}
		
		return false;
	}
	
}
