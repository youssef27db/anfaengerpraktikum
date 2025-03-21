using System;
using Godot;

/**
 * @brief Klasse für die Spielerstats
 */
public partial class PlayerStats : Node
{

	public static PlayerStats Instance { get; private set; }
	
	private String RespawnLevelTag = "intro";
    private String CurrentLevelTag = "intro";
	private Vector2 SpawnPoint;
    private Vector2 Position = new Vector2(-540, 160);
	private int SinAmount;
	private float MaxHealthPoints = 100f;
    private float CurrentHealth;
	private float MaxStamina = 100f; 
    private float CurrentStamina;
	private int BVHealAmount = 25;
    private int BVMaxUses = 5;
    private int BVCurrentUses;


	/** 
     * @brief Initialisierung der Referenzen.
     */
    public override void _Ready(){
        CurrentHealth = MaxHealthPoints;
		CurrentStamina = MaxStamina;
		BVCurrentUses = BVMaxUses;
		Instance = this;
    }

    /**
    * @brief Getter für RespawnLevelTag.
    * @return String RespawnLevelTag
    */
    public String GetRespawnLevelTag() {
		return RespawnLevelTag;
	}

	/**
    * @brief Setter für RespawnLevelTag.
    * @param RespawnLevelTag Neuer Tag
    */
	public void SetRespawnLevelTag(String levelTag) {
		RespawnLevelTag = levelTag;
	}

    /**
    * @brief Getter für CurrentLevelTag.
    * @return String CurrentLevelTag
    */
    public String GetCurrentLevelTag() {
		return CurrentLevelTag;
	}

	/**
    * @brief Setter für CurrentLevelTag.
    * @param CurrentLevelTag Neuer Tag
    */
	public void SetCurrentLevelTag(String levelTag) {
		CurrentLevelTag = levelTag;
	}

    /** 
     * @brief Setzt den SpawnPoint des Spielers.
     * @param Der SpawnPoint des Spielers.
     */
    public void SetSpawnPoint(Vector2 spawnPoint) {
        SpawnPoint = spawnPoint;
    }

    /**
    * @brief Getter für den SpawnPoint
    * @return Der SpawnPoint des Spielers
    */
    public Vector2 GetSpawnPoint(){
        return SpawnPoint;
    }

    /** 
     * @brief Setzt die Position des Spielers.
     * @param Position des Spielers.
     */
    public void SetPosition(Vector2 position) {
        Position = position;
    }

    /**
    * @brief Getter für die Position
    * @return Position des Spielers
    */
    public Vector2 GetPosition(){
        return Position;
    }


	/**
    * @brief Getter für SinAmount.
    * @return int Sins
    */
	public int GetSinAmount(){
		return SinAmount;
	}

	/** 
    * @brief Setzt den SinAmount des Spielers.
    * @param Value Der neue Wert für den SinAmount.
    */
	public void SetSinAmount(int Value) {
        // SinAmount muss immer >= 0 sein
        SinAmount = Mathf.Max(Value, 0);
    }

	/** 
    * @brief Gibt die maximalen Lebenspunkte des Spielers zurück.
    * @return Die maximalen Lebenspunkte.
    */
    public float GetMaxHealthPoints(){
        return MaxHealthPoints;
    }

	/** 
    * @brief Setzt die maximalen Lebenspunkte des Spielers.
    * @param maxHealthPoints Die neuen maximalen Lebenspunkte (muss positiv sein).
    */
	public void SetMaxHealthPoints(float maxHealthPoints){
        // MaxHealthPoints muss immer positiv sein
        MaxHealthPoints = Mathf.Max(maxHealthPoints, 1); // Verhindert, dass MaxHealthPoints <= 0 wird
    }

	/** 
    * @brief Gibt die aktuellen Lebenspunkte des Spielers zurück.
    * @return Die aktuellen Lebenspunkte.
    */
    public float GetCurrentHealth(){
        return CurrentHealth;
    }

	/** 
    * @brief Setzt die aktuellen Lebenspunkte des Spielers.
    * @param Health Neue Lebenspunkte, die gesetzt werden sollen.
    */
    public void SetCurrentHealth(float Health){
        // CurrentHealth darf MaxHealthPoints nicht überschreiten.
        CurrentHealth = Mathf.Min(Health, MaxHealthPoints);
    }

    /** 
    * @brief Setzt die maximale Stamina des Spielers.
    * @param Value Den neuen Wert für die maximale Stamina (muss positiv sein).
    */
    public void SetMaxStamina(float Value) {
        // MaxStamina muss immer positiv sein
        MaxStamina = Mathf.Max(Value, 1);
    }

    /** 
    * @brief Gibt die maximale Stamina des Spielers zurück.
    * @return Die maximale Stamina.
    */
    public float GetMaxStamina() {
        return MaxStamina;
    }

	/** 
    * @brief Setzt die Stamina des Spielers.
    * @param Value Den neuen Wert für Stamina (muss im Bereich zwischen 0 und MaxStamina liegen).
    */
    public void SetStamina(float Value) {
        // Stellt sicher, dass die CurrentStamina im gültigen Bereich bleibt (zwischen 0 und MaxStamina)
        CurrentStamina = Mathf.Clamp(Value, 0, MaxStamina);
    }

    /** 
    * @brief Gibt die aktuelle Stamina des Spielers zurück.
    * @return Die aktuelle Stamina.
    */
    public float GetStamina() {
        return CurrentStamina;
    }

	/**
    * @brief Setzt den HealAmount eines Bloodvials.
	* @param Value Den neuen Wert für den HealAmount.
    */
    public void SetBVHealAmount(int Value){
        BVHealAmount = Math.Max(0, Value);
    }

	/** 
    * @brief Gibt den aktuellen HealAmount eines Bloodvials zurück.
    * @return Der aktuelle HealAmount.
    */
    public int GetBVHealAmount() {
        return BVHealAmount;
    }

	/**
    * @brief Setzt die MaxUses der Bloodvials.
	* @param Value Die MaxUses der Bloodvials.
    */
    public void SetBVMaxUses(int Value){
        BVMaxUses = Math.Max(0, Value);
    }

	/** 
    * @brief Gibt die MaxUses der Bloodvials zurück.
    * @return Die aktuellen MaxUses.
    */
    public int GetBVMaxUses() {
        return BVMaxUses;
    }

	/**
    * @brief Setzt die CurrentUses der Bloodvials.
	* @param Value Die CurrentUses der Bloodvials.
    */
    public void SetBVCurrentUses(int Value){
        BVCurrentUses = Math.Max(0, Value);
    }

	/** 
    * @brief Gibt die CurrentUses der Bloodvials zurück.
    * @return Die aktuellen CurrentUses.
    */
    public int GetBVCurrentUses() {
        return BVCurrentUses;
    }

    /**
    * @brief Setzt die Attribute zurück.
    */
    public void Reload(){
        Instance = new PlayerStats();
        Instance._Ready();
    }

}
