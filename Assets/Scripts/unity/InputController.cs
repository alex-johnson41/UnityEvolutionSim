using UnityEngine;
using TMPro;
using System.Collections.Generic;

public class InputFieldsController : MonoBehaviour
{
    [SerializeField] private List<TMP_InputField> inputFields;
    [SerializeField] private List<TMP_Dropdown> dropdownInputs;
    [SerializeField] private SimControllerCreator sim; 

    public void sendData()
    {
        Dictionary<string, string> inputTextDictionary = new Dictionary<string, string>();
        foreach (TMP_InputField inputField in inputFields)
        {
            inputTextDictionary[inputField.name] = inputField.text;
        }
        foreach (TMP_Dropdown dropdownField in dropdownInputs){
            inputTextDictionary[dropdownField.name] = dropdownField.options[dropdownField.value].text;
        }
        sim.saveInputs(inputTextDictionary);
    }
}