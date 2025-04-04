# Uno Flip Remix

**Team Members:**  
Kevin Ishak, Zain-Alabedeen Garada, Mingyang Xu, Jianhao Wei  
**Project Duration:** September 8, 2024 – April 2, 2025  

---

## Project Overview

Uno Flip Remix is a digital adaptation of the official **UNO Flip®** card game. This game introduces a twist on the classic UNO format by including **two-sided cards** (light and dark sides), **flip mechanics**, and **new action cards** that dramatically change gameplay.

Our goal was to design and implement a **real-time, multiplayer-ready** digital game that replicates the physical gameplay experience while enhancing it through:
- Automated rule enforcement
- Animated visuals
- Multiplayer over TCP
- Score tracking
- Game state synchronization

---

## 📁 Repository Structure

```text
UNO-Flip-3D/
├── docs/
│   ├── SRS-Volere/                # Software Requirements Specification (SRS)
│   ├── Design/
│   │   ├── SoftArchitecture/      # Module Guide (MG) and UML diagrams
│   │   ├── SoftDetailedDes/       # Module Interface Specification (MIS)
│   ├── VnVPlan/                   # Verification & Validation Plan
|   ├── VnVReport/                 # Verification & Validation Report
│   ├── HazardAnalysis/           # Game safety and failure modes documentation
│   ├── DevelopmentPlan/          # Gantt chart, schedule, and team roles
│   ├── ProblemStatementAndGoals/    # Problem Statement and Goal of the project
│   ├── Presentation/             # Final project expo poster and demo presentations
│
├── refs/                         # Reference material (e.g., UNO Flip rules, research)
│
├── UNOFlip/                      # Game source code
│   ├── Assets/                   # Unity assets (sprites, UI elements, etc.)
│   ├── Scripts/                  # Core game logic and components
│   ├── Scenes/                   # Unity scenes for game states
│   └── Testing/                  # Unity test scripts and test scenes
│
├── Network UNO Card Game TCP Server/
│   ├── Server.py                 # TCP server to manage multiplayer games
│   └── utils.py                  # Helper scripts for managing sessions, rooms, etc.
│
└── README.md                     # You're here!
