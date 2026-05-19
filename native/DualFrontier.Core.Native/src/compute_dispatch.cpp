#include "compute_dispatch.h"

namespace dualfrontier {

bool dispatch_compute_noop(const char* /*field_name*/,
                           uint32_t pipeline_id,
                           uint32_t /*x*/, uint32_t /*y*/, uint32_t /*z*/)
{
    // V0.B stub: accept any non-zero pipeline_id. V1+ will load the registered
    // pipeline, bind descriptor sets к the named K9 field, record VkCmdDispatch,
    // submit к async compute queue, и wait on a fence per К-L7 atomic-from-observer.
    return pipeline_id != 0;
}

}  // namespace dualfrontier
