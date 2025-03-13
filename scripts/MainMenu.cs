using Godot;
using System;

public partial class MainMenu : Node2D {

    private int MenuState = 0;

    private VBoxContainer Navigation;
    private MarginContainer SavesContainer;
    private Button ContinueButton;
    private Label InfoLabel;
    private Label[] SaveLabel = new Label[3];
    private Button[] SelectButton = new Button[3];
    private Button[] DeleteButton = new Button[3];

    public override void _Ready() {
        Navigation = GetNode<VBoxContainer>("Control/Navigation");
        SavesContainer = GetNode<MarginContainer>("Control/Saves");
        ContinueButton = GetNode<Button>("Control/Navigation/ContinueButton");
        InfoLabel = GetNode<Label>("Control/Saves/VBoxContainer/Info");

        SaveLabel[0] = GetNode<Label>("Control/Saves/VBoxContainer/HBoxContainer/Save1/Label");
        SelectButton[0] = GetNode<Button>("Control/Saves/VBoxContainer/HBoxContainer/Save1/Select");
        DeleteButton[0] = GetNode<Button>("Control/Saves/VBoxContainer/HBoxContainer/Save1/Delete");
        SaveLabel[1] = GetNode<Label>("Control/Saves/VBoxContainer/HBoxContainer/Save2/Label");
        SelectButton[1] = GetNode<Button>("Control/Saves/VBoxContainer/HBoxContainer/Save2/Select");
        DeleteButton[1] = GetNode<Button>("Control/Saves/VBoxContainer/HBoxContainer/Save2/Delete");
        SaveLabel[2] = GetNode<Label>("Control/Saves/VBoxContainer/HBoxContainer/Save3/Label");
        SelectButton[2] = GetNode<Button>("Control/Saves/VBoxContainer/HBoxContainer/Save3/Select");
        DeleteButton[2] = GetNode<Button>("Control/Saves/VBoxContainer/HBoxContainer/Save3/Delete");

        if(StorageManager.Instance.GetLastSaveId() > -1){
            ContinueButton.Visible = true;
        }
    }

    private void Change(){
        if(MenuState == 0){
            SavesContainer.Visible = false;
            Navigation.Visible = true;
        } else {
            Navigation.Visible = false;
            SavesContainer.Visible = true;

            int Saves = StorageManager.Instance.GetSaves();

            if(MenuState == 1){
                InfoLabel.Text = "Select empty save to start a new Game";
                for(int i = 0; i < 3; i++){
                    if((Saves & (int) Math.Pow(2, i)) == (int) Math.Pow(2, i)){
                        SaveLabel[i].Text = "Save " + (i+1);
                        SelectButton[i].Disabled = true;
                        DeleteButton[i].Disabled = false;
                    } else {
                        SaveLabel[i].Text = "Save " + (i+1) + "\nEmpty";
                        SelectButton[i].Disabled = false;
                        DeleteButton[i].Disabled = true;
                    }
                }
            } else {
                InfoLabel.Text = "Select save to load Game";
                for(int i = 0; i < 3; i++){
                    if((Saves & (int) Math.Pow(2, i)) == (int) Math.Pow(2, i)){
                        SaveLabel[i].Text = "Save " + (i+1);
                        SelectButton[i].Disabled = false;
                        DeleteButton[i].Disabled = false;
                    } else {
                        SaveLabel[i].Text = "Save " + (i+1) + "\nEmpty";
                        SelectButton[i].Disabled = true;
                        DeleteButton[i].Disabled = true;
                    }
                }
            }
        }
    }

    public void OnContinueButtonPressed(){

    }

    public void OnQuitButtonPressed(){
        StorageManager.Instance.SaveAll();
        GetTree().Quit();
    }

    public void OnNewGameButtonPressed(){
        MenuState = 1;
        Change();
    }

    public void OnLoadGameButtonPressed(){
        MenuState = 2;
        Change();
    }

    public void OnBackButtonPressed(){
        MenuState = 0;
        Change();
    }
}
