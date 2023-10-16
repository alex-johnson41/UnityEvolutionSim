using System;
using System.Collections.Generic;
using System.Linq;


public enum InputTypes{
    AGE,
    RANDOM,
    X_POSITION,
    Y_POSITION,
    // CLOSEST_X_BORDER,
    CLOSEST_Y_BORDER
}

public enum OutputTypes{
    MOVE_FORWARD,
    MOVE_X,
    MOVE_Y,
    MOVE_RANDOM
}

public enum MoveDirections{
    N  = 0b0001,
    S  = 0b0011,
    E  = 0b0100,
    W  = 0b1100,
    NE = 0b0101,
    SE = 0b0111,
    SW = 0b1111,
    NW = 0b1101,
}

public enum SurvivalConditions{
    RIGHT_SIDE,
    LEFT_SIDE,
}

public static class MoveDirectionsHelper{
    private static readonly Dictionary<(int x, int y), MoveDirections> DirectionLookup = new Dictionary<(int x, int y), MoveDirections>{
        {(1, 1), MoveDirections.NE},
        {(1, -1), MoveDirections.SE},
        {(-1, -1), MoveDirections.SW},
        {(-1, 1), MoveDirections.NW},
        {(1, 0), MoveDirections.E},
        {(-1, 0), MoveDirections.W},
        {(0, 1), MoveDirections.N},
        {(0, -1), MoveDirections.S},
    };

    public static MoveDirections getMoveDirection(int x, int y){
        if (DirectionLookup.TryGetValue((x, y), out var moveDirection)){
            return moveDirection;
        }
        throw new ArgumentException("Invalid coordinates (0, 0) do not map to any MoveDirection.");
    }

    public static (int x, int y) getCoordinates(MoveDirections moveDirection)
    {
        return DirectionLookup.First(kv => kv.Value == moveDirection).Key;
    }
}

