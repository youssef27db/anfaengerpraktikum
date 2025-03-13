using Godot;
using System;
using System.Collections;

public partial class StorageManager : Node {

    private const String PathSettings = "user://settings.txt";
    private String[] PathSave = {"user://save1.dat", "user://save2.dat", "user://save3.dat"};
    private int LastSaveId = -1;
    private int Saves = 3;
    public static StorageManager Instance { get; private set; }

    public override void _Ready(){
        Instance = this;
    }

    public void SaveAll(){

    }

    public int GetLastSaveId(){
        return LastSaveId;
    }

    public int GetSaves(){
        return Saves;
    }
}
