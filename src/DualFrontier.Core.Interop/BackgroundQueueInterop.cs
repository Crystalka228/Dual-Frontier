using System;
using System.IO;
using System.Runtime.InteropServices;

namespace DualFrontier.Core.Interop;

/// <summary>
/// K10.2 Item 31 — Background queue save-integrated storage (S3-Q3
/// untargeted persistence). Managed binding for native serialize / deserialize
/// of <c>pending_background_</c> queue.
///
/// Wire format ownership: native side (K10.2 schema v1). Managed binding
/// is a thin wrapper that round-trips the byte buffer between native code
/// and a save-file <see cref="Stream"/>. Section is optional in the save:
/// older saves без the section load с empty background queue (graceful
/// degradation per R-K10-6 mitigation).
/// </summary>
public static class BackgroundQueueInterop
{
    private const string DllName = "DualFrontier.Core.Native";

    [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
    private static extern int df_background_queue_compute_save_size(out uint out_required_bytes);

    [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
    private static extern int df_background_queue_serialize(
        IntPtr out_buffer, uint buffer_size, out uint out_bytes_written);

    [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
    private static extern int df_background_queue_deserialize(
        IntPtr buffer, uint buffer_size, out uint out_events_loaded);

    /// <summary>
    /// Computes the buffer size required к serialize the current pending
    /// queue. Returns 0 if the queue is empty (header still written —
    /// 12 bytes minimum).
    /// </summary>
    public static int ComputeSaveSize()
    {
        int rc = df_background_queue_compute_save_size(out uint size);
        return rc == 1 ? (int)size : 0;
    }

    /// <summary>
    /// Serializes the pending background queue к a managed byte array.
    /// Returns the array (zero-length если queue is empty AND header was
    /// not requested — currently always returns at least 12 bytes для the
    /// header).
    /// </summary>
    public static byte[] SerializeToArray()
    {
        int size = ComputeSaveSize();
        if (size == 0) return Array.Empty<byte>();
        byte[] buffer = new byte[size];
        unsafe
        {
            fixed (byte* ptr = buffer)
            {
                int rc = df_background_queue_serialize((IntPtr)ptr, (uint)size, out uint written);
                if (rc != 1) throw new InvalidOperationException("Native serialization failed");
                if (written != size) throw new InvalidOperationException(
                    $"Native serialization wrote {written} bytes; expected {size}");
            }
        }
        return buffer;
    }

    /// <summary>
    /// Deserializes pending events from a byte array (typically read from
    /// save). Replaces the current pending queue.
    /// </summary>
    /// <returns>
    /// Number of events loaded; <c>-1</c> on malformed buffer (no state mutation).
    /// </returns>
    public static int DeserializeFromArray(byte[] buffer)
    {
        if (buffer is null) throw new ArgumentNullException(nameof(buffer));
        if (buffer.Length == 0) return 0;
        unsafe
        {
            fixed (byte* ptr = buffer)
            {
                int rc = df_background_queue_deserialize((IntPtr)ptr, (uint)buffer.Length, out uint loaded);
                if (rc != 1) return -1;
                return (int)loaded;
            }
        }
    }

    /// <summary>
    /// Serializes the pending queue к a <see cref="Stream"/>. Writes a
    /// length-prefixed section (uint32 length + bytes) so the save loader
    /// can skip the section если parsing fails (graceful degradation).
    /// </summary>
    public static void WriteSection(Stream output)
    {
        if (output is null) throw new ArgumentNullException(nameof(output));
        byte[] payload = SerializeToArray();
        Span<byte> header = stackalloc byte[4];
        BitConverter.TryWriteBytes(header, payload.Length);
        output.Write(header);
        if (payload.Length > 0) output.Write(payload);
    }

    /// <summary>
    /// Reads the save section produced by <see cref="WriteSection"/>.
    /// Tolerates missing section (returns 0 if the stream is at EOF).
    /// </summary>
    public static int ReadSection(Stream input)
    {
        if (input is null) throw new ArgumentNullException(nameof(input));
        Span<byte> header = stackalloc byte[4];
        int read = 0;
        while (read < 4)
        {
            int n = input.Read(header[read..]);
            if (n <= 0)
            {
                if (read == 0) return 0;  // EOF — section absent (older save)
                throw new InvalidDataException("Truncated background_queue section header");
            }
            read += n;
        }
        int length = BitConverter.ToInt32(header);
        if (length < 0) throw new InvalidDataException("Negative background_queue section length");
        if (length == 0) return 0;
        byte[] payload = new byte[length];
        read = 0;
        while (read < length)
        {
            int n = input.Read(payload, read, length - read);
            if (n <= 0) throw new InvalidDataException("Truncated background_queue section payload");
            read += n;
        }
        return DeserializeFromArray(payload);
    }
}
