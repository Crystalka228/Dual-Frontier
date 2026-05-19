#version 450

// Fullscreen triangle vertex shader — no vertex buffer required.
// 3 vertices via gl_VertexIndex produce a triangle covering [-1,3]×[-1,3]
// which clips to the entire [-1,1] viewport.
void main() {
    vec2 positions[3] = vec2[](vec2(-1.0, -1.0), vec2(3.0, -1.0), vec2(-1.0, 3.0));
    gl_Position = vec4(positions[gl_VertexIndex], 0.0, 1.0);
}
