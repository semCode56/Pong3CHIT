using Godot;
using System;
using System.Collections.Generic;

public partial class GameManager : Node
{
    static List<int> scores = new List<int>() { 0, 0 };

    public static void AddPointsForPlayer(int player, int score)
    {
            scores[player] += score;
        

    }

    public static int GetPointsForPlayer(int player)
    {
        return scores[player];
    }
}
