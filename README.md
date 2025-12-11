# SmartTravelPlanner

## Overview

SmartTravelPlanner is a WPF desktop application built with .NET 10 that allows users to create a traveler, load a city map from a 
.txt file, plan the shortest route to a destination using Dijkstra-like logic, view the route and total distance, and save/load traveler data as JSON.

## Build

- **Prerequisites:** .NET 10 SDK on Windows  
- **Build:**    dotnet build Project.sln -c Release -m
- **Run:**      dotnet run --project SmartTravelPlanner.csproj -c Release

## Map file format

Each line: CityA-CityB,Distance
Example:
Kyiv-Lviv,540
Lviv-Warsaw,400
Warsaw-Berlin,520

## Features

- Create a traveler (name, current location)
- Load map from .txt file 
- Enter destination and Plan Route
- List of route cities and total distance label
- Save/Load traveler (.json)
- Clear Route and Exit buttons

## Diagrams

See diagrams in ../diagrams/ and linked below:
- Use Case: ../diagrams/usecase.png
- Activity: ../diagrams/activity.png
- Sequence: ../diagrams/sequence.png