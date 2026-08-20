using System.Numerics;
using DualFrontier.Application.Bridge.Commands;
using DualFrontier.Launcher;
using AwesomeAssertions;
using Xunit;

namespace DualFrontier.Runtime.Tests;

/// <summary>
/// W3/G2 — the render half of the ambient-tint primitive. Covers the dispatch arm and the
/// modulation it hands the renderer. The renderer itself needs a live Vulkan device, so what
/// is asserted here is the whole of the tint LOGIC: the stored state, the clamp, and the
/// strength-0 identity that makes "no tint" mean literally the untinted frame.
/// </summary>
public sealed class AmbientTintDispatchTests
{
    private static RenderCommandDispatcher NewDispatcher() => new(new SceneState());

    [Fact]
    public void FreshDispatcher_IsUntinted()
    {
        RenderCommandDispatcher dispatcher = NewDispatcher();

        dispatcher.AmbientTint.W.Should().Be(0f, "a scene starts untinted");
        dispatcher.AmbientModulation.Should().Be(new Vector3(1f, 1f, 1f));
    }

    [Fact]
    public void AmbientTintCommand_UpdatesTheStoredTint()
    {
        RenderCommandDispatcher dispatcher = NewDispatcher();

        dispatcher.Dispatch(new AmbientTintCommand(0.25f, 0.5f, 1f, 0.5f));

        dispatcher.AmbientTint.Should().Be(new Vector4(0.25f, 0.5f, 1f, 0.5f));

        // lerp(white, colour, 0.5) per channel.
        Vector3 m = dispatcher.AmbientModulation;
        m.X.Should().BeApproximately(0.625f, 1e-5f);
        m.Y.Should().BeApproximately(0.75f, 1e-5f);
        m.Z.Should().BeApproximately(1f, 1e-5f);
    }

    [Fact]
    public void StrengthZero_RestoresTheUntintedScene_Exactly()
    {
        RenderCommandDispatcher dispatcher = NewDispatcher();
        dispatcher.Dispatch(new AmbientTintCommand(1f, 0f, 0f, 1f));
        dispatcher.AmbientModulation.Should().Be(new Vector3(1f, 0f, 0f), "full-strength red");

        dispatcher.Dispatch(new AmbientTintCommand(1f, 0f, 0f, 0f));

        dispatcher.AmbientModulation.Should().Be(new Vector3(1f, 1f, 1f),
            "strength 0 is the IDENTITY multiplier, so sprite tints stay white and the clear " +
            "colour stays its base value — the frame is the pre-tint frame, not an approximation");
    }

    [Fact]
    public void OutOfRangeChannelsAndStrength_AreClamped_SoAModCannotDriveTheRendererOutOfRange()
    {
        RenderCommandDispatcher dispatcher = NewDispatcher();

        dispatcher.Dispatch(new AmbientTintCommand(5f, -3f, 0.5f, 42f));

        dispatcher.AmbientTint.Should().Be(new Vector4(1f, 0f, 0.5f, 1f));
    }

    [Fact]
    public void AmbientTintCommand_IsARealArm_NotAnUnknownCommand()
    {
        RenderCommandDispatcher dispatcher = NewDispatcher();

        // The dispatcher throws NotSupportedException on an unhandled IRenderCommand, so a
        // clean dispatch is itself the proof that the arm exists.
        System.Action act = () => dispatcher.Dispatch(new AmbientTintCommand(0f, 0f, 0f, 0f));

        act.Should().NotThrow();
    }
}
