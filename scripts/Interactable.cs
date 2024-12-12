using Godot;
using System;

/**
* @brief Klasse für Interaktion
*/
public partial class Interactable : AnimatedSprite2D {

    private Player Player;
    private RichTextLabel TextLabel;
    private Control PopUp;
    private Area2D Area;

    [Export(PropertyHint.MultilineText)]
    private String Text { get; set;}

    /** 
    * @brief Initialisierung der Referenzen.
    * Findet die relevanten Knoten in der Szene und weist sie zu.
    */
    public override void _Ready(){
        Player = GetNode<Player>("../Player");
        TextLabel = GetNode<RichTextLabel>("../HUD/PopUp/Text");
        PopUp = GetNode<Control>("../HUD/PopUp");
        Area = GetNode<Area2D>("Area2D");
    }

    /**
    * @brief Testet, ob der Spieler mit der Node Interagiert und öffnet ein PopUp.
    * @param DeltaTime Zeit zwischen den Frames.
    */
    public override void _Process(double DeltaTime){
        if(Input.IsActionJustPressed("interact")){
            Godot.Collections.Array<Node2D> Bodies = Area.GetOverlappingBodies();
            foreach(Node2D Body in Bodies){
                if(Body == Player){
                    TextLabel.Clear();
                    TextLabel.AppendText(Text);
                    PopUp.Visible = true;
                    return;
                }
            }
        }
    }

    /**
    * @brief Detektiert, wenn der Spieler den Bereich verlässt und schließt das PopUp.
    * @param Node2D die den Bereich verlässt.
    */
    public void OnAreaBodyExited(Node2D Body){
        if(Body == Player){
            PopUp.Visible = false;
            TextLabel.Clear();
        }
    }

}
