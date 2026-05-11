using System;
using FluentAssertions;
using Xunit;

namespace DualFrontier.Core.Interop.Tests;

public sealed class FieldRegistryTests : IDisposable
{
    private readonly NativeWorld _world = new();

    public void Dispose() => _world.Dispose();

    [Fact]
    public void Register_NewField_Succeeds()
    {
        var handle = _world.Fields.Register<float>("test.field", 10, 10);
        handle.Id.Should().Be("test.field");
        handle.Width.Should().Be(10);
        handle.Height.Should().Be(10);
        handle.ElementType.Should().Be(typeof(float));
    }

    [Fact]
    public void Register_SameDimensions_IsIdempotent()
    {
        var first = _world.Fields.Register<float>("test.idem", 5, 5);
        var second = _world.Fields.Register<float>("test.idem", 5, 5);
        second.Should().BeSameAs(first);
    }

    [Fact]
    public void Register_ConflictingDimensions_Throws()
    {
        _world.Fields.Register<float>("test.conf", 5, 5);
        Action act = () => _world.Fields.Register<float>("test.conf", 6, 6);
        act.Should().Throw<InvalidOperationException>();
    }

    [Fact]
    public void Register_ConflictingType_Throws()
    {
        _world.Fields.Register<float>("test.type", 5, 5);
        Action act = () => _world.Fields.Register<int>("test.type", 5, 5);
        act.Should().Throw<InvalidOperationException>();
    }

    [Fact]
    public void Register_EmptyId_Throws()
    {
        Action act = () => _world.Fields.Register<float>("", 5, 5);
        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void Register_NonPositiveDimensions_Throws()
    {
        Action a1 = () => _world.Fields.Register<float>("test.zero", 0, 5);
        Action a2 = () => _world.Fields.Register<float>("test.neg", 5, -1);
        a1.Should().Throw<ArgumentException>();
        a2.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void Get_RegisteredField_Returns()
    {
        var registered = _world.Fields.Register<float>("test.get", 3, 3);
        var got = _world.Fields.Get<float>("test.get");
        got.Should().BeSameAs(registered);
    }

    [Fact]
    public void Get_UnregisteredField_Throws()
    {
        Action act = () => _world.Fields.Get<float>("does.not.exist");
        act.Should().Throw<InvalidOperationException>();
    }

    [Fact]
    public void Get_WrongType_Throws()
    {
        _world.Fields.Register<float>("test.gt", 3, 3);
        Action act = () => _world.Fields.Get<int>("test.gt");
        act.Should().Throw<InvalidOperationException>();
    }

    [Fact]
    public void Unregister_RemovesField()
    {
        _world.Fields.Register<float>("test.un", 3, 3);
        _world.Fields.IsRegistered("test.un").Should().BeTrue();
        _world.Fields.Unregister("test.un");
        _world.Fields.IsRegistered("test.un").Should().BeFalse();
    }

    [Fact]
    public void Count_ReflectsRegistrations()
    {
        _world.Fields.Count.Should().Be(0);
        _world.Fields.Register<float>("a", 2, 2);
        _world.Fields.Register<float>("b", 2, 2);
        _world.Fields.Count.Should().Be(2);
        _world.Fields.Unregister("a");
        _world.Fields.Count.Should().Be(1);
    }
}
