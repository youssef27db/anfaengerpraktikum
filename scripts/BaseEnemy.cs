using Godot;
using System;

public partial class BaseEnemy : Node2D
{

    //customizable variables
    [Export]
    private bool dead = false;
    [Export]
    private bool respawnable = true;
    [Export]
    private float max_health_points = 100f;
    [Export]
    private float armor = 20f;
    [Export]
    private float max_stamina = 100f;
    [Export]
    private bool can_jump = false;
    [Export]
    private float speed = 10;
    [Export]
    private int sin_amount = 10;
    [Export]
    private double return_to_start_after = 5;

    //private variables
    private float current_health_points;
    private float current_stamina;
    private double return_to_start;
    private bool pursuing = false;
    private Node2D current_target = null;
    private Vector2 target_position = Vector2.Inf;
    private Vector2 start_position;
    private bool start_rotation = false;

    //linked nodes
    private AnimatedSprite2D sprite;
    private CollisionPolygon2D collision_polygon;
    private RayCast2D front_collision_ray_cast;

    public override void _Ready()
    {
        sprite = GetNode<AnimatedSprite2D>("RigidBody2D/AnimatedSprite2D");
        collision_polygon = GetNode<CollisionPolygon2D>("RigidBody2D/detection/CollisionPolygon2D");
        front_collision_ray_cast = GetNode<RayCast2D>("RigidBody2D/FrontCollisionRayCast");

        current_health_points = max_health_points;
        current_stamina = max_stamina;
        return_to_start = return_to_start_after;
        start_position = Position;
        start_rotation = sprite.FlipH;
    }

    public override void _Process(double delta_time)
    {
        HandleMovement(delta_time);
    }

    //signal when player enters detection area -> start following player
    public void OnDetectionBodyEntered(Node2D body){
        if(CheckLineOfSight(body)){
            pursuing = true;
            current_target = body;
        }
    }

    //signal when player leaves pursuing area -> stop following player
    public void OnPursuingRadiusBodyExited(Node2D body){
        if(body == current_target){
            pursuing = false;
            current_target = null;
        }
    }

    private void HandleMovement(double delta_time){
        
        if(pursuing){
            target_position = current_target.Position;
            if(IsCloseTo(Position.X, target_position.X, 0.05f)){
                return;
            }
            return_to_start = return_to_start_after;
        } else if(return_to_start >= 0){
            return_to_start -= delta_time;
            target_position = Vector2.Inf;
        } else if(!IsCloseTo(Position.X, start_position.X, 0.05f)){
            target_position = start_position;
        }

        if(target_position != Vector2.Inf){

            if(target_position.X > Position.X){
                SetRotation(true);
                if(!front_collision_ray_cast.IsColliding()){
                    Vector2 new_position = Position;
                    new_position.X += (float) (delta_time * speed);
                    Position = new_position;
                }
            } else {
                SetRotation(false);
                if(!front_collision_ray_cast.IsColliding()){
                    Vector2 new_position = Position;
                    new_position.X -= (float) (delta_time * speed);
                    Position = new_position;
                }
            }

            if(IsCloseTo(Position.X, target_position.X, 0.05f)){
                if(target_position == start_position && sprite.FlipH != start_rotation){
                    FlipRotation();
                }
                target_position = Vector2.Inf;
            }
        }
    }

    private bool CheckLineOfSight(Node2D body){
        //TODO: implement check
        return true;
    }

    //flips rotation of sprite and collision nodes
    private void FlipRotation(){
        sprite.FlipH = !sprite.FlipH;
        collision_polygon.RotationDegrees = Math.Abs(collision_polygon.RotationDegrees -180);
        front_collision_ray_cast.RotationDegrees = Math.Abs(front_collision_ray_cast.RotationDegrees - 180);
    }

    private void SetRotation(bool rotation){
        sprite.FlipH = rotation;
        if(rotation){
            collision_polygon.RotationDegrees = 180;
            front_collision_ray_cast.RotationDegrees = 180;
        } else {
            collision_polygon.RotationDegrees = 0;
            front_collision_ray_cast.RotationDegrees = 0;
        }
    }

    private bool IsCloseTo(float value1, float value2, float delta){
        return (value1 <= value2 + delta && value1 >= value2 - delta);
    }
}
