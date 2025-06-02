using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class WeaponIcon : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI txtPower;

    public void SetPower(int power)
    {
        txtPower.text = power.ToString();
    }
}