using System;
using System.Runtime.CompilerServices;

namespace DualFrontier.Core.Interop;

/// <summary>
/// Type-erased contract surface for a registered K9 field. The generic
/// concrete type is <see cref="FieldHandle{T}"/>; this interface exists so
/// <see cref="FieldRegistry"/> can hold heterogeneous handles in a single
/// dictionary without exposing the element type at the registry level.
/// </summary>
public interface IFieldHandle
{
    string Id { get; }
    int Width { get; }
    int Height { get; }
    Type ElementType { get; }
}

/// <summary>
/// Managed handle over a native <c>RawTileField</c> registered in
/// <see cref="NativeWorld"/>. Exposes point read/write, span access (via
/// <see cref="FieldSpanLease{T}"/>), conductivity map, per-cell storage
/// flag bitmap, and ping-pong buffer swap.
/// </summary>
/// <remarks>
/// All operations forward to the C ABI with the field id encoded as a
/// null-terminated UTF-8 byte sequence (stackalloc path; field ids are
/// typically short namespace identifiers like <c>"vanilla.magic.mana"</c>).
/// On a native return of 0 the call throws <see cref="FieldOperationFailedException"/>
/// — out-of-bounds, size mismatch, field-not-found, or mutation during
/// active span all surface the same way.
/// </remarks>
public sealed class FieldHandle<T> : IFieldHandle where T : unmanaged
{
    private readonly IntPtr _worldHandle;
    private readonly string _fieldId;
    private readonly int _width;
    private readonly int _height;

    internal FieldHandle(IntPtr worldHandle, string fieldId, int width, int height)
    {
        _worldHandle = worldHandle;
        _fieldId = fieldId;
        _width = width;
        _height = height;
    }

    public string Id => _fieldId;
    public int Width => _width;
    public int Height => _height;
    public Type ElementType => typeof(T);

    internal IntPtr WorldHandle => _worldHandle;

    public unsafe T ReadCell(int x, int y)
    {
        T value = default;
        int result;
        int byteCount = System.Text.Encoding.UTF8.GetByteCount(_fieldId);
        Span<byte> buffer = byteCount + 1 <= 256
            ? stackalloc byte[byteCount + 1]
            : new byte[byteCount + 1];
        System.Text.Encoding.UTF8.GetBytes(_fieldId, buffer.Slice(0, byteCount));
        buffer[byteCount] = 0;
        fixed (byte* idPtr = buffer)
        {
            result = NativeMethods.df_world_field_read_cell(
                _worldHandle, idPtr, x, y, &value, sizeof(T));
        }
        if (result != 1)
            throw new FieldOperationFailedException($"ReadCell({x},{y}) on '{_fieldId}' returned 0");
        return value;
    }

    public unsafe void WriteCell(int x, int y, T value)
    {
        int result;
        int byteCount = System.Text.Encoding.UTF8.GetByteCount(_fieldId);
        Span<byte> buffer = byteCount + 1 <= 256
            ? stackalloc byte[byteCount + 1]
            : new byte[byteCount + 1];
        System.Text.Encoding.UTF8.GetBytes(_fieldId, buffer.Slice(0, byteCount));
        buffer[byteCount] = 0;
        fixed (byte* idPtr = buffer)
        {
            result = NativeMethods.df_world_field_write_cell(
                _worldHandle, idPtr, x, y, &value, sizeof(T));
        }
        if (result != 1)
            throw new FieldOperationFailedException(
                $"WriteCell({x},{y}) on '{_fieldId}' returned 0 (out of bounds, size mismatch, or active span)");
    }

    public unsafe FieldSpanLease<T> AcquireSpan()
    {
        void* data = null;
        int w = 0, h = 0;
        int result;
        int byteCount = System.Text.Encoding.UTF8.GetByteCount(_fieldId);
        Span<byte> buffer = byteCount + 1 <= 256
            ? stackalloc byte[byteCount + 1]
            : new byte[byteCount + 1];
        System.Text.Encoding.UTF8.GetBytes(_fieldId, buffer.Slice(0, byteCount));
        buffer[byteCount] = 0;
        fixed (byte* idPtr = buffer)
        {
            result = NativeMethods.df_world_field_acquire_span(
                _worldHandle, idPtr, &data, &w, &h);
        }
        if (result != 1 || data == null)
            throw new FieldOperationFailedException($"AcquireSpan on '{_fieldId}' returned 0");
        return new FieldSpanLease<T>(_worldHandle, _fieldId, (T*)data, w, h);
    }

    public unsafe void SetConductivity(int x, int y, float value)
    {
        int result;
        int byteCount = System.Text.Encoding.UTF8.GetByteCount(_fieldId);
        Span<byte> buffer = byteCount + 1 <= 256
            ? stackalloc byte[byteCount + 1]
            : new byte[byteCount + 1];
        System.Text.Encoding.UTF8.GetBytes(_fieldId, buffer.Slice(0, byteCount));
        buffer[byteCount] = 0;
        fixed (byte* idPtr = buffer)
        {
            result = NativeMethods.df_world_field_set_conductivity(_worldHandle, idPtr, x, y, value);
        }
        if (result != 1)
            throw new FieldOperationFailedException($"SetConductivity({x},{y}) on '{_fieldId}' returned 0");
    }

    public unsafe float GetConductivity(int x, int y)
    {
        int byteCount = System.Text.Encoding.UTF8.GetByteCount(_fieldId);
        Span<byte> buffer = byteCount + 1 <= 256
            ? stackalloc byte[byteCount + 1]
            : new byte[byteCount + 1];
        System.Text.Encoding.UTF8.GetBytes(_fieldId, buffer.Slice(0, byteCount));
        buffer[byteCount] = 0;
        fixed (byte* idPtr = buffer)
        {
            return NativeMethods.df_world_field_get_conductivity(_worldHandle, idPtr, x, y);
        }
    }

    public unsafe void SetStorageFlag(int x, int y, bool enabled)
    {
        int result;
        int byteCount = System.Text.Encoding.UTF8.GetByteCount(_fieldId);
        Span<byte> buffer = byteCount + 1 <= 256
            ? stackalloc byte[byteCount + 1]
            : new byte[byteCount + 1];
        System.Text.Encoding.UTF8.GetBytes(_fieldId, buffer.Slice(0, byteCount));
        buffer[byteCount] = 0;
        fixed (byte* idPtr = buffer)
        {
            result = NativeMethods.df_world_field_set_storage_flag(
                _worldHandle, idPtr, x, y, enabled ? 1 : 0);
        }
        if (result != 1)
            throw new FieldOperationFailedException($"SetStorageFlag({x},{y}) on '{_fieldId}' returned 0");
    }

    public unsafe bool GetStorageFlag(int x, int y)
    {
        int byteCount = System.Text.Encoding.UTF8.GetByteCount(_fieldId);
        Span<byte> buffer = byteCount + 1 <= 256
            ? stackalloc byte[byteCount + 1]
            : new byte[byteCount + 1];
        System.Text.Encoding.UTF8.GetBytes(_fieldId, buffer.Slice(0, byteCount));
        buffer[byteCount] = 0;
        fixed (byte* idPtr = buffer)
        {
            return NativeMethods.df_world_field_get_storage_flag(_worldHandle, idPtr, x, y) == 1;
        }
    }

    public unsafe void SwapBuffers()
    {
        int result;
        int byteCount = System.Text.Encoding.UTF8.GetByteCount(_fieldId);
        Span<byte> buffer = byteCount + 1 <= 256
            ? stackalloc byte[byteCount + 1]
            : new byte[byteCount + 1];
        System.Text.Encoding.UTF8.GetBytes(_fieldId, buffer.Slice(0, byteCount));
        buffer[byteCount] = 0;
        fixed (byte* idPtr = buffer)
        {
            result = NativeMethods.df_world_field_swap_buffers(_worldHandle, idPtr);
        }
        if (result != 1)
            throw new FieldOperationFailedException($"SwapBuffers on '{_fieldId}' returned 0 (active span?)");
    }
}

/// <summary>
/// Stack-only lease of zero-copy read access to a field's primary buffer.
/// While at least one lease is active on the underlying field, mutations
/// (write_cell, set_conductivity, set_storage_flag, swap_buffers) are
/// rejected by the native side via <see cref="FieldOperationFailedException"/>.
/// </summary>
/// <remarks>
/// Ref struct: cannot be heap-allocated, captured by lambda, or cross an
/// async boundary. The <c>using</c> statement pattern is the canonical
/// consumer (see <c>IsotropicDiffusionKernel.Run</c>).
/// </remarks>
public unsafe ref struct FieldSpanLease<T> where T : unmanaged
{
    private readonly IntPtr _worldHandle;
    private readonly string _fieldId;
    private readonly T* _data;
    private readonly int _width;
    private readonly int _height;
    private bool _released;

    internal FieldSpanLease(IntPtr worldHandle, string fieldId, T* data, int width, int height)
    {
        _worldHandle = worldHandle;
        _fieldId = fieldId;
        _data = data;
        _width = width;
        _height = height;
        _released = false;
    }

    public ReadOnlySpan<T> Span => new(_data, _width * _height);
    public int Width => _width;
    public int Height => _height;

    public T this[int x, int y]
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => _data[y * _width + x];
    }

    public void Dispose()
    {
        if (_released) return;
        _released = true;
        int byteCount = System.Text.Encoding.UTF8.GetByteCount(_fieldId);
        Span<byte> buffer = byteCount + 1 <= 256
            ? stackalloc byte[byteCount + 1]
            : new byte[byteCount + 1];
        System.Text.Encoding.UTF8.GetBytes(_fieldId, buffer.Slice(0, byteCount));
        buffer[byteCount] = 0;
        fixed (byte* idPtr = buffer)
        {
            NativeMethods.df_world_field_release_span(_worldHandle, idPtr);
        }
    }
}

/// <summary>
/// Surfaces a non-success return code from the K9 field C ABI. Causes
/// include out-of-bounds coordinates, size/type mismatch, field not
/// registered, and mutation attempted while a span is active.
/// </summary>
public sealed class FieldOperationFailedException : Exception
{
    public FieldOperationFailedException(string message) : base(message) { }
}
