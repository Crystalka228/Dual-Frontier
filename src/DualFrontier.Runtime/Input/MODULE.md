# DualFrontier.Runtime.Input

**Purpose:** Typed input events. V0.A scope is marker interface only — concrete event types
(`KeyPressedEvent`, `MouseMovedEvent`, etc.) и actual Win32 message → typed event translation
land V0.C.

**Spec authority:** [VULKAN_SUBSTRATE.md](../../../docs/architecture/VULKAN_SUBSTRATE.md) §2.2 Input module.

**Dependencies:** `System` (BCL).

## V0.A surface

- `IInputEvent` — marker interface для future event types

## V0.C surface (deferred)

- Concrete events: `KeyPressedEvent`, `KeyReleasedEvent`, `MouseMovedEvent`,
  `MouseButtonEvent`, `MouseWheelEvent`, `WindowResizedEvent`, `WindowFocusEvent`
- Enums: `Key`, `MouseButton`
- Window.cs WindowProcedure dispatch: WM_KEYDOWN → KeyPressedEvent.Enqueue(InputEventQueue)
