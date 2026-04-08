# Therapeutic VR System — EMDR Backyard

**1st Place — UCSD XR Hackathon (March 2025)**

A therapeutic VR experience built for Meta Quest 3, engineering dynamic bilateral visual stimulation algorithms in support of EMDR-style exposure and grounding workflows. This repository focuses on the core C# systems that generate anxiety-reducing visual patterns and validate user head orientation, presented in a form you can adapt or extend in your own Unity project.

## Overview

EMDR Backyard explores how immersive environments and calm visual feedback can work together in VR. The highlighted logic includes:

* **User Orientation Validation** — Validates the headset's forward vector against the therapeutic zone to ensure the user's head remains stationary and facing forward while the visual algorithm runs.
* **Bilateral Visual Stimulation** — Smooth, sinusoidal left–right motion of a focal stimulus (e.g., a glowing sphere), with clean start/stop and return-to-center behavior.

These pieces are designed to integrate with Meta XR SDK and XR Interaction Toolkit workflows on Quest-class hardware.

## Tech Stack

| Area | Technology |
| :--- | :--- |
| **Runtime & tooling** | Unity |
| **Language** | C# |
| **VR platform** | Meta Quest (Meta XR SDK) |
| **Interaction** | XR Interaction Toolkit (recommended alongside this repo’s patterns) |

## Repository Contents

Scripts live under `Assets/Scripts/EMDR_VR/`:

* **`UserAlignment/OrientationValidator.cs`** — Vector-based orientation checks to ensure the user's field of view is locked onto the therapeutic focal area, with Unity / C# events for downstream systems.
* **`TherapeuticAlgorithms/BilateralStimulator.cs`** — Coroutine-driven oscillation (`Mathf.Sin`) with configurable width and speed, plus `ToggleStimulation(bool)` for smooth shutdown to center.
* **`TherapeuticAlgorithms/VisualPatternManager.cs`** — Environment color transitions toward calming palette targets via `Color.Lerp` and material property blocks.

## Getting Started

1. Open or create a Unity project targeting Android with Meta XR / OpenXR configured for Quest.
2. Import the Meta XR SDK and (as needed) XR Interaction Toolkit per Meta’s current documentation.
3. Copy the `EMDR_VR` scripts into your project’s Assets hierarchy.
4. Assign your center eye (or camera rig) transform and scene references in the Inspector.

*(Exact Unity Editor and package versions depend on your production pipeline; align them with your Meta XR SDK release notes.)*

## License & Acknowledgments

This project was recognized at the UCSD XR Hackathon (March 2025). If you reuse or adapt this code, please retain appropriate attribution and ensure any clinical or therapeutic use follows professional guidelines and institutional review.

## Contact

For collaboration or questions about the hackathon build, reach out via your preferred channel (GitHub Issues, email, or team links as you publish them).
