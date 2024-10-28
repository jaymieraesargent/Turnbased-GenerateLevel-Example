# Turnbased-GenerateLevel-Example
 
Red Warlord vs Blue Baron
Genre: Turn Based, RPG
Project Summary
Your studio manager has decided the studio is going to begin working on an adaptation of a unique and obscure pair of RPG books which made up a standalone turn-based ‘FPS’ game: Combat heroes 1: White Warlord and Combat Heroes 1: Black Baron.
https://www.youtube.com/watch?v=8SiUnqMJEVU 
These two RPG books allowed two players to take on the role of either the Warlord or the Baron, and linked together to create a turn-based PvP experience.
Each page of the book represented a location within a small ‘dungeon’ of slightly overlapping 3x3 grids. On each page the player was shown a ‘First person’ view of the environment and a number of options: to turn left, right, 180 degrees, or to move forward/back. Each decision corresponded with a page number to turn to in the same style as Choose Your Own Adventure books.
The unique twist is how the books linked together. If either player ended up in a location facing a direction where they could see the opposing player, they would go to a different page. On this page they’d be shown the adversary instead of the normal empty area and would have options for combat.
 
The studio head isn’t intending to recreate the books 1:1. Instead they want to capture the ‘spirit’ of the original game with the same map layout and similar premise: You play a turn-based game against an opponent in a cramped grid-based environment from a first person perspective.
Gameplay Loops & Objectives
The main object is to defeat the AI controlled Blue Baron before it can defeat you.
While the studio head is considering how this could be made into a multiplayer experience, for now they want to focus on the single player aspect. As such they feel that in order to add some challenge to the game & complexity beyond the two characters simply hunting each other down and then fighting, the AI character should start out slightly stronger than the player.
This will incentivize the player to try to get to upgrades (Shrines, detailed below) quickly. This will both deny the upgrades to the AI, who will grab them if they happen to start a turn next to them, but won’t prioritize them. 
Map Layout, Design and Size 
As shown above the original game takes place in a map consisting of 14 cells total, in a formation which is essentially two overlapping 3x3 grids with the middle of each 3x3 blocked.
This limited map size in the original Combat Heroes 1 books was necessary as any additional cells required additional pages to the book. The books, with only 14 cells available, were 356 each – this included pages for character stats, rules to the game and combat tables. Nevertheless, increasing the size of the environment would exponentially increase the number of required pages.
This game will not have that problem. As such the studio head is happy for the game environment to be larger – so long as the layout maintains the restrictions that the original had. Namely:
-	The environment is designed in such a way that the characters will only need to be able to move north, south, east and west. No diagonal movement.
-	Likewise, characters can only see north, south, east and west. They cannot see ‘around’ corners. The size of the cells should be such that when a character is standing in the blue shaded cell in the diagram below, they cannot see anyone standing in the red shaded cell.
o	Restricting the Field of View for the player character can also help with this.
-	The ‘corridors’ should only be 1x cell wide. This avoids any issues with diagonal views or movement.
  
Above: The blue square denotes where a character could be standing, facing north. They cannot see anything in the centre of the red squares, as the walls of the cells left and right of them would block it. The second image demonstrates the theoretical viewcone in yellow, showing that anything in the centre of the red cells would be hidden.
One method for expanding the original layout would be to use 3x3 groups of cells with the middle cell blocked with walls. Each additional 3x3 group would overlap a corner of the other 3x3 groups.
   
   
Above: Examples of potential larger layouts.
Potential for Expanded Content
As well as a larger play area, the general ‘blueprint’ of this adaptation has the potential for more features. This could include:
-	Multiple enemies
-	A ‘meta progression’ system
-	More combat abilities, or different weapons with different stats
-	The ability to choose different characters (Play as either the Blue Baron or Red Warlord)
-	More than two characters to choose from (Green Sorcerer, Purple Samurai, etc, etc…)
Gameplay
Core Mechanics
Action Points
Both the Player, as the Red Warlord, and the AI as the Blue Baron, have 3 Action Points at the start of their turn.
The following actions for both the player and the AI will use action points:
-	Move (1 point)
-	Attack (Melee) (1 point)
-	Attack (Ranged) (2 point)
-	Healing Spell (2 point)
-	Interact (1 point)
Movement
The player and AI can perform the following movement actions:
-	Move forward one space
-	Turn left
-	Turn right
-	Turn 180 degrees
For the player each movement action is shown as a UI widget in the HUD which can be interacted with. Alternatively the player can use an HCI device to input a directional input to either turn or move forward/back.
Compass and Threat Indicator
Due to the claustrophobic nature of the level and the restricted view radius the player will be provided with two tools in their HUD: the Compass and the Threat Indicator
Compass
The Compass will simply show which direction the player is facing. 
It will also display the grid coordinates that the player is currently at, which will allow them to map out the level themselves while exploring, if they wish.
Threat Indicator
The Threat Indicator will be similar to the compass, but have eight ‘gems’ or ‘lights’ which can be lit up either red or yellow in one of the 8 directions when different conditions are met.
The red symbol will indicate that the player is being attacked from that direction. 
The Threat Indicator will activate when the AI performs an attack for the first time – whether it is successful or not – and remain lit up so long as the AI is in line of sight of the player, even if they aren’t looking at the AI directly. It will display red in these conditions and only use the north, south, west or east directions.
If the threat indicator is lit up and the player turns, the threat indicator will update to continue showing which direction the enemy is in.
If the player moves out of line of sight of the AI after the Threat Indicator is activated, the indicator will turn yellow. It will then update whenever the player moves or turns to indicate the direction that the player was last attacked from. The yellow indicator can use the diagonal (north-west, south-west, etc, etc) slots in the Threat Indicator

Combat
Both the player and the AI can attempt to do damage to each other by using either the Attack (Melee) action or the Attack (Ranged) action.
These can be selected by the player using the UI widgets for them in the HUD, or by pressing the relevant input on the player’s HCI device.
Each character (the Blue Baron and Red Warlord) has the following stats for calculating combat encounters:
Health Points (HP)
Attack stat
Block stat
Ranged stat
Dodge stat
Ammo stat
Healpot stat
Attack (Melee)
This attack will hit an opposing character (the Blue Baron or Red Warlord) in the adjacent square directly in front of the character using it.
The attack will generate a random number and add the attacking character’s Attack stat. 
This will be compared against the target’s Block stat. If the modified Attack stat is greater than the Block stat, the attack is successful and a value equal to the Attack stat minus a randomly generated number is subtracted from the target’s health.
Attack (Ranged)
This attack can only be used if the character has more than 0 Ammo. Each time they use it the Ammo is reduced by 1.
This attack will hit an opposing character in a square in front of the character using it, no matte how far away they are.
The attack will generate a random attack and add it t the Ranged stat. The result is compared to the target character’s Dodge stat. If the result is greater then the attack is successful. When successful the Ranged stat plus a randomly generated value is subtracted from the target’s health.
Healing
Each character starts with a limited amount of Healpot stats. 
On their turn a character can use action points to use the Healing action. This will subtract a Healpot stat and restore their health by a randomly generated amount, with a set minimum value. 
EG: 10 + (1 – 10).
Death
If the Red Warlord (the player character) is reduced to zero HP the player loses the game and is taken to the game over screen.
If the Blue Baron (AI character) is reduced to zero Hp the player wins the game and is taken to the victory screen.
Interaction
The player can interact with some objects in the environment which can provide a benefit.
-	Healpots: interacting with one of these increases the number of healpots the player has by 1
-	Ammo: interacting with one of these increases the number of ammunition the player has by 2
-	Shrine: interacting with one of these increases the HP, Attack, Ranged, Block or Dodge stat by between 5 and 10.
AI Navigation
The AI will navigate the environment using a pathfinding algorithm capable of determining the shortest path to a target location on a non-weighted graph grid of square cells.
The target location of the AI Agent will depend on the decision making state that they are currently in.
Since the game only allows character to move forward or turn the AI will need to have logic which determines whether it can move to the next cell on it’s path, or whether it needs to turn left/right before trying to move forward.
AI Decision Making
The AI Agent will need to be able to determine which of it’s available actions should be used on its turn.
The AI should use a state machine to differentiate between the behavior it can engage in. Within each state it will have a number of actions. Each time it can spend action points it will use an algorithm to assign each action it could perform a score, and then choose the action with the highest score (so long as it has enough action points, and/or healpots/ammo). 
Wander State
In this state the AI Agent does not know the player’s precise location.
It will choose a location somewhere within the environment at random. It will then follow the shortest path to reach the location.
When it arrives it will then choose a new location to go to and repeat the process.
While in this state if the AI spots the player or is attacked by them it will switch to the Combat state.
While in this state if the AI starts a turn in a cell adjacent to an interactable world object, it will turn towards the interactable, then use it, before resuming it’s pathfinding.
Combat
In this state the AI knows the player’s location, either from spotting them or being attacked.
If the AI cannot see the player in this state due to facing the wrong way, it will prioritize turning to face the player.
If the AI cannot see the player due to terrain being in the way, it will prioritize moving to a cell which has line of sight to the player (this can be achieved by just trying to move to the player’s location)
If the AI can see the player and is facing them it will prioritize attacking them with the ranged attack.
If the AI can see the player and is facing them but has no ammo it will prioritize getting into melee range. Once in melee range it will then perform the melee action.
Regardless of any other conditions if the AI is between 25% health and has Healpots remaining it will prioritize using a Healing action.

Specifications
Platform
The game will be developed using Unity 2022.3.14f1 on Windows 10.
The platforms the studio head intends to publish the game on are:
-	Windows 10 desktop (Steam, itch.io, GOG and Epic Games)
-	WebGL browser based (itch.io)
-	Mobile (Android/iOS)
Visuals
3D Models
List here the 3D models required for implementation, detailing the required textures and animations as well.
•	Character list
o	Red Warlord (Player Character)
	Shown when the player dies
o	Blue Baron (AI character)	•	Environment models
o	Barrel containing Arrows
o	Shrines (HP, Attack, Ranged, Block, Dodge boosts)
o	Shelf with Healpot
o	Walls, floors, ceiling of dungeon
o	Doors

User-Interface
The UI for the game will need to be setup to use a resolution of 1920x1080, and will need to scale with both the screen width and height.
In-Game Graphical User Interface (GUI)
•	UI panels containing interactive widgets for…
o	Movement and Interaction (forward, left turn, right turn, 180 degrees)
o	Combat (Attack (Melee), Attack (Ranged), Heal)
o	Opening the Options Menu
o	Opening/closing the Stats window
•	Compass
o	Shows the direction the player is facing, to help with navigation
•	Threat direction indicator
o	Shows the direction the player is being attacked from while the AI is in view and attacking, or shows the last spot the AI was known to be if the AI is no longer in view
•	HP, Healpot, Ammo indicators
Options Menu
-	Options Menu itself containing buttons to…
o	Resume game
o	Open Video/Audio settings
o	Open Save menu 
o	Open Load menu
o	Quit to main menu
-	Video / Audio settings
-	Save menu
-	Load menu
-	‘Are you sure?’ prompt for quitting to main menu
Game Over Menu
-	Try Again option
-	Quit to Main menu
o	‘Are you sure?’ prompt
Victory Menu
-	Play Again option
-	Quit to Main menu 
o	‘Are you sure?’ prompt
Main Menu
-	New Game menu with difficulty settings
-	Video/Audio settings
-	Load game menu
-	Credits
-	Quit game
o	‘Are you sure’ prompt
Audio
Describe the audio specifications here including audio assets and audio properties.
•	Main Menu 
o	UI sounds
o	Menu music
•	Gameplay
o	Ambient music
o	Footsteps (Player and AI)
o	Attack sounds
o	Injury sounds
o	Healing sound
o	Death sounds
o	UI widget interaction sounds
o	Options Menu interaction sounds
o	Game Over music
o	Victory music
Control Scheme
Game Mechanic	Keyboard Mapping	Controller Mapping
Move Forward	Up Arrow	D-Pad Up
Turn Left	Left Arrow	D-Pad Left
Turn Right	Right Arrow	D-Pad Right
180 Degree Turn	Down Arrow	D-Pad Down
Attack (Melee)	A Key	West Face Button
Attack (Ranged)	R Key	East Face Button
Heal	H Key	North Face Button
Interact	E Key	South Face Button
Open Stat Screen	I Key	Options Button
Open Options Menu	Esc Key	Start/Pause Button
Navigate Menu	Up/Down/Left/Right Arrows	D-Pad
Menu Button Selection	Enter Key	South Face Button
