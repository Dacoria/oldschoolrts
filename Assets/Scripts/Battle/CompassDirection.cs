using System;

public enum CompassDirection
{
	NORTH, 
	EAST, 
	SOUTH, 
	WEST,
	NORTH_WEST,
	SOUTH_WEST,
	NORTH_EAST,
	SOUTH_EAST
}

public static class CompassDirectionHelper
{
	public static int GetAngle(this CompassDirection direction) => direction switch
    {
        CompassDirection.NORTH => 0,
        CompassDirection.EAST => 90,
        CompassDirection.SOUTH => 180,
        CompassDirection.WEST => 270,
        CompassDirection.NORTH_WEST => 315,
        CompassDirection.SOUTH_WEST => 225,
        CompassDirection.NORTH_EAST => 45,
        CompassDirection.SOUTH_EAST => 135,
        _ => throw new NotImplementedException(),
    };

    public static CompassDirection TurnRight(this CompassDirection direction)
    {
        var nextAngle = (direction.GetAngle() + 45) % 360;
        foreach (CompassDirection dir in Enum.GetValues(typeof(CompassDirection)))
        {
            if (dir.GetAngle() == nextAngle)
            {
                return dir;
            }
        }

        throw new NotImplementedException();
    }

    public static CompassDirection TurnLeft(this CompassDirection direction)
    {
        foreach (CompassDirection dir in Enum.GetValues(typeof(CompassDirection)))
        {
            if(dir.TurnRight() == direction)
            {
                return dir;
            }
        }

        throw new NotImplementedException();
    }
}
