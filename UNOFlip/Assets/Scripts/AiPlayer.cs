using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AiPlayer : Player
{
    public AiPlayer(string name) : base(name, false)
    {

    }

    //AI LOGIC
    public override void TakeTurn()
    {
        Debug.Log("AI TURN");
    }
}