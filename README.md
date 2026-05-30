# Virtual Horse Racing Simulator

A Unity-based virtual horse racing simulator that allows users to predetermine the win and place finish order before the race begins. The race dynamically adjusts horse speeds in real-time to deliver the specified outcome while maintaining a realistic, competitive-looking race.

## Overview

This project is a proof of concept for dynamically setting the finish order in a simulated horse race. Users input which horses should finish 1st (win) and 2nd (place) via the menu, and the race engine ensures those outcomes are achieved through real-time speed manipulation of the AI-controlled horses.

## How It Works

1. **Menu** - Select or input the desired win and place horses (1-6)
2. **Race** - Six AI horses race along a waypoint-based track
3. **Dynamic Adjustment** - The race controller adjusts horse speeds in real-time to ensure the predetermined horses cross the finish line in the correct order
4. **Result** - The finish order is displayed at the end of the race

## Key Features

- **Predetermined Outcomes** - Set win and place finishers before the race starts
- **Realistic Racing** - Dynamic speed adjustments create natural-looking competition
- **6-Horse Field** - Six uniquely textured horses with AI pathfinding
- **Waypoint Circuit** - Catmull-Rom spline interpolation for smooth track navigation
- **Camera System** - Automatic camera tracking of the race leader with a finish-line camera switch

## Tech Stack

- Unity (C#)
- NavMesh-based AI pathfinding
- Cinemachine camera system
- TextMesh Pro UI

## Project Structure

```
Assets/
├── Scripts/
│   ├── Menu_Controller.cs        # Menu UI and race initialization
│   └── System/
│       ├── RaceController.cs     # Core race logic and speed manipulation
│       ├── OpponentController.cs # AI horse movement and waypoint tracking
│       ├── WaypointCircuit.cs    # Track layout with spline interpolation
│       ├── ProgressTracker.cs    # Race progress calculation
│       ├── RaceManager.cs        # Race configuration
│       ├── SoundManager.cs       # Audio management
│       └── Statistics.cs         # Lap/race tracking
├── HorseJockey/                  # Horse models and demo scene
└── Menu.unity                    # Main menu scene
```
