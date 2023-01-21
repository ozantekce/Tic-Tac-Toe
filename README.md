# Tic-Tac-Toe
 
https://ozantekce.itch.io/tic-tac-toe

This project is using minimax tree. A player is labeled X and AI is labeled O. The game value is 1 if the player wins the game, and the game value is -1 if the AI wins the game, otherwise, the game is a draw and the value is 0.

A tree node is labeled as max or min. A child node will be labeled min if its parent is max or labeled max if its parent is min. Only leaves can have value directly. This value is calculated as above. Other nodes will select a value from children. For example, if a node has 2 children and this node is a max, child1's value is 0, and child2's value is 1 it will select child2's value so its value will be 1.

I use string to represent game status, for example, "NNNNNNNNN" represents the initial state. If the player selects row 0 and column 1, the string will be "NXNNNNNNN". We can create 9 different strings from the initial state string and these strings can create 72 different strings. It goes like this but there are not 362.880 (9!)  unique game states because some different paths create the same strings also some strings represent the finished game and we don't need to create new strings from them for example "XXXOONNO". We have 6046 unique strings so we need 6046 different nodes for these strings.

I create these nodes once and calculated these nodes' values once in the Configurations method. This method creates nodes and saves them in _idNodePairs dictionary. We can create the current board's string easily and we can get the node that represents this status from the dictionary. AI checks this node's children if there are any children with value -1 it will select it if not it will select a child node with value 0.
