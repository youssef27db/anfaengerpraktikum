using Godot;

public class Damage{

    private float PhysicalDMG;
    private float TrueDMG;
    private Vector2 PushAmount;

    public Damage(float PhysicalDMG, float TrueDMG, Vector2 PushAmount){
        this.PhysicalDMG = PhysicalDMG;
        this.TrueDMG = TrueDMG;
        this.PushAmount = PushAmount;
    }

    public float GetPhysicalDMG(){
        return PhysicalDMG;
    }

    public float GetTrueDMG(){
        return TrueDMG;
    }

    public Vector2 GetPushAmount(){
        return PushAmount;
    }
}