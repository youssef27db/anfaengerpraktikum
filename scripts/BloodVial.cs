using GdMUT;
using Godot;
using System;

/** 
* @brief Klasse für die Interaktion zum heilen. 
*/
public partial class BloodVial : Label {

    /** 
    * @brief Initialisierung der Referenzen.
    * Findet die relevanten Knoten in der Szene und weist sie zu.
    */
    public override void _Ready() {
        Text = PlayerStats.Instance.GetBVCurrentUses() + "";
    }

    /**
    * @brief Versucht ein Bloodvial zu verwenden um den Spieler zu Heilen.
    */
    public void UseBloodVial(){
        if(PlayerStats.Instance.GetBVCurrentUses() <= 0) return;
        PlayerStats.Instance.SetBVCurrentUses(PlayerStats.Instance.GetBVCurrentUses() - 1);
        Text = PlayerStats.Instance.GetBVCurrentUses() + "";
        PlayerStats.Instance.SetCurrentHealth(PlayerStats.Instance.GetCurrentHealth() + PlayerStats.Instance.GetBVHealAmount());
    }

    /**
    * @brief Setzt die Anzahl der Bloodvials auf das Maximum.
    */
    public void ResetUses(){
        PlayerStats.Instance.SetBVCurrentUses(PlayerStats.Instance.GetBVMaxUses());
        Text = PlayerStats.Instance.GetBVCurrentUses() + "";
    }

    /**
    * @brief Verbessert die Maximale Anzahl an Bloodvials um die angegebene Anzahl.
    * @param int Amount, um die MaxUses erhöht wird.
    */
    public void AddMaxUses(int Amount){
        PlayerStats.Instance.SetBVMaxUses(PlayerStats.Instance.GetBVMaxUses() + Amount);
        ResetUses();
    }

    /**
    * @brief Verbessert den HealAMount eines Bloodvials um 25.
    */
    public void LevelHealAmount(){
        PlayerStats.Instance.SetBVHealAmount(PlayerStats.Instance.GetBVHealAmount() + 25);
    }
}
