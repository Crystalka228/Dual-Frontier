using System;
using DualFrontier.Application.Bridge;
using DualFrontier.Application.Loop;
using DualFrontier.Runtime;
using DualFrontier.Runtime.Input;
using DualFrontier.Runtime.Window;

namespace DualFrontier.Launcher;

/// <summary>
/// Production launcher entry point for Dual Frontier. Composes Vulkan
/// substrate (<see cref="Runtime.Runtime"/>) + Domain layer
/// (<see cref="GameContext"/> via <see cref="GameBootstrap"/>) +
/// <see cref="LauncherRenderer"/> bridge between them. Drives main loop
/// per Q-G-7 (d) hybrid orchestration.
///
/// К-extensions cascade #2 (2026-05-23) ships infrastructure-only:
/// — Window opens, Vulkan initializes, GameLoop ticks (on its own background
///   thread), bridge connects, dispatcher receives commands (defensive throws
///   fire if visual paths invoked) per Lesson #N12 first application.
/// Visual implementation lands в К-extensions cascade #3 (next session).
///
/// Architecture note (brief amendment, Crystalka Option A ratification
/// 2026-05-23 mid-cascade): brief assumed <c>gameContext.GameLoop.Tick()</c>
/// — empirically <see cref="GameLoop"/> runs on its own background thread
/// (Start/Stop API only, no external Tick()). Main loop drives window +
/// input + rendering only; simulation ticks autonomously on background
/// thread via accumulator-based fixed step. Q-G-7 (d) hybrid orchestration
/// intent preserved — Program.cs still explicitly drives lifecycle
/// (Start/Stop, message pump, render), just не the sim tick (which is
/// background thread concern). Cross-thread communication через
/// <see cref="PresentationBridge"/> command queue.
/// </summary>
internal static class Program
{
    public static int Main(string[] args)
    {
        // === Composition ===
        var runtimeOptions = new RuntimeOptions
        {
            Window = new WindowOptions
            {
                Title = "Dual Frontier",
                Width = 1280,
                Height = 720,
            },
            AssetsDirectory = "assets",
            // EnableValidationLayer: omitted к use RuntimeOptions DEBUG/Release
            // conditional default (#if DEBUG = true, else = false).
        };
        using var runtime = Runtime.Runtime.Create(runtimeOptions);
        var bridge = new PresentationBridge();
        GameContext gameContext = GameBootstrap.CreateLoop(bridge);
        var dispatcher = new RenderCommandDispatcher(runtime);
        using var renderer = new LauncherRenderer(runtime, bridge, dispatcher);

        // === Lifecycle init ===
        renderer.Initialize();
        runtime.Window.Show();
        gameContext.Loop.Start();

        // === Main loop (Q-G-7 (d) hybrid orchestration, amended Crystalka 2026-05-23) ===
        var lastFrameTime = DateTime.UtcNow;
        while (runtime.Window.IsOpen)
        {
            var now = DateTime.UtcNow;
            var deltaSeconds = (now - lastFrameTime).TotalSeconds;
            lastFrameTime = now;

            // 1. Pump Windows messages (surfaces input events к InputQueue).
            runtime.Window.PumpMessages();

            // 2. Drain InputQueue → forward к Application.
            //    К-extensions cascade #3 territory — InputBridge wiring TBD.
            //    Cascade #2: input events pumped but discarded (no consumer yet).
            while (runtime.InputQueue.TryDequeue(out IInputEvent? _))
            {
                // Cascade #3 will forward к Application input bridge here.
            }

            // 3. (No simulation tick here — GameLoop runs on its own background
            //    thread via Start() above. Cross-thread communication через
            //    PresentationBridge command queue. Brief amendment Crystalka
            //    Option A 2026-05-23 — gameContext.Loop self-ticks internally.)

            // 4. Render frame (drain bridge + dispatch + future Runtime record).
            renderer.RenderFrame(deltaSeconds);
        }

        // === Shutdown ===
        gameContext.Loop.Stop();
        renderer.Shutdown();
        return 0;
    }
}
