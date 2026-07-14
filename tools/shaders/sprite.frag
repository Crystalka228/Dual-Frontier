#version 450

// V0.C.1 sprite fragment shader. Samples combined image+sampler descriptor + multiplies
// vertex tint. Discards fully-transparent fragments (alpha < 0.01) к avoid blend overhead.

layout(set = 0, binding = 0) uniform sampler2D atlas;

layout(location = 0) in vec2 vUv;
layout(location = 1) in vec4 vColor;

layout(location = 0) out vec4 outColor;

void main() {
    vec4 sampled = texture(atlas, vUv);
    vec4 straight = sampled * vColor;
    // S-LOCK-5: premultiplied-alpha workflow. The sprite pipeline blends with
    // ONE / ONE_MINUS_SRC_ALPHA, which requires the source RGB already scaled by alpha.
    // The atlas is uploaded straight-alpha and the tint is straight, so premultiply here —
    // otherwise semi-transparent texels over-contribute their own colour (F04). Discard
    // near-zero alpha к save blend cost.
    if (straight.a < 0.01) {
        discard;
    }
    outColor = vec4(straight.rgb * straight.a, straight.a);
}
