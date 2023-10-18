using UnityEngine;
using TMPro;

public class InputData : MonoBehaviour
{
    public TMP_InputField user_inputField;
    public string text {get; private set;}

    public void Start(){
        
    }

    public void SaveInput()
    {
        text = user_inputField.text.ToString();
    }
}