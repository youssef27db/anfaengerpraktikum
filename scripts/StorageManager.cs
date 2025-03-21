using Godot;
using System;
using System.Collections;

/**
* @brief Klasse für das Speichern und Laden von Daten.
*/
public partial class StorageManager : Node {

    private const String PathSettings = "user://settings.txt";
    private String[] PathSave = {"user://save1.dat", "user://save2.dat", "user://save3.dat"};
    private int LastSaveId = -1;
    private int Saves = 0;
    public static StorageManager Instance { get; private set; }


    /** 
     * @brief Initialisierung der Instanz und erstes laden der Einstellungen.
     */
    public override void _Ready(){
        LoadSettings();
        Instance = this;
    }

    /** 
    * @brief Laden der Einstellungen.
    */
    public void LoadSettings(){
        if(!FileAccess.FileExists(PathSettings)){
            return;
        }
        FileAccess File = FileAccess.Open(PathSettings, FileAccess.ModeFlags.Read);
        Saves = (int) File.GetVar();
        LastSaveId = (int) File.GetVar();

        File.Close();
    }

    /** 
    * @brief Laden eines Spielstandes.
    * @param Id des Spielstandes.
    */
    public void LoadGameFile(int id){
        if(!FileAccess.FileExists(PathSave[id])){
            return;
        }
        FileAccess File = FileAccess.Open(PathSave[id], FileAccess.ModeFlags.Read);
        PlayerStats.Instance.SetRespawnLevelTag((String) File.GetVar());
        PlayerStats.Instance.SetCurrentLevelTag((String) File.GetVar());
        PlayerStats.Instance.SetSpawnPoint((Vector2) File.GetVar());
        PlayerStats.Instance.SetPosition((Vector2) File.GetVar());
        PlayerStats.Instance.SetSinAmount((int) File.GetVar());
        PlayerStats.Instance.SetMaxHealthPoints((float) File.GetVar());
        PlayerStats.Instance.SetCurrentHealth((float) File.GetVar());
        PlayerStats.Instance.SetMaxStamina((float) File.GetVar());
        PlayerStats.Instance.SetStamina((float) File.GetVar());
        PlayerStats.Instance.SetBVHealAmount((int) File.GetVar());
        PlayerStats.Instance.SetBVMaxUses((int) File.GetVar());
        PlayerStats.Instance.SetBVCurrentUses((int) File.GetVar());

        File.Close();
    }

    /** 
    * @brief Speichern der Einstellungen und eines Spielstandes.
    * @param Id des Spielstandes.
    */
    public void SaveAll(int id){
        SaveGameFile(id);
        SaveSettings();
    }

    /** 
    * @brief Speichern der Einstellungen.
    */
    public void SaveSettings(){
        FileAccess File = FileAccess.Open(PathSettings, FileAccess.ModeFlags.Write);
        File.StoreVar(Saves);
        File.StoreVar(LastSaveId);

        File.Close();
    }

    /** 
    * @brief Speichern eines Spielstandes.
    * @param Id des Spielstandes.
    */
    public void SaveGameFile(int id){
        FileAccess File = FileAccess.Open(PathSave[id], FileAccess.ModeFlags.Write);
        File.StoreVar(PlayerStats.Instance.GetRespawnLevelTag());
        File.StoreVar(PlayerStats.Instance.GetCurrentLevelTag());
        File.StoreVar(PlayerStats.Instance.GetSpawnPoint());
        File.StoreVar(PlayerStats.Instance.GetPosition());
        File.StoreVar(PlayerStats.Instance.GetSinAmount());
        File.StoreVar(PlayerStats.Instance.GetMaxHealthPoints());
        File.StoreVar(PlayerStats.Instance.GetCurrentHealth());
        File.StoreVar(PlayerStats.Instance.GetMaxStamina());
        File.StoreVar(PlayerStats.Instance.GetStamina());
        File.StoreVar(PlayerStats.Instance.GetBVHealAmount());
        File.StoreVar(PlayerStats.Instance.GetBVMaxUses());
        File.StoreVar(PlayerStats.Instance.GetBVCurrentUses());

        File.Close();
    }

    /**
    * @brief Setter für LastSaveId.
    * @param int LastSaveId
    */
    public void SetLastSaveId(int id){
        LastSaveId = id;
    }

    /**
    * @brief Getter für LastSaveId.
    * @return int LastSaveId
    */
    public int GetLastSaveId(){
        return LastSaveId;
    }

    /**
    * @brief Setter für Saves.
    * @param int Saves
    */
    public void SetSaves(int Saves){
        this.Saves = Saves;
    }

    /**
    * @brief Getter für Saves.
    * @return int Saves
    */
    public int GetSaves(){
        return Saves;
    }
}
