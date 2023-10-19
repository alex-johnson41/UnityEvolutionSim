using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;
using System.Linq;

public class SurvivalConditionDropdown : MonoBehaviour
{
    public TMP_Dropdown dropdown;
    void Start()
    {
        List<string> options = Enum.GetNames(typeof(SurvivalConditions)).ToList();
        dropdown.AddOptions(options);
    }
}
