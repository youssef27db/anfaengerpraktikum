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
    private ConfirmationDialog DeleteConfirmation;
    private int SaveToDelete = 0;

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

        DeleteConfirmation = GetNode<ConfirmationDialog>("DeleteConfirmation");

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
        StorageManager.Instance.LoadGameFile(StorageManager.Instance.GetLastSaveId());
        NavigationManager.Instance.GoToLevel(PlayerStats.Instance.GetCurrentLevelTag(), null);
    }

    public void OnQuitButtonPressed(){
        StorageManager.Instance.SaveSettings();
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

    public void OnSave1SelectPressed(){
        if(MenuState == 2){
            StorageManager.Instance.LoadGameFile(0);
        }
        NavigationManager.Instance.GoToLevel(PlayerStats.Instance.GetCurrentLevelTag(), null);
        StorageManager.Instance.SetSaves(StorageManager.Instance.GetSaves() | 1);
        StorageManager.Instance.SetLastSaveId(0);
    }

    public void OnSave1DeletePressed(){
        SaveToDelete = 1;
        DeleteConfirmation.SetText("Are you sure you want to DELETE Save " + SaveToDelete + "?");
        DeleteConfirmation.Show();
    }

    public void OnSave2SelectPressed(){
        if(MenuState == 2){
            StorageManager.Instance.LoadGameFile(1);
        }
        NavigationManager.Instance.GoToLevel(PlayerStats.Instance.GetCurrentLevelTag(), null);
        StorageManager.Instance.SetSaves(StorageManager.Instance.GetSaves() | 2);
        StorageManager.Instance.SetLastSaveId(1);
    }

    public void OnSave2DeletePressed(){
        SaveToDelete = 2;
        DeleteConfirmation.SetText("Are you sure you want to DELETE Save " + SaveToDelete + "?");
        DeleteConfirmation.Show();
    }

    public void OnSave3SelectPressed(){
        if(MenuState == 2){
            StorageManager.Instance.LoadGameFile(2);
        }
        NavigationManager.Instance.GoToLevel(PlayerStats.Instance.GetCurrentLevelTag(), null);
        StorageManager.Instance.SetSaves(StorageManager.Instance.GetSaves() | 4);
        StorageManager.Instance.SetLastSaveId(2);
    }

    public void OnSave3DeletePressed(){
        SaveToDelete = 3;
        DeleteConfirmation.SetText("Are you sure you want to DELETE Save " + SaveToDelete + "?");
        DeleteConfirmation.Show();
    }

    public void OnDeleteConfirmationCanceled(){
        SaveToDelete = 0;
        Change();
    }

    public void OnDeleteConfirmationConfirmed(){
        StorageManager.Instance.SetSaves(StorageManager.Instance.GetSaves() ^ (int) Math.Pow(2, SaveToDelete - 1));
        Change();
    }

    public void OnDeleteConfirmationCloseRequested(){
        OnDeleteConfirmationCanceled();
    }

}
