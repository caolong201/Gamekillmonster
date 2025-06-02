using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerEndAttackEvent : MonoBehaviour
{
    private PlayerController parent;
    public void Init(PlayerController _parent)
    {
        this.parent = _parent;
    }

    //Animation event
    public void PlayerEndAttack()
    {
        parent.PlayerEndAttackEvent();
    }
}
