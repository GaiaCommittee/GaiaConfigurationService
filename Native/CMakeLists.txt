#==============================
# Requirements
#==============================

cmake_minimum_required(VERSION 3.10)

#==============================
# Project Settings
#==============================

if (NOT PROJECT_DECLARED)
    project("Gaia Configuration Service" LANGUAGES CXX VERSION 0.8)
    set(PROJECT_DECLARED)
endif()

#==============================
# Project Modules
#==============================

add_subdirectory("GaiaConfigurationClient")
add_subdirectory("GaiaScrollBar")

if (WITH_TEST)
#    add_subdirectory("GaiaConfigurationClientTest")
endif()
