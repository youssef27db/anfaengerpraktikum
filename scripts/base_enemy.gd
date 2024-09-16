extends Node2D

# customizable variables
@export var dead = false
@export var respawnable = true
@export var max_health_points = 100
@export var armor = 20
@export var max_stamina = 100
@export var can_jump = false
@export var speed = 10
@export var sin_amount = 10
@export var return_to_start_after = 5

# variables
var current_health_points = max_health_points
var current_stamina = max_stamina
var return_to_start = return_to_start_after
var pursuing = false
var current_target = null
var target_position = null
var start_position = null
var start_rotation = false

@onready var sprite = $RigidBody2D/AnimatedSprite2D
@onready var collision_polygon = $RigidBody2D/detection/CollisionPolygon2D
@onready var ray_cast = $RigidBody2D/RayCast2D

# one time call after everything is initialized
func _ready() -> void:
	start_rotation = sprite.flip_h
	start_position = position

# game loop function
func _process(delta: float) -> void:
	
	# vvvvv movement vvvvv
	
	# sets target position
	if(pursuing):
		target_position = current_target.position
		if(is_close_to(position.x, target_position.x, 0.05)): return
		return_to_start = return_to_start_after
	elif(return_to_start >= 0):
		return_to_start -= delta
		target_position = null
	elif(!is_close_to(position.x, start_position.x, 0.05)):
		target_position = start_position
		
		
	# enemy has target
	if(target_position != null):
	
		# move right
		if(target_position.x > position.x):
			sprite.flip_h = true
			collision_polygon.rotation_degrees = 180
			ray_cast.rotation_degrees = 180
			
			# check if is in front of something
			if(!ray_cast.is_colliding()):
				position.x += delta * speed
			
		# move left
		else :
			sprite.flip_h = false
			collision_polygon.rotation_degrees = 0
			ray_cast.rotation_degrees = 0
			
			# check if is in front of something
			if(!ray_cast.is_colliding()):
				position.x -= delta * speed
			
		# stop movement if position is close to target
		if(is_close_to(position.x, target_position.x, 0.05)):
			if(target_position == start_position && sprite.flip_h != start_rotation):
				flip_rotation()
			target_position = null
	
	pass
	
# signal when player enters detection area -> start following player
func _on_detection_body_entered(body: Node2D) -> void:
	if(check_line_if_sight(body)): 
		pursuing = true
		current_target = body
	pass

# signal when player leaves pursuing area -> stop following player
func _on_pursuing_radius_body_exited(body: Node2D) -> void:
	if(body == current_target):
		pursuing = false
		current_target = null
	pass

# check line of sight to a node
func check_line_if_sight(body: Node2D) -> bool:
	# TODO: implement check
	return true

# flips the rotation of sprite, detection polygon and raycast
func flip_rotation() -> void:
	sprite.flip_h = !sprite.flip_h
	collision_polygon.rotation_degrees = abs(collision_polygon.rotation_degrees - 180)
	ray_cast.rotation_degrees = abs(ray_cast.rotation_degrees - 180)
	

# checks if two values are close to each other with set delta	
func is_close_to(value: float, close: float, delta: float) -> bool:
	var up = close + delta
	var down = close - delta
	return (value <= up && value >= down)
