using Godot;
using System;

/**
* @brief Klasse für die MainMenuBackground-Animation
*/
public partial class MainMenuBackground : ParallaxLayer {

    [Export]
    private float ScrollSpeed = -10f;

    /** 
    * @brief Methode wird in jedem Frame ausgeführt.
    * @param DeltaTime Zeit seit dem letzten Frame.
    */
    public override void _Process(double delta) {
        float X = GetMotionOffset().X;
        X += ScrollSpeed * (float) delta;
        SetMotionOffset(new Vector2(X,0));
    }
}
