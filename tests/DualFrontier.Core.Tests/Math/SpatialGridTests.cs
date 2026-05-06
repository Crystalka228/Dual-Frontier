using System;
using DualFrontier.Contracts.Core;
using DualFrontier.Contracts.Math;
using DualFrontier.Core.Math;
using FluentAssertions;
using Xunit;

namespace DualFrontier.Core.Tests.Math;

public sealed class SpatialGridTests
{
    [Fact]
    public void Constructor_PositiveDimensions_Succeeds()
    {
        var act = () => new SpatialGrid(10, 20);
        act.Should().NotThrow();
    }

    [Fact]
    public void Constructor_ZeroOrNegativeWidth_Throws()
    {
        var actZero = () => new SpatialGrid(0, 5);
        var actNegative = () => new SpatialGrid(-1, 5);

        actZero.Should().Throw<ArgumentOutOfRangeException>();
        actNegative.Should().Throw<ArgumentOutOfRangeException>();
    }

    [Fact]
    public void Constructor_ZeroOrNegativeHeight_Throws()
    {
        var actZero = () => new SpatialGrid(5, 0);
        var actNegative = () => new SpatialGrid(5, -1);

        actZero.Should().Throw<ArgumentOutOfRangeException>();
        actNegative.Should().Throw<ArgumentOutOfRangeException>();
    }

    [Fact]
    public void Width_And_Height_ReflectConstructorArgs()
    {
        var grid = new SpatialGrid(10, 20);

        grid.Width.Should().Be(10);
        grid.Height.Should().Be(20);
    }

    [Fact]
    public void GetAt_EmptyCell_ReturnsEmpty()
    {
        var grid = new SpatialGrid(10, 10);

        grid.GetAt(new GridVector(3, 3)).Should().BeEmpty();
    }

    [Fact]
    public void GetAt_OutOfBounds_ReturnsEmpty()
    {
        var grid = new SpatialGrid(10, 10);

        grid.GetAt(new GridVector(-1, 5)).Should().BeEmpty();
        grid.GetAt(new GridVector(5, -1)).Should().BeEmpty();
        grid.GetAt(new GridVector(10, 5)).Should().BeEmpty();
        grid.GetAt(new GridVector(5, 10)).Should().BeEmpty();
    }

    [Fact]
    public void Add_Then_GetAt_ReturnsTheEntity()
    {
        var grid = new SpatialGrid(10, 10);
        var id = new EntityId(1, 0);
        var pos = new GridVector(4, 7);

        grid.Add(pos, id);

        grid.GetAt(pos).Should().ContainSingle().Which.Should().Be(id);
    }

    [Fact]
    public void Add_MultipleEntities_GetAt_ReturnsAll()
    {
        var grid = new SpatialGrid(10, 10);
        var id1 = new EntityId(1, 0);
        var id2 = new EntityId(2, 0);
        var pos = new GridVector(4, 7);

        grid.Add(pos, id1);
        grid.Add(pos, id2);

        grid.GetAt(pos).Should().BeEquivalentTo(new[] { id1, id2 });
    }

    [Fact]
    public void Remove_RemovesEntity_FromCell()
    {
        var grid = new SpatialGrid(10, 10);
        var id = new EntityId(1, 0);
        var pos = new GridVector(4, 7);

        grid.Add(pos, id);
        grid.Remove(pos, id);

        grid.GetAt(pos).Should().BeEmpty();
    }

    [Fact]
    public void Move_RelocatesEntity_BetweenCells()
    {
        var grid = new SpatialGrid(10, 10);
        var id = new EntityId(1, 0);
        var from = new GridVector(2, 2);
        var to = new GridVector(7, 8);

        grid.Add(from, id);
        grid.Move(from, to, id);

        grid.GetAt(from).Should().BeEmpty();
        grid.GetAt(to).Should().ContainSingle().Which.Should().Be(id);
    }

    [Fact]
    public void RemoveAll_ClearsEntityFromAllTiles()
    {
        var grid = new SpatialGrid(10, 10);
        var id = new EntityId(1, 0);
        var p1 = new GridVector(1, 1);
        var p2 = new GridVector(5, 5);
        var p3 = new GridVector(9, 9);

        grid.Add(p1, id);
        grid.Add(p2, id);
        grid.Add(p3, id);

        grid.RemoveAll(id);

        grid.GetAt(p1).Should().BeEmpty();
        grid.GetAt(p2).Should().BeEmpty();
        grid.GetAt(p3).Should().BeEmpty();
    }

    [Fact]
    public void Clear_RemovesAllEntities()
    {
        var grid = new SpatialGrid(10, 10);
        var id1 = new EntityId(1, 0);
        var id2 = new EntityId(2, 0);
        var p1 = new GridVector(1, 1);
        var p2 = new GridVector(5, 5);

        grid.Add(p1, id1);
        grid.Add(p1, id2);
        grid.Add(p2, id1);

        grid.Clear();

        grid.GetAt(p1).Should().BeEmpty();
        grid.GetAt(p2).Should().BeEmpty();
    }

    [Fact]
    public void GetInRadius_RadiusZero_ReturnsOnlyCenterCellEntities()
    {
        var grid = new SpatialGrid(10, 10);
        var idCenter = new EntityId(1, 0);
        var idAdjacent = new EntityId(2, 0);
        var center = new GridVector(5, 5);
        var adjacent = new GridVector(6, 5);

        grid.Add(center, idCenter);
        grid.Add(adjacent, idAdjacent);

        var result = grid.GetInRadius(center, 0);

        result.Should().ContainSingle().Which.Should().Be(idCenter);
    }

    [Fact]
    public void GetInRadius_RadiusOne_ReturnsAll9TileEntities()
    {
        var grid = new SpatialGrid(10, 10);
        var idCenter = new EntityId(1, 0);
        var idTL = new EntityId(2, 0);
        var idBR = new EntityId(3, 0);
        var idBL = new EntityId(4, 0);
        var idTR = new EntityId(5, 0);
        var center = new GridVector(5, 5);

        grid.Add(center, idCenter);
        grid.Add(new GridVector(4, 4), idTL);
        grid.Add(new GridVector(6, 6), idBR);
        grid.Add(new GridVector(4, 6), idBL);
        grid.Add(new GridVector(6, 4), idTR);

        var result = grid.GetInRadius(center, 1);

        result.Should().BeEquivalentTo(new[] { idCenter, idTL, idBR, idBL, idTR });
    }

    [Fact]
    public void GetInRadius_ClipsToGridBounds()
    {
        var grid = new SpatialGrid(10, 10);
        var id = new EntityId(1, 0);
        grid.Add(new GridVector(0, 0), id);

        var act = () => grid.GetInRadius(new GridVector(0, 0), 5);

        act.Should().NotThrow();
        act().Should().ContainSingle().Which.Should().Be(id);
    }
}
