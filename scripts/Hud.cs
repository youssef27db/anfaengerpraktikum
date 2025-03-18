using Godot;
using System;

public partial class Hud : CanvasLayer {

    private AnimationPlayer AnimationPlayer;
    private CenterContainer Buttons;
    private bool Enabled;

    public override void _Ready() {
        AnimationPlayer = GetNode<AnimationPlayer>("PauseMenu/AnimationPlayer");
        Buttons = GetNode<CenterContainer>("PauseMenu/Buttons");
        AnimationPlayer.Play("RESET");
    }

    public override void _Process(double delta) {
        if(Input.IsActionJustPressed("escape")){
            TogglePause();
        }
    }

    private void TogglePause(){
        Enabled = !Enabled;
        GetTree().Paused = Enabled;
        if(Enabled){
            AnimationPlayer.Play("Pause");
            Buttons.Visible = true;
        } else {
            AnimationPlayer.PlayBackwards("Pause");
            Buttons.Visible = false;
        }
    }

    public void OnResumeButtonPressed(){
        TogglePause();
    }

    public void OnSaveButtonPressed(){
        StorageManager.Instance.SaveAll(StorageManager.Instance.GetLastSaveId());
    }

    public void OnSaveMenuButtonPressed(){
        StorageManager.Instance.SaveAll(StorageManager.Instance.GetLastSaveId());
        NavigationManager.Instance.GoToLevel("main_menu", null);
        PlayerStats.Instance.Reload();
        GetTree().Paused = false;
    }

    public void OnSaveQuitButtonPressed(){
        StorageManager.Instance.SaveAll(StorageManager.Instance.GetLastSaveId());
        GetTree().Quit();
    }

}
