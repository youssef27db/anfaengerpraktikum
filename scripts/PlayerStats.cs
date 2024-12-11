using Godot;

/**
 * Klasse für die Spielerstats
 */

public partial class PlayerStats : Node
{
	private string RespawnLevelTag = "intro";

	// getter for RespawnLevelTag
	public string GetRespawnLevelTag()
	{
		return RespawnLevelTag;
	}

	// setter for RespawnLevelTag
	public void SetRespawnLevelTag(string levelTag)
	{
		RespawnLevelTag = levelTag;
	}
}
