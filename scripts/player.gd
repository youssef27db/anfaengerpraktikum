extends CharacterBody2D

const SPEED = 100
const JUMP_VELOCITY = -300

# Variables for double jump
var jump_max = 2  
var jump_count = 0 

# Variables for dash
var dash_direction = Vector2.ZERO  # Direction of the dash (2D vector)
var dash_speed = 300  
var is_dashing = false 
var can_dash = true 
var dash_trail_interval = 0.05  # Interval at which dash trail is created
var dash_trail_timer = 0.0  # Timer to track the interval

@onready var animation_player = $AnimationPlayer  
@onready var sprite = $Sprite2D 
@onready var dashEffect = $DashEffect

func _physics_process(delta: float) -> void:
	# Add gravity if the character is not on the floor
	if not is_on_floor():
		velocity += get_gravity() * delta
	else:
		can_dash = true  # Reset `can_dash` when the character is on the floor

	# Handle jump and double jump
	# Reset the jump counter if the character is on the floor
	if jump_count != 0 and is_on_floor():
		jump_count = 0

	# Check if the jump button is pressed and the character has jumps left
	if Input.is_action_just_pressed("ui_up") and jump_count < jump_max:
		velocity.y = JUMP_VELOCITY  
		jump_count += 1 

	# Determine movement direction (normalized to handle diagonal movement)
	var direction = Vector2(
		Input.get_axis("ui_left", "ui_right"),
		Input.get_axis("ui_up", "ui_down")
	).normalized()

	# Flip the sprite based on the movement direction
	if direction.x < 0:
		sprite.flip_h = true
	elif direction.x > 0:
		sprite.flip_h = false

	# Handle walking or dashing
	if is_dashing:
		velocity = dash_direction * dash_speed 
		# Create dash trail at intervals
		dash_trail_timer -= delta
		if dash_trail_timer <= 0:
			create_dash_effect()
			dash_trail_timer = dash_trail_interval
	else: 
		if direction != Vector2.ZERO: 
			velocity.x = direction.x * SPEED  
		else:
			velocity.x = move_toward(velocity.x, 0, SPEED)

	# Check if the dash button is pressed and dashing is possible
	if Input.is_action_just_pressed("dash"):
		if direction != Vector2.ZERO and can_dash:
			dash_direction = direction  # Set the dash direction
			start_dash()  # Start the dash

	move_and_slide()
	update_animations(direction)

# Function for updating animations
func update_animations(direction):
	if is_on_floor():
		if direction == Vector2.ZERO:
			animation_player.play("idle")  
		else: 
			animation_player.play("run") 
	else:
		if velocity.y < 0:
			animation_player.play("jump") 
		elif velocity.y > 0:
			animation_player.play("fall")  

# Functions for dashing
func start_dash():
	is_dashing = true  # Set the status to dashing
	can_dash = false  # Prevent further dashes until reset
	$DashTimer.connect("timeout", stop_dash)  
	$DashTimer.start()
	dashEffect.start()
	dash_trail_timer = 0.0  # Reset the dash trail timer

# Add the duplicated sprite to the parent
func create_dash_effect():
	var playerCopyNode = $Sprite2D.duplicate()
	get_parent().add_child(playerCopyNode)
	
	# Set the copy's position to match the player's global position with an offset
	playerCopyNode.global_position = global_position + Vector2(0, sprite.texture.get_height() * sprite.scale.y * -0.5)
	
	# Add fading effect to the dash trail
	var animationTime = $DashTimer.wait_time / 3
	await get_tree().create_timer(animationTime).timeout
	playerCopyNode.modulate.a = 0.4
	await get_tree().create_timer(animationTime).timeout
	playerCopyNode.modulate.a = 0.2
	await get_tree().create_timer(animationTime).timeout
	playerCopyNode.queue_free()
	
func stop_dash():
	is_dashing = false
	dashEffect.stop()
	$DashTimer.disconnect("timeout", stop_dash)
