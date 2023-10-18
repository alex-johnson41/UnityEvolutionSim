using UnityEngine;
using TMPro;
using System.Collections.Generic;

public class InputFieldsController : MonoBehaviour
{
    [SerializeField] private List<TMP_InputField> inputFields;
    [SerializeField] private SimControllerCreator sim; 

    public void sendData()
    {
        Dictionary<string, string> inputTextDictionary = new Dictionary<string, string>();
        foreach (TMP_InputField inputField in inputFields)
        {
            inputTextDictionary[inputField.name] = inputField.text;
        }
        sim.saveInputs(inputTextDictionary);
    }
}