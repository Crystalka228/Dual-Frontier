#version 450

// V0.C.1 sprite vertex shader. Consumes per-vertex 2D position + UV + packed RGBA tint.
// Applies push-constant Camera2D MVP matrix per S-LOCK-8 (V0.C.1 single-sprite test uses
// identity; V0.C.2 Camera2D populates actual ortho projection × view).

layout(location = 0) in vec2 inPos;
layout(location = 1) in vec2 inUv;
layout(location = 2) in vec4 inColor;   // tint, normalized 0..1 via R8G8B8A8_UNORM format

layout(push_constant) uniform PushConstants {
    mat4 mvp;       // model-view-projection matrix (Camera2D)
} pc;

layout(location = 0) out vec2 vUv;
layout(location = 1) out vec4 vColor;

void main() {
    gl_Position = pc.mvp * vec4(inPos, 0.0, 1.0);
    vUv = inUv;
    vColor = inColor;
}
