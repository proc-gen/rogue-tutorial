# rogue-tutorial
Attempting to follow the Rust Roguelike tutorial (https://bfnightly.bracketproductions.com/) with C#

This project is being built using .NET 6.0, so Visual Studio 2022 or newer will be needed to compile.

Libraries In Use:
 - SadConsole V9 (http://sadconsole.com/)
   - Using the MonoGame template. This is my first try using SadConsole and the features seem pretty good, although tutorials for V9 still seem a little low in number.
 - SimpleECS (https://github.com/PeteyChan/SimpleECS)
   - Trying to find something to replicate SPECS entirely isn't something I wanted to take the time to do. This seemed to be a good start, although I may need to add things as I go along. For example, there's no defined implementation of a System, so I made my own that takes a query in the constructor and has a Run function. My only true negative is that I had to include the code directly as it wasn't available as a Nuget package.
 
Completed:
 - Section 2.0 - Hello World (7/1/2022)
 - Section 2.1 - Entities and Components (7/2/2022)
 
In Progress
 - Section 2.2 - Walking a Map
