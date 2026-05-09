cmake_minimum_required(VERSION 3.20)
project(DualFrontier.Core.Native LANGUAGES CXX)

set(CMAKE_CXX_STANDARD 20)
set(CMAKE_CXX_STANDARD_REQUIRED ON)
set(CMAKE_CXX_EXTENSIONS OFF)

if(NOT CMAKE_BUILD_TYPE)
    set(CMAKE_BUILD_TYPE Release CACHE STRING "Build type" FORCE)
endif()

if(CMAKE_CXX_COMPILER_ID MATCHES "GNU|Clang|AppleClang")
    add_compile_options(-Wall -Wextra -Wpedantic -fvisibility=hidden)
    add_compile_options($<$<CONFIG:Release>:-O3>)
elseif(MSVC)
    add_compile_options(/W4 /permissive-)
    add_compile_options($<$<CONFIG:Release>:/O2>)
endif()

set(DF_NATIVE_SOURCES
    src/world.cpp
    src/component_store.cpp
    src/capi.cpp
    src/bootstrap_graph.cpp
    src/thread_pool.cpp
    src/string_pool.cpp
    src/keyed_map.cpp
    src/composite.cpp
)

set(DF_NATIVE_HEADERS
    include/df_capi.h
    include/sparse_set.h
    include/component_store.h
    include/world.h
    include/entity_id.h
    include/bootstrap_graph.h
    include/thread_pool.h
    include/string_pool.h
    include/keyed_map.h
    include/composite.h
)

add_library(DualFrontier.Core.Native SHARED ${DF_NATIVE_SOURCES} ${DF_NATIVE_HEADERS})

target_include_directories(DualFrontier.Core.Native PUBLIC
    $<BUILD_INTERFACE:${CMAKE_CURRENT_SOURCE_DIR}/include>
)

target_compile_definitions(DualFrontier.Core.Native PRIVATE DF_NATIVE_BUILDING_DLL)

set_target_properties(DualFrontier.Core.Native PROPERTIES
    OUTPUT_NAME "DualFrontier.Core.Native"
    PREFIX ""
    POSITION_INDEPENDENT_CODE ON
)

option(DF_NATIVE_BUILD_SELFTEST "Build the native self-test executable" ON)
if(DF_NATIVE_BUILD_SELFTEST)
    # The selftest exercises C++ internals (BootstrapGraph, ThreadPool) that
    # are intentionally NOT exported via DF_API. Compile the sources directly
    # into the executable so it can link to non-exported symbols. This is
    # equivalent to a static-link but bypasses the need for an extra static
    # library target.
    add_executable(df_native_selftest test/selftest.cpp ${DF_NATIVE_SOURCES})
    target_include_directories(df_native_selftest PRIVATE include)
    # DF_NATIVE_BUILDING_DLL makes DF_API resolve to dllexport — harmless on
    # an .exe (the symbols are simply marked externally visible) and avoids
    # the dllimport stub that would otherwise expect a DLL.
    target_compile_definitions(df_native_selftest PRIVATE DF_NATIVE_BUILDING_DLL)
endif()
