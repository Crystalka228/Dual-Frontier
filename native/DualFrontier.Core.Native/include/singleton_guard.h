#pragma once

#include <atomic>
#include <cstdint>

namespace dualfrontier {

// F-29(a) — distinct negative return code signalling a DETECTED concurrent-entry
// violation of a process-global singleton's single-threaded mutation/compute
// contract. Append-only and additive: distinct from the scheduler graph's
// 1 (success) / 0 (generic failure) / -1 (write conflict) / -2 (cycle). Surfaced
// managed-side as SystemGraphInterop.ComputeResult.ConcurrencyViolation.
inline constexpr int32_t kConcurrencyViolation = -3;

// Fail-loud single-thread guard for a process-global singleton whose design
// contract is single-threaded registration / mutation / compute. (Concurrent
// reads of already-computed, immutable state remain safe and are NOT guarded --
// only the state-touching mutation/compute entry points are.)
//
// Acquire-or-fail: the constructor performs one atomic exchange (false -> true).
// If another thread is already inside a guarded entry point, acquired() is false
// and the caller MUST bail out WITHOUT touching any shared state -- returning
// kConcurrencyViolation (or false / a no-op for bool / void entry points)
// instead of proceeding. This converts the silent heap corruption of a contract
// violation into an immediate, visible rejection.
//
// It is a DETECTOR, not a lock: it never blocks and never serialises legitimate
// sequential cross-thread use (register on a load thread, tick on another thread
// later is fine); it only rejects genuinely CONCURRENT entry. The single atomic
// exchange is negligible even on the per-tick path.
class SingletonGuard {
public:
    explicit SingletonGuard(std::atomic<bool>& flag) noexcept
        : flag_(flag),
          acquired_(!flag.exchange(true, std::memory_order_acquire)) {}

    ~SingletonGuard() {
        if (acquired_) {
            flag_.store(false, std::memory_order_release);
        }
    }

    SingletonGuard(const SingletonGuard&) = delete;
    SingletonGuard& operator=(const SingletonGuard&) = delete;

    [[nodiscard]] bool acquired() const noexcept { return acquired_; }

private:
    std::atomic<bool>& flag_;
    bool               acquired_;
};

}  // namespace dualfrontier
