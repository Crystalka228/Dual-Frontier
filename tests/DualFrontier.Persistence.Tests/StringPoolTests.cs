using DualFrontier.Persistence.Compression;
using FluentAssertions;
using Xunit;

namespace DualFrontier.Persistence.Tests;

public sealed class StringPoolTests
{
    [Fact]
    public void StringPool_intern_dedup()
    {
        var pool = new StringPool();

        ushort first  = pool.Intern("iron_ore");
        ushort second = pool.Intern("iron_ore");

        second.Should().Be(first, "interning the same string must return the same handle");
        pool.Count.Should().Be(1, "duplicate intern must not grow the pool");
        pool.Resolve(first).Should().Be("iron_ore");
    }
}
