shader_type canvas_item;

uniform float flash_amount : hint_range(0.0, 1.0) = 0.0;
uniform float warning_amount : hint_range(0.0, 1.0) = 0.0;

void fragment() {
    vec4 source = texture(TEXTURE, UV);
    vec3 warned = mix(source.rgb, vec3(1.0, 0.13, 0.13), warning_amount);
    COLOR = vec4(mix(warned, vec3(1.0), flash_amount), source.a);
}
