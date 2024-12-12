using Godot;
using System;

/** 
* @brief Klasse für die Interaktion zum heilen. 
*/
public partial class BloodVial : Label {

    private Player Player;
    [Export]
    private int HealAmount = 25;
    [Export]
    private int MaxUses = 5;
    private int CurrentUses;

    /** 
    * @brief Initialisierung der Referenzen.
    * Findet die relevanten Knoten in der Szene und weist sie zu.
    */
    public override void _Ready() {
        
        CurrentUses = MaxUses;
        Player = GetNode<Player>("../../../Player");
        Text = CurrentUses + "";

    }

    /**
    * @brief Versucht ein Bloodvial zu verwenden um den Spieler zu Heilen.
    */
    public void UseBloodVial(){
        if(CurrentUses <= 0) return;
        CurrentUses--;
        Text = CurrentUses + "";
        Player.SetCurrentHealth(Player.GetCurrentHealth() + HealAmount);
    }

    /**
    * @brief Setzt die Anzahl der Bloodvials auf das Maximum.
    */
    public void ResetUses(){
        CurrentUses = MaxUses;
        Text = CurrentUses + "";
    }

    /**
    * @brief Verbessert die Maximale Anzahl an Bloodvials um die angegebene Anzahl.
    * @brief int Amount, um die MaxUses erhöht wird.
    */
    public void AddMaxUses(int Amount){
        MaxUses += Amount;
        CurrentUses += Amount;
        Text = CurrentUses + "";
    }

    /**
    * @brief Verbessert den HealAMount eines Bloodvials um 25.
    */
    public void LevelHealAmount(){
        HealAmount += 25;
    }
}
