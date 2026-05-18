#version 450

// V0.B clearcolor fragment — outputs a dark blue solid color.
// Render pass loadOp = CLEAR with this color is also acceptable, but the
// pipeline shader provides explicit fragment output for graphics pipeline
// minimality test (Commit 11).
layout(location = 0) out vec4 outColor;

void main() {
    outColor = vec4(0.1, 0.2, 0.4, 1.0);
}
