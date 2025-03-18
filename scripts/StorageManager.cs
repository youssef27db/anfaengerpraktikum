using Godot;
using System;
using System.Collections;

public partial class StorageManager : Node {

    private const String PathSettings = "user://settings.txt";
    private String[] PathSave = {"user://save1.dat", "user://save2.dat", "user://save3.dat"};
    private int LastSaveId = -1;
    private int Saves = 0;
    public static StorageManager Instance { get; private set; }

    public override void _Ready(){
        LoadSettings();
        Instance = this;
    }

    public void LoadSettings(){
        if(!FileAccess.FileExists(PathSettings)){
            return;
        }
        FileAccess File = FileAccess.Open(PathSettings, FileAccess.ModeFlags.Read);
        Saves = (int) File.GetVar();
        LastSaveId = (int) File.GetVar();

        File.Close();
    }

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

    public void SaveAll(int id){
        SaveGameFile(id);
        SaveSettings();
    }

    public void SaveSettings(){
        FileAccess File = FileAccess.Open(PathSettings, FileAccess.ModeFlags.Write);
        File.StoreVar(Saves);
        File.StoreVar(LastSaveId);

        File.Close();
    }

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

    public void SetLastSaveId(int id){
        LastSaveId = id;
    }

    public int GetLastSaveId(){
        return LastSaveId;
    }

    public void SetSaves(int Saves){
        this.Saves = Saves;
    }

    public int GetSaves(){
        return Saves;
    }
}
