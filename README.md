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
 - Section 2 - Hello Rust (7/23/2022)
 - Section 3.1 - Nice Walls with Bitsets (7/23/2022)
 - Section 3.2 - Bloodstains (7/23/2022)
 - Section 3.3 - Particle Effects (7/23/2022)
 - Section 3.4 - Hunger Clock (7/23/2022)
 - Section 3.5 - Magic Mapping (7/23/2022)
 - *Section 3.6 - REX Paint Menu SKIPPED*
 - Section 3.7 - Simple Traps (7/23/2022)

In Progress
 - Nothing Yet

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
     - I'm not sure what caused this to happen, but the equipped entities query for the melee combat system would not work unless it was instantiated in the function using it for calculations. If the query was instantiated at system creation, it wasn't picking up any of the entities that it was supposed to find.
 - Section 3.6
   - Everything
     - Like the Post-Process Scanlines feature from before, loading the REX Paint image is something I may come back to later.
     - I already have the code accounting for proper spacing depending on which menu options are visible, so there's nothing left to accomplish for this section.