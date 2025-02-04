Unity 2022.3.26f1

This project was created by Alex Bethke as part of a take-home technical test for a job interview. 
The assignment was to clone this game minus the 6x paths, river jump, and music/sfx: https://apps.apple.com/us/app/the-line/id880338977


- Project is configured for Portrait locking on iOS; Landscape was deemed out of scope and not addressed
- Project is configured to resize based on Portrait screen size with all game features scaling relationally
- Localization solutions were not implemented 
- Folder structure is using a specific naming schema. This is a style choice but also future proofs projects against the inclusion of plugins and asset store packages so the core project files stay grouped together and are quickly findable.
- GameDebugger singleton was added to provide a central panel for controlling which debugging features are active. For non-developers I would create a custom editor to control this.




Development Log
- Unity specific version installation
- project structure and initial UI setup
- setting up game assets, single block collision detection, and basic game loop to detect end of game
- Setup wall prefab and controller, setup starting wall layout, added road spawning detection, added stub for road spawning logic
- Refactored for object pooling, refactored for game resetting separate from game start, setup base screen class and added in fade UI element functionality for main menu and game over screen, refactored pause flag into game states, setup custom inspector script for getting all fadeable ui elements, wired in game over handling and game restarting, added split between UI interaction and gameplay interaction, setup debugging and related Utils, added score mechanic
- added death blink and best score tracking
- Created layout element supporting script to set values based on percentages, font matching, cleaned up style for game over screen
- Cleaned up font styling for main menu and game displays, fixed moved speed to better match the original game
- Code clean up
- Added score resetting when game restarts
- Reduced game over screen delay from 5 to 3 seconds
- Reduced screen fade in time from 0.5 to 0.25
- Fixed calculation to handle multiple screen resolutions
- Added code to update collider size based on screen calculations
- Code clean up
- Added invincible flag and wall segment handling
- Refactored road generation to add logic for tracking the current path connection and building straight pieces off of that, when a bend is created placeholder logic is used for an empty row and the path connection is randomly choosen from within that row
- Added lookup grid for how far a bend can go for any player reachable position on the play area
- Added code to force a straight path after a bend (temp)
- Iterated on road generation logic, adjusting implementation
- Wired in main menu 
- Fixed input issue on multiple resolutions
- Stopped main menu from being accessed while in game over state
- Moved some inline values to a shared constants class
- Restored instructions after game over
- Adjusted position of UI to stop overlap from iOS devices top bar
- Refactored instructions UI to scale and position based on screen resolution and wall height
- Created rounded corner panel backgrounds for UI elements and integrated into project with sprite 9-slicing
- Add player dynamic position, sizing, and collider updating
- Added precompiler flag to stop debug statements outputting when active
- Added calculation for value and code to stop input above a certain point on the screen
- Tweaked road generation code
- Code clean up
- Added power pickup to scene with code to resize it based on screen resolution
- Integrated power pickup into world scroll
- Added controller script to power pickup
- Added draft small powerup coroutine
- Wired in randomized of powerup on spawn
- Added player / powerup hit detection
- Added invincible power coroutine
- Added player blink vfx during invincible
- Sped up player blink
- Added power display
- Added power display message for invincible
- Added power countdown message for small
- Changed invincible duration to 6 seconds based on observation from reference game video
- Stopped power spawning while one is active
- Added code to restore player colour at end of invincible
- Changed powerup spawn range (tied to segment spawns) from 10..20 - 17..35
- Cleared score display on app launch, fixes placeholder text showing
- Removed reference grid
- Adjusted powerup display to use a set width
- Added code to position powerup display in relation to player's touch
- Added a shuffled array of data to represent road segments queued for generation
- Setup control variables for bends vs straights in segments queue
- Refactored current road generation code to disable version 1 logic and move to version 2 logic based on a shuffled queue
- Added road generation logic that  detects  when overlapping bend road segments are going to spawn and fixes them either with a straight at edge of screen or  with a simple bend in the direction we were last moving to stop the overlap
- Added dev cheat for 10x gameplay speed to test v2 road generation over time
- Changed cheat from 10x to 5x
- Added road generation logic that handles overlapping bends generally vs at edge of playfield
- Made powerup spawning range configurable via inspector
- Modified paths accessible from a given position to reduce maximum player movement from 5 blocks to 4
- Added code to stop bend spawning from randomly selected a straight outside of the special case handling
- Code clean up pass and final bug testing

Development Log 2 - Revisions based on feedback
- Refactored to move wall object pooling to its own script
- Fixed typo in name of WallSegment script
- Fixed game over handling to wait for user input before starting
- Added dev cheats for activating powers
- Reduced some duplicated calculations in favour of a pre-calculated value
- Disabled power pickup and reset position to default on game reset
- Refactored input handling to only allow input on the green bar
- Removed SharedReferences and refactored where needed to restore required references
- Fix bug with input limiting as it related to position of instructions on screen
- Refactored power up display to its own script; Refactored where needed
- Refactored to move player functionality to its own script; Refactored where needed
- Refactored score display to its own script; Refactored where needed
- Refactored GameController to move road generation to its own script; Refactored where needed
- Added a generic list to RoadBuilder to handle scrolling objects
- Refactored  GameController to move power up generation and handling to its own script; Refactored where needed
- Converted most of Constants values to a ScriptableObject, using a singleton reference to share between all objects; Refactored where needed
- Fully deprecated use of Constants.cs
- Code clean up pass
- Added CameraResizer script
- Converted Wall prefab from Images (UI) to SpriteRenderer; Refactored referencing where needed
- Refactored road generator and some CalculatedValues to work in world space
- Refactored player to world space
- Refactored power up to world space
- Fixed initial positioning issues with road generation
- Fixed calculation for valid input
- Fixed UI resizing in relation to other changes
- Restored player positioning
- Removed collider resizing code from Player
- Created interface for GameController
- Refactored all modules to work with game interface
- Added state check to stop over triggering of small power if you die while picking it up
- Fixed positioning of power up instructions
- Fixed positioning and spawning of power up


