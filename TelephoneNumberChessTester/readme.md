# Dynamic Chess Engine for Financial Asset Management Interview

## Overview
This repository contains a dynamic chess engine developed as part of a take-home programming interview for a position at a financial alternative asset manager. The engine is designed with extensibility and flexibility in mind, using a JSON configuration to dynamically define chess pieces, board layouts, and moves. This approach allows for adjustments to the game setup at runtime without the need to alter the code or restart the application.

## Original Instructions
The original PDF with detailed instructions and guidelines for this project can be found [here](./Chess.pdf).

## Features
- **Dynamic Configuration:** Chess board, pieces, and game moves can be configured dynamically using a JSON file. This makes the system adaptable and extensible to other types of games and scenarios.
- **Extensible Design:** The design supports various game types, board sizes, and piece types, enabling easy expansion and customization.
- **Test Console Application:** Includes a console application for testing the functionality of the chess engine.

## Results
The engine calculates the number of possible telephone numbers per chess piece based on their movements across the board:
- Pawn: 0
- Rook: 5034
- Knight: 952
- Bishop: 864
- Queen: 135495
- King: 864
- Eldar (Custom Piece): 120

## JSON Configuration
The JSON configuration file plays a critical role in defining the dynamics of the chess engine. It allows for the flexible specification of board layouts, piece movements, and game rules without altering the underlying codebase. Here's a breakdown of the key components:

- **Board:** Defines the layout of the chess board as a grid, where each array element represents a row.
- **Exclusions:** Sets conditions for excluding certain numbers from being valid moves, such as numbers containing specific characters or starting with certain digits.
- **Terminators:** Defines conditions under which a series of moves should terminate.
- **Pieces:** Specifies the types of pieces available and their respective movement patterns.

### Example Configuration:
```json
{
  "board": [
    [ "1", "2", "3" ],
    [ "4", "5", "6" ],
    [ "7", "8", "9" ],
    [ "*", "0", "#" ]
  ],
  "exclusions": [
    "p => p.Contains(\"*\") || p.Contains(\"#\")", // Exclude numbers containing * or #
    "p => p.StartsWith(\"0\") || p.StartsWith(\"1\")" // Exclude numbers starting with 0 or 1
  ],
  "terminators": [ "p => p.Length == 7" ],
  "pieces": [
    {
      "name": "Pawn",
      "moves": [ "u" ]
    },
    {
      "name": "Rook",
      "moves": [ "U", "D", "R", "L" ]
    },
    {
      "name": "Knight",
      "moves": [ "uur", "uul", "llu", "lld", "rru", "rrd", "ddl", "ddr" ]
    },
    {
      "name": "Bishop",
      "moves": [ "Q", "W", "S", "A" ]
    },
    {
      "name": "Queen",
      "moves": [ "U", "D", "R", "L", "Q", "W", "S", "A" ]
    },
    {
      "name": "King",
      "moves": [ "q", "w", "s", "a" ]
    },
    {
      "name": "Eldar",
      "moves": [ "ll", "rr" ]
    }
  ]
}
```

## Movement Mechanics

The chess engine uses a combination of lowercase and uppercase letters to define the movement patterns of pieces. Each piece has a set of valid moves represented in the JSON configuration.

### Move Definitions
- **Lowercase Letters (`u`, `d`, `r`, `l`, `q`, `w`, `s`, `a`):** These letters represent single-step movements in specific directions:
  - `u`: Up (North)
  - `d`: Down (South)
  - `r`: Right (East)
  - `l`: Left (West)
  - `q`: Upper-left diagonal (Northwest)
  - `w`: Upper-right diagonal (Northeast)
  - `s`: Lower-right diagonal (Southeast)
  - `a`: Lower-left diagonal (Southwest)

### Uppercase Letters (`U`, `D`, `R`, `L`, `Q`, `W`, `S`, `A`)
- Uppercase versions of the movement letters indicate the piece can move in the specified direction continuously until it reaches the edge of the board or encounters an obstacle. This is typically used for pieces like the Rook, Bishop, and Queen which can move multiple squares in a single move.

### Code Implementation
The `NextMoves` function in the engine determines possible next positions for a piece based on its current position and move patterns:

```csharp
public IEnumerable<IBoardPosition> NextMoves(IBoardPiece piece, IBoardPosition currentBoardPosition)
{
    foreach (string movePattern in piece.Moves)
    {
        int newX = currentBoardPosition.X;
        int newY = currentBoardPosition.Y;
        foreach ((int dx, int dy) in piece.GetValidMoves(movePattern))
        {
            newX = newX + dx;
            newY = newY + dy;
            if (movePattern.ToUpper() == movePattern) // uppercase, iterate over each change
            {
                // Check if the new position is within the board bounds
                if (newX >= 0 && newX < Rows && newY >= 0 && newY < Cols)
                {
                    yield return new BoardPosition(this, newX, newY);
                }
            }
        }
        if (movePattern.ToUpper() != movePattern) // lowercase, keep the last move
        {
            // Check if the new position is within the board bounds
            if (newX >= 0 && newX < Rows && newY >= 0 && newY < Cols)
            {
                yield return new BoardPosition(this, newX, newY);
            }
        }
    }
}
```

## The moveMap defines the letter movements.

```csharp
private readonly Dictionary<char, (int, int)> moveMap = new Dictionary<char, (int, int)>
{
    {'u', (+1, 0)},
    {'d', (-1, 0)},
    {'r', (0, +1)},
    {'l', (0, -1)},
    {'q', (+1, -1)},
    {'w', (+1, +1)},
    {'s', (-1, +1)},
    {'a', (-1, -1)}
};
```

## Disclaimer
Please note that the numerical results generated by this engine have not been independently verified for accuracy. While the engine has been tested using the accompanying console application, it does not include unit tests.

## Usage
Instructions on how to configure and use the chess engine are included in the JSON configuration file within this repository. Users can modify this file to test different configurations and scenarios without altering the core application.
