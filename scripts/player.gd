extends CharacterBody2D


const SPEED = 100
const JUMP_VELOCITY = -300

@onready var animated_sprite: AnimatedSprite2D = $AnimatedSprite2D

func _physics_process(delta: float) -> void:
	# Add the gravity.
	if not is_on_floor():
		velocity += get_gravity() * delta

	# Handle jump.
	if Input.is_action_just_pressed("ui_up") and is_on_floor():
		velocity.y = JUMP_VELOCITY

	#-1, 0, 1
	var direction := Input.get_axis("ui_left", "ui_right")
	
	#Flip direction
	if direction < 0:
		animated_sprite.flip_h = true
	elif direction > 0:
		animated_sprite.flip_h = false
	
	#Walk right or left
	if direction:
		velocity.x = direction * SPEED
	else:
		velocity.x = move_toward(velocity.x, 0, SPEED)
		
	#Play animations
	if is_on_floor():
		if direction == 0:
			animated_sprite.play("Idle")
		else: 
			animated_sprite.play("run")
	else:
		animated_sprite.play("jump")
		
	move_and_slide()
