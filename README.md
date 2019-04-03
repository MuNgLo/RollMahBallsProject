# RollMahBallsProject

This is to mess around with and learn, brush off on some ways of doing things.

# Goals..

WiP: Make a plugin with a fairly generic random maze generator with room support. Oh and multithreaded generation.
So far there is a room generation pass and then a fill pass that generates a valid maze pattern except for connectivity. That is next thing to take a look at.

Maybe: I'll maybe use this to also have a look back on multiplayer in Unity. Should be a lot there that has changed.


# Done so far..
Player ball that you roll with torque forces and fully integrated in the physics engine.
Decent camera that doesn't clip into walls.
Models and prefabs to build the maze with.

Feel free to do whatever you want with this. I won't be using anything but my own code and models.*

Oh and lastly I did put this together over at https://www.twitch.tv/munglo

*With one exception. The PRNGMarsenneTwister class. It isn't mine and has its own license stuff inside it. Its function here is to make the random generation act the same with same seed regardless of system, platform and so on.