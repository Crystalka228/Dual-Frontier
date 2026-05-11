using System;
using System.Collections.Generic;

namespace DualFrontier.Core.Interop;

/// <summary>
/// Managed-side tracker for K9 field handles registered against a single
/// <see cref="NativeWorld"/>. The native side owns the storage (one
/// <c>RawTileField</c> per registered id); this registry owns the managed
/// wrappers and enforces type identity across re-registrations.
/// </summary>
/// <remarks>
/// Registration is idempotent on identical <c>(type, width, height)</c>:
/// repeated calls return the cached <see cref="FieldHandle{T}"/>. Conflicting
/// type or dimensions throw — same contract as the native side, surfaced at
/// the managed layer to give callers an actionable exception.
///
/// The lock guards the handle dictionary; the kernel is single-threaded per
/// K-L7 but the lock is cheap and removes a future migration step.
/// </remarks>
public sealed class FieldRegistry
{
    private readonly IntPtr _worldHandle;
    private readonly Dictionary<string, IFieldHandle> _handles = new();
    private readonly object _lock = new();

    internal FieldRegistry(IntPtr worldHandle)
    {
        _worldHandle = worldHandle;
    }

    public unsafe FieldHandle<T> Register<T>(string id, int width, int height) where T : unmanaged
    {
        if (string.IsNullOrEmpty(id))
            throw new ArgumentException("Field id must be non-empty", nameof(id));
        if (width <= 0 || height <= 0)
            throw new ArgumentException("Field dimensions must be positive");

        lock (_lock)
        {
            if (_handles.TryGetValue(id, out var existing))
            {
                if (existing is FieldHandle<T> typed
                    && typed.Width == width && typed.Height == height)
                {
                    return typed;
                }
                throw new InvalidOperationException(
                    $"Field '{id}' already registered with different type or dimensions");
            }

            int cellSize = sizeof(T);
            int result;
            int byteCount = System.Text.Encoding.UTF8.GetByteCount(id);
            Span<byte> buffer = byteCount + 1 <= 256
                ? stackalloc byte[byteCount + 1]
                : new byte[byteCount + 1];
            System.Text.Encoding.UTF8.GetBytes(id, buffer.Slice(0, byteCount));
            buffer[byteCount] = 0;
            fixed (byte* idPtr = buffer)
            {
                result = NativeMethods.df_world_register_field(_worldHandle, idPtr, width, height, cellSize);
            }
            if (result != 1)
                throw new InvalidOperationException($"Native registration failed for field '{id}'");

            var handle = new FieldHandle<T>(_worldHandle, id, width, height);
            _handles[id] = handle;
            return handle;
        }
    }

    public FieldHandle<T> Get<T>(string id) where T : unmanaged
    {
        lock (_lock)
        {
            if (!_handles.TryGetValue(id, out var existing))
                throw new InvalidOperationException($"Field '{id}' is not registered");
            if (existing is not FieldHandle<T> typed)
                throw new InvalidOperationException(
                    $"Field '{id}' is registered with element type {existing.ElementType.Name}, not {typeof(T).Name}");
            return typed;
        }
    }

    public bool TryGet<T>(string id, out FieldHandle<T>? handle) where T : unmanaged
    {
        lock (_lock)
        {
            handle = null;
            if (!_handles.TryGetValue(id, out var existing)) return false;
            if (existing is not FieldHandle<T> typed) return false;
            handle = typed;
            return true;
        }
    }

    public bool IsRegistered(string id)
    {
        lock (_lock) return _handles.ContainsKey(id);
    }

    public unsafe void Unregister(string id)
    {
        lock (_lock)
        {
            if (!_handles.Remove(id)) return;
            int byteCount = System.Text.Encoding.UTF8.GetByteCount(id);
            Span<byte> buffer = byteCount + 1 <= 256
                ? stackalloc byte[byteCount + 1]
                : new byte[byteCount + 1];
            System.Text.Encoding.UTF8.GetBytes(id, buffer.Slice(0, byteCount));
            buffer[byteCount] = 0;
            fixed (byte* idPtr = buffer)
            {
                NativeMethods.df_world_field_unregister(_worldHandle, idPtr);
            }
        }
    }

    public int Count
    {
        get { lock (_lock) return _handles.Count; }
    }
}
