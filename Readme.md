# Welcome to Galaxy Gauntlet!

## What is this?

Galaxy Gauntlet is an emulation of the Chip's Challenge logic engine written in Godot. This game has support for loading native Chip's Challenge level files, as well as a client for the Archipelago randomizer.

## Setup

#### Cloning the repository

Currently this repo has no releases, and is offered only as a Godot project. The code can be pulled down by running this in your command line pointed at the folder you want the project to reside.

``git clone https://github.com/BinnAuto/Galaxy-Gauntlet.git``

The game can be played either through the Godot debugger itself, or by creating a Windows export using the existing export preset. The game can be exported for other platforms by setting up the appropriate export preset in the Godot project.

#### Importing levels

This game has the capability to load in C2M level files from Chip's Challenge 2, but it does not come with any levels by default. To start, run Galaxy Gauntlet from either the Godot editor or your exported .exe to create the necessary ``levels`` folder.

Once this folder is created, official Chip's Challenge level files can be obtained by downloading [Chip's Challenge from Steam](https://store.steampowered.com/app/346850/Chips_Challenge_1/) for free. Once Chip's Challenge is downloaded, browse the files in your computer by right clicking the game in your library and selecting Manage > Browse local files. In the Chip's Challenge 1 folder that appears, the level files are located at ``/data/games/cc1``, separated into groups of 20 levels. These c2m files can be copied into the ``levels`` folder in Galaxy Gauntlet.

## Archipelago
#### Files and implementation
An example settings YAML and an early APWorld for the game "Chip's Challenge Test" can be found in the ``archipelago`` folder. This AP World only contains logic for the first ten levels in Chip's Challenge. Be sure to read the Settings.yaml file as it contains explanations for the settings specific to this game.

There is no separate client. Galaxy Gauntlet allows you to connect to the Archipelago server directly from the title screen.

#### Generating custom AP Worlds
This repo contains a tool that can generate the Python code needed to make an AP World file. It is recommended that you understand how AP Worlds are created in order to use this tool correctly.

In the repo, build the ``GalaxyGauntlet.APWorldGen`` C# project. This will create the path ``GalaxyGauntlet.APWorldGen/bin/Debug/net8.0``. To avoid stepping through initial errors when running the executable, create a ``levels`` folder here and put in the C2M level files you want to include in the AP World. If done correctly, when you run the program you will be prompted to enter a game name for the AP, and then the author name. The generated Python code will be created in an ``apworldout`` folder, which can then be used in the [standard process to generate an AP World](https://github.com/ArchipelagoMW/Archipelago/blob/0a601afbf575a4660077304a18ecb521ff1886c4/docs/running%20from%20source.md).

**Note:** To prevent conflicts with the actual AP Worlds once they are released, the tool does not allow worlds with the game name of "Chip's Challenge" or "Chip's Challenge 2".
