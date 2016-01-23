# Tronsformers
also known as...<h1>(End Of Line)</h1>
<hr/>
This project was done with two other people. This game was written using C# .NET/XAML in a WPF program and interfacing with an Xbox360 Kinect. The game itself is two bikes (much like the game Tron) (at least one user controlled) moving about the screen. the controls for left and right are the appropriate leaning of the user infront of the kinect.

The game plays itself, and when a person walks into frame, a timer starts. The timer triggers an old arcade screen to appear where the user must "place" their play token into the machines token slot. From there, the player (or two user-players if two people entered frame) pick a colour for their respective bike. After a timer, the game launches. It is played until both players have crashed.

<h2>Overview</h2>
The design for this project can be broken into two main components, the Game Play, Game Setup, and the User Control. This document intends to outline the designs for each of those parts of the project.
<h3>Part 1: Game Play</h3>
The game has players battling a head to head battle, trying to outlast the other player. The player’s marker will move forward until either it has crashed, the other player has crashed, or the player has signaled a change of direction. The both characters will be controlled by a single thread, reviving direction information from other threads through some IPC mechanisms.
<h3>Part 2: Game Setup</h3>
Before the game starts, the system will need to detect when there are two players ready for a match to begin. We intend on having a gesture that can be used to signal the system that the user is ready, and provide the user with the option of picking a colour for their marker.
<h3>Part 3: User Control</h3>
The players will have control of their marker by using a Microsoft Kinect. The Kinect will send skeleton detail for the closest two players. This data will be processed to determine if a user has leaned to the left or the right. If the system detects that a player has leaned to the side, it will signal the game to turn their player marker in to the left or the right depending on which way the player had leaned.
