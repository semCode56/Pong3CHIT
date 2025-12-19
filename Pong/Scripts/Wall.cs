using Godot;
using System;

public partial class Wall : Area2D
{
    public enum WallTypeEnum
    {
        TopBottomWall,
        LeftRightWall
    }

    [Export]
    WallTypeEnum walltype;

    public void OnWallArea2DEntered(Area2D area)
    {
        if (area is Ball ball)
        {
            ball.Bounce(walltype == WallTypeEnum.TopBottomWall);
        }
    }

}
