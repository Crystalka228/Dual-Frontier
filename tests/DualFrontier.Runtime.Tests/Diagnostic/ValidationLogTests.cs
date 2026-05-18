using DualFrontier.Runtime.Diagnostic;
using FluentAssertions;
using Xunit;

namespace DualFrontier.Runtime.Tests.Diagnostic;

public sealed class ValidationLogTests
{
    [Fact]
    public void Record_increments_severity_count()
    {
        var log = new ValidationLog();
        log.Record(ValidationSeverity.Error, "error msg");
        log.Record(ValidationSeverity.Warning, "warning msg");
        log.Record(ValidationSeverity.Warning, "another warning");
        log.Record(ValidationSeverity.Info, "info msg");

        log.ErrorCount.Should().Be(1);
        log.WarningCount.Should().Be(2);
        log.InfoCount.Should().Be(1);
    }

    [Fact]
    public void Snapshot_returns_recorded_messages_in_order()
    {
        var log = new ValidationLog();
        log.Record(ValidationSeverity.Error, "first");
        log.Record(ValidationSeverity.Warning, "second");

        var snapshot = log.Snapshot();
        snapshot.Should().HaveCount(2);
        snapshot[0].Message.Should().Be("first");
        snapshot[0].Severity.Should().Be(ValidationSeverity.Error);
        snapshot[1].Message.Should().Be("second");
        snapshot[1].Severity.Should().Be(ValidationSeverity.Warning);
    }

    [Fact]
    public void Clear_resets_state()
    {
        var log = new ValidationLog();
        log.Record(ValidationSeverity.Error, "msg");
        log.Clear();

        log.ErrorCount.Should().Be(0);
        log.WarningCount.Should().Be(0);
        log.InfoCount.Should().Be(0);
        log.Snapshot().Should().BeEmpty();
    }

    [Fact]
    public void Ring_buffer_evicts_oldest_at_capacity()
    {
        var log = new ValidationLog();
        for (int i = 0; i < 1100; i++)
        {
            log.Record(ValidationSeverity.Warning, $"msg {i}");
        }
        var snapshot = log.Snapshot();
        snapshot.Should().HaveCount(1024);
        // Counts are NOT reset on eviction — they track total recorded, не current buffer size.
        log.WarningCount.Should().Be(1100);
        // Oldest message in buffer is the (1100-1024)th = 76th
        snapshot[0].Message.Should().Be("msg 76");
        snapshot[^1].Message.Should().Be("msg 1099");
    }
}
