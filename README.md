# ExamplerinoUnitino - Content

1. /app : Unity application that showcases some basic features and how they are implemented
2. /cli : This is a javascript (horrible written, I know ;) ) utility server that provides some basic message sending and gameboard creation "logic"

# App

__WARNING: The three asset packs in this project ("PolygonXY" folders) are only licensed to me and so should never be used in any non-commercial nor commercial use, replace these if you want to publish this in any way.__

__Unity Version:__ 2020.3.25f , URP Workflow


The Unity app contains out of a simple SampleScene that connects via WebSocket to the cli tool and receives an InitMessage. Based on this InitMessage, 
the gameboard will be populated based on the send, randomly generated, data.
Please look up the "Prefab" system and what it does.
The scripts are documented and explain what each class does, 
you can find all scripts in the "app/Assets/Scripts" directory. 

For more questions, create an issue.


*Functionality of the client are the following:*
1. Event based Gameboard creation
2. Randomized asset placement on fixed positions in the tile prefab
3. Tile highlighting on mouse over
4. Tile onClick events that print the corresponding coordinates in the console
5. Moving player via waypoints based on MoveMessage events received via WebSocket
6. Post Processing (default off, use UI)
7. Absolute basic UI method hooking (toogle post-processing buttons)
8. Orbital Camera

## Installation

1. Install Unity via the standard installer at there homepage. 
2. In the Unity Hub, add the project (/app) to the list and open it. 
3. Go to the "Scenes" folder and open the "SampleScene".
   
<br/>

# Cli


__WARNING: This code is absolutely horrible, do not take this as an example for good javascript code in any way! 
This is quick and very dirty stuff for testing only!__

__Nodejs Version:__ v16.7.0

The cli is a basic command line tool to setup a websocket server the client can connect to. You can choose in the command line if you want to send p1 or p2 to a new location by switching with your arrow keys and pressing enter. The position is printed to the console and after a fake delay, the event is send to the client via WebSocket as MoveMessage.

## Installation
1. Check that the client is not running, start the server first. You can let it run even after closing the client.
2. Install nodejs version v16.7.x. 
3. Type "node --version" into your console of choice and check if the version number appears.
4. cd to the "/cli" directory and enter "npm install" to install the needed dependencies.
5. type "node index.js" to start the server

<br><br>

# Test your skill

If you feel confident enough, you can try to add these features to the game. These are just some briefly brought up ideas and not "real exercises" so feel free to modify them as you like.

## [C#] Implement a select and command functionality


Take the example "MockOnClickedEventManager" script and add an differentiation between click on a player and a tile. 

Then implement the functionality to select a character, track which character is selected.

Add the possibility to print to the console "Character Z will be send to coordinates X/Y" when first a player and second a tile is selected.

Equal to the "HighlightScript" add a highlight to the selected player.

## [C#] Move one by on instead of diagonal (characters)
Move only up and down on the tiles with your character

## [JS + C#] Random Position Server sided
Compute and send a random position in the init message instead of placing the heroes in random positions on client side
