# rogue-tutorial
Attempting to follow the Rust Roguelike tutorial (https://bfnightly.bracketproductions.com/) with C#

This project is being built using .NET 6.0, so Visual Studio 2022 or newer will be needed to compile.

Libraries In Use:
 - SadConsole V9 (http://sadconsole.com/, https://github.com/Thraka/SadConsole)
   - Using the MonoGame template. This is my first try using SadConsole and the features seem pretty good, although tutorials for V9 still seem a little low in number.
 - SimpleECS (https://github.com/PeteyChan/SimpleECS)
   - Trying to find something to replicate SPECS entirely isn't something I wanted to take the time to do. This seemed to be a good start, although I may need to add things as I go along. For example, there's no defined implementation of a System, so I made my own that takes a query in the constructor and has a Run function. My only true negative is that I had to include the code directly as it wasn't available as a Nuget package.
 - RogueSharp (https://github.com/FaronBracy/RogueSharp)
   - I finally ran into a point where I needed something like RLTK in order to do the FOV calculations from section 2.4. Between this and GoRogue, this seemed like it would be easier to implement without a lot of refactoring for the maps. I'm sure that I'll find other uses for it in the future.
 
Completed:
 - Section 2.0 - Hello World (7/1/2022)
 - Section 2.1 - Entities and Components (7/2/2022)
 - Section 2.2 - Walking a Map (7/2/2022)
 - Section 2.3 - A More Interesting Map (7/2/2022)
 - Section 2.4 - Field of View (7/3/2022)
 - Section 2.5 - Monsters (7/4/2022)
 - Section 2.6 - Dealing Damage (7/5/2022)
 - Section 2.7 - User Interface (7/6/2022)
 - Section 2.8 - Items and Inventory (7/7/2022)
 - Section 2.9 - Ranged Scrolls/Targeting (7/15/2022)
 - Section 2.10 - Saving and Loading (7/17/2022)
 - Section 2.11 - Delving Deeper (7/18/2022)
 - Section 2.12 - Difficulty (7/19/2022)
 - Section 2.13 - Equipment (7/22/2022)

In Progress
 - Minor refactoring and bug fixes

Other Notes
 - Section 2.7 - User Interface
   - Post-Process Scanlines
     - This feature isn't something necessary to completing the tutorial as a whole and marked as optional by the author. I may revisit this at another point in time after completing all the required parts.
 - Section 2.9 - Ranged Scrolls/Targeting
   - Targeting System
     - SadConsole has Update, Render, ProcessKeyboard, and ProcessMouse all as separate functions, so the targeting was not as simple as the Rust version. It needed an additional RunState so that the GUI could be rendered properly. Consumables also needed a field to keep track if they'd been used in case the player chose to cancel out of using the scroll/item.
 - Section 2.10 - Saving and Loading
   - SimpleECS Limitations
     - This section would have been very straightforward if the Entity struct provided by SimpleECS was easily serializable. Of course, this wasn't the case and meant either finding a new library after writing so much code or writing my own Save/Load code. I opted for the latter because it was more tedious than anything else and not computationally difficult. 
 - Section 2.13 - Equipment
   - SimpleECS Oddities
     - I'm not sure what caused this to happen, but the equipped entities query for the melee combat system would not work unless it was instantiated in the function using it for calculations. If the query was instantiated at system creation, it wasn't picking up any of the entities that it was supposed to find.'