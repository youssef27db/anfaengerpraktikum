shader_type canvas_item;

uniform sampler2D screen_texture: hint_screen_texture, filter_linear_mipmap;
uniform float amount: hint_range(0.0, 6.0);

void fragment() {
	COLOR = textureLod(screen_texture, SCREEN_UV, amount);
}