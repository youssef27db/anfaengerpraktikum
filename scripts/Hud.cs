using Godot;
using System;


/**
* @brief Klasse für das PauseMenu
*/
public partial class Hud : CanvasLayer {

    private AnimationPlayer AnimationPlayer;
    private CenterContainer Buttons;
    private bool Enabled;


    /** 
    * @brief Initialisierung der Referenzen.
    * Findet die relevanten Knoten in der Szene und weist sie zu.
    */
    public override void _Ready() {
        AnimationPlayer = GetNode<AnimationPlayer>("PauseMenu/AnimationPlayer");
        Buttons = GetNode<CenterContainer>("PauseMenu/Buttons");
        AnimationPlayer.Play("RESET");
    }

    /** 
    * @brief Methode wird in jedem Frame ausgeführt.
    * @param DeltaTime Zeit seit dem letzten Frame.
    */
    public override void _Process(double DeltaTime) {
        if(Input.IsActionJustPressed("escape")){
            TogglePause();
        }
    }

    /** 
    * @brief Toggled die Pause Funktion.
    */
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

    /** 
    * @brief Signal für den Resume-Button.
    */
    public void OnResumeButtonPressed(){
        TogglePause();
    }

    /** 
    * @brief Signal für den Save-Button.
    */
    public void OnSaveButtonPressed(){
        StorageManager.Instance.SaveAll(StorageManager.Instance.GetLastSaveId());
    }

    /** 
    * @brief Signal für den SaveAndReturnToMenu-Button.
    */
    public void OnSaveMenuButtonPressed(){
        StorageManager.Instance.SaveAll(StorageManager.Instance.GetLastSaveId());
        NavigationManager.Instance.GoToLevel("main_menu", null);
        PlayerStats.Instance.Reload();
        GetTree().Paused = false;
    }

    /** 
    * @brief Signal für den SaveAndQuit-Button.
    */
    public void OnSaveQuitButtonPressed(){
        StorageManager.Instance.SaveAll(StorageManager.Instance.GetLastSaveId());
        GetTree().Quit();
    }

}
