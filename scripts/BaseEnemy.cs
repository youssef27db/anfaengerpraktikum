using Godot;
using System;

public partial class BaseEnemy : CharacterBody2D
{

    private enum State {
        IDLE, WALK, ATTACK, TAKE_HIT
    }

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
    private State AnimationState = State.IDLE;

    //linked nodes
    private AnimatedSprite2D Sprite;
    private CollisionPolygon2D CollisionPolygon;
    private Area2D SwordHitbox;
    private CollisionShape2D MainCollision;
    private RayCast2D FrontCollisionRayCast;
    private RayCast2D LineOfSight;
    private RayCast2D LeftFallProtection;
    private RayCast2D RightFallProtection;
    private TextureProgressBar HealthBar;

    public override void _Ready()
    {
        Sprite = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
        CollisionPolygon = GetNode<CollisionPolygon2D>("detection/CollisionPolygon2D");
        SwordHitbox = GetNode<Area2D>("AnimatedSprite2D/SwordHitBox");
        MainCollision = GetNode<CollisionShape2D>("MainCollision");
        FrontCollisionRayCast = GetNode<RayCast2D>("FrontCollisionRayCast");
        LineOfSight = GetNode<RayCast2D>("LineOfSight");
        LeftFallProtection = GetNode<RayCast2D>("LeftFallProtection");
        RightFallProtection = GetNode<RayCast2D>("RightFallProtection");
        HealthBar = GetNode<TextureProgressBar>("HealthBar");

        CurrentHealthPoints = MaxHealthPoints;
        CurrentStamina = MaxStamina;
        ReturnToStart = ReturnToStartAfter;
        StartPosition = Position;
        StartRotation = Sprite.FlipH;

        HealthBar.Value = 100f* CurrentHealthPoints/MaxHealthPoints;
    }

    public override void _Process(double DeltaTime)
    {
        HandleMovement(DeltaTime);
        if (!IsOnFloor() && !Dead) {
            Velocity += GetGravity() * (float)DeltaTime;
        }
        UpdateAnimation();
        MoveAndSlide();
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

    public void OnHitboxAreaEntered(Area2D area){
        Player Player1 = (Player) area.GetParent().GetParent();
        TakeDamage(Player1.GetDamage());
    }

    public void OnSwordHitBoxBodyEntered(Node2D body){
        Sprite.Play("attack");
        GD.Print("Test");

    }

    private void HandleMovement(double DeltaTime){
        if(Dead) return;
        if((Sprite.Animation == "take_hit" || Sprite.Animation == "attack") && Sprite.IsPlaying()){
            Velocity = Vector2.Zero;
            return;
        }
        if(Pursuing){
            AnimationState = State.WALK;
            TargetPosition = CurrentTarget.Position;
            if(IsCloseTo(Position.X, TargetPosition.X, 0.1f)){
                AnimationState = State.IDLE;
                Velocity = Vector2.Zero;
                return;
            }
            ReturnToStart = ReturnToStartAfter;
        } else if(ReturnToStart >= 0){
            AnimationState = State.IDLE;
            ReturnToStart -= DeltaTime;
            TargetPosition = Vector2.Inf;
        } else if(!IsCloseTo(Position.X, StartPosition.X, 0.1f)){
            AnimationState = State.WALK;
            TargetPosition = StartPosition;
        }

        if(TargetPosition != Vector2.Inf){

            if(IsCloseTo(Position.X, TargetPosition.X, 0.1f)){
                AnimationState = State.IDLE;
                Velocity = Vector2.Zero;
                if(TargetPosition == StartPosition && Sprite.FlipH != StartRotation){
                    FlipRotation();
                }
                TargetPosition = Vector2.Inf;
                return;
            }

            if(TargetPosition.X > Position.X){
                SetRotation(true);
                if(!FrontCollisionRayCast.IsColliding()){
                    Vector2 velocity = Vector2.Zero;
                    velocity.X = Speed;
                    Velocity = velocity;
                }
            } else {
                SetRotation(false);
                if(!FrontCollisionRayCast.IsColliding()){
                    Vector2 velocity = Vector2.Zero;
                    velocity.X = -Speed;
                    Velocity = velocity;
                }
            }

            if((!RightFallProtection.IsColliding() && !Sprite.FlipH) || (!LeftFallProtection.IsColliding() && Sprite.FlipH)){
                Velocity = Vector2.Zero;
            }

        } else {
            Velocity = Vector2.Zero;
            AnimationState = State.IDLE;
        }
    }

    private void UpdateAnimation(){
        if(Dead) return;
        if(!((Sprite.Animation == "take_hit" || Sprite.Animation == "attack") && Sprite.IsPlaying())){
            switch(AnimationState){
                case State.IDLE: 
                    Sprite.Play("idle");
                    break;
                
                case State.WALK:
                    Sprite.Play("walk");
                    break;
                    
                case State.ATTACK:
                    Sprite.Play("attack");
                    break;
                
                case State.TAKE_HIT:
                    Sprite.Play("take_hit");
                    break;
            }
        }
        HealthBar.Value = 100f* CurrentHealthPoints/MaxHealthPoints;

    }

    private void TakeDamage(Damage DMG){
        CurrentHealthPoints -= DMG.GetPhysicalDMG() + DMG.GetTrueDMG();
        Position += DMG.GetPushAmount();
        if(CurrentHealthPoints <= 0){
            Die();
        } else {
            Sprite.Play("take_hit");
        }
    }

    private void Die(){
        Dead = true;
        Velocity = Vector2.Zero;
        MainCollision.SetDeferred(CollisionShape2D.PropertyName.Disabled, true);
        Sprite.Play("death");
        HealthBar.SetVisible(false);

    }

    private bool CheckLineOfSight(Node2D body){
        Vector2 offset = Vector2.Zero;
        offset.Y = -14;
        LineOfSight.TargetPosition = body.Position + offset - (Position + LineOfSight.Position);
        if(LineOfSight.IsColliding()){
            return LineOfSight.GetCollider() == body;
        }
        return true;
    }

    //flips rotation of sprite and collision nodes
    private void FlipRotation(){
        Sprite.FlipH = !Sprite.FlipH;
        CollisionPolygon.RotationDegrees = Math.Abs(CollisionPolygon.RotationDegrees -180);
        SwordHitbox.RotationDegrees = Math.Abs(SwordHitbox.RotationDegrees -180);
        FrontCollisionRayCast.RotationDegrees = Math.Abs(FrontCollisionRayCast.RotationDegrees - 180);
    }

    private void SetRotation(bool Rotation){
        Sprite.FlipH = Rotation ^ StartRotation;
        if(Rotation){
            CollisionPolygon.RotationDegrees = 180;
            SwordHitbox.RotationDegrees = 180;
            FrontCollisionRayCast.RotationDegrees = 180;
        } else {
            CollisionPolygon.RotationDegrees = 0;
            SwordHitbox.RotationDegrees = 0;
            FrontCollisionRayCast.RotationDegrees = 0;
        }
    }

    private bool IsCloseTo(float Value1, float Value2, float Delta){
        return (Value1 <= Value2 + Delta && Value1 >= Value2 - Delta);
    }
}
