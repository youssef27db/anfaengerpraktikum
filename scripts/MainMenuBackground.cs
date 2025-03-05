using Godot;
using System;

public partial class MainMenuBackground : ParallaxLayer {

    [Export]
    private float ScrollSpeed = -10f;

    public override void _Process(double delta) {
        float X = GetMotionOffset().X;
        X += ScrollSpeed * (float) delta;
        SetMotionOffset(new Vector2(X,0));
    }
}
