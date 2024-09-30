using Godot;
using System;

public partial class BaseEnemy : Node2D
{

    //customizable variables
    [Export]
    private bool Dead = false;
    [Export]
    private bool Respawnable = true;
    [Export]
    private float MaxHealthPoints = 100f;
    [Export]
    private float Armor = 20f;
    [Export]
    private float MaxStamina = 100f;
    [Export]
    private bool CanJump = false;
    [Export]
    private float Speed = 10;
    [Export]
    private int SinAmount = 10;
    [Export]
    private double ReturnToStartAfter = 5;

    //private variables
    private float CurrentHealthPoints;
    private float CurrentStamina;
    private double ReturnToStart;
    private bool Pursuing = false;
    private Node2D CurrentTarget = null;
    private Vector2 TargetPosition = Vector2.Inf;
    private Vector2 StartPosition;
    private bool StartRotation = false;

    //linked nodes
    private AnimatedSprite2D Sprite;
    private CollisionPolygon2D CollisionPolygon;
    private RayCast2D FrontCollisionRayCast;

    public override void _Ready()
    {
        Sprite = GetNode<AnimatedSprite2D>("RigidBody2D/AnimatedSprite2D");
        CollisionPolygon = GetNode<CollisionPolygon2D>("RigidBody2D/detection/CollisionPolygon2D");
        FrontCollisionRayCast = GetNode<RayCast2D>("RigidBody2D/FrontCollisionRayCast");

        CurrentHealthPoints = MaxHealthPoints;
        CurrentStamina = MaxStamina;
        ReturnToStart = ReturnToStartAfter;
        StartPosition = Position;
        StartRotation = Sprite.FlipH;
    }

    public override void _Process(double DeltaTime)
    {
        HandleMovement(DeltaTime);
    }

    //signal when player enters detection area -> start following player
    public void OnDetectionBodyEntered(Node2D body){
        if(CheckLineOfSight(body)){
            Pursuing = true;
            CurrentTarget = body;
        }
    }

    //signal when player leaves pursuing area -> stop following player
    public void OnPursuingRadiusBodyExited(Node2D body){
        if(body == CurrentTarget){
            Pursuing = false;
            CurrentTarget = null;
        }
    }

    private void HandleMovement(double DeltaTime){
        
        if(Pursuing){
            TargetPosition = CurrentTarget.Position;
            if(IsCloseTo(Position.X, TargetPosition.X, 0.05f)){
                return;
            }
            ReturnToStart = ReturnToStartAfter;
        } else if(ReturnToStart >= 0){
            ReturnToStart -= DeltaTime;
            TargetPosition = Vector2.Inf;
        } else if(!IsCloseTo(Position.X, StartPosition.X, 0.05f)){
            TargetPosition = StartPosition;
        }

        if(TargetPosition != Vector2.Inf){

            if(TargetPosition.X > Position.X){
                SetRotation(true);
                if(!FrontCollisionRayCast.IsColliding()){
                    Vector2 new_position = Position;
                    new_position.X += (float) (DeltaTime * Speed);
                    Position = new_position;
                }
            } else {
                SetRotation(false);
                if(!FrontCollisionRayCast.IsColliding()){
                    Vector2 new_position = Position;
                    new_position.X -= (float) (DeltaTime * Speed);
                    Position = new_position;
                }
            }

            if(IsCloseTo(Position.X, TargetPosition.X, 0.05f)){
                if(TargetPosition == StartPosition && Sprite.FlipH != StartRotation){
                    FlipRotation();
                }
                TargetPosition = Vector2.Inf;
            }
        }
    }

    private bool CheckLineOfSight(Node2D body){
        //TODO: implement check
        return true;
    }

    //flips rotation of sprite and collision nodes
    private void FlipRotation(){
        Sprite.FlipH = !Sprite.FlipH;
        CollisionPolygon.RotationDegrees = Math.Abs(CollisionPolygon.RotationDegrees -180);
        FrontCollisionRayCast.RotationDegrees = Math.Abs(FrontCollisionRayCast.RotationDegrees - 180);
    }

    private void SetRotation(bool Rotation){
        Sprite.FlipH = Rotation;
        if(Rotation){
            CollisionPolygon.RotationDegrees = 180;
            FrontCollisionRayCast.RotationDegrees = 180;
        } else {
            CollisionPolygon.RotationDegrees = 0;
            FrontCollisionRayCast.RotationDegrees = 0;
        }
    }

    private bool IsCloseTo(float Value1, float Value2, float Delta){
        return (Value1 <= Value2 + Delta && Value1 >= Value2 - Delta);
    }
}
