using UnityEngine;
using UnityEngine.UI;

public class ToggleManager : MonoBehaviour
{
    public int variableValue = 0;

    public void OnToggleValueChanged(Toggle toggle, int value)
    {
        if (toggle.isOn)
        {
            variableValue = value;
            UpdateToggles(value);
        }
    }

    void UpdateToggles(int valueToExclude)
    {
        Toggle[] toggles = GetComponentsInChildren<Toggle>();

        foreach (Toggle toggle in toggles)
        {
            if (toggle.isOn && toggle.GetComponent<ToggleData>().value != valueToExclude)
            {
                toggle.isOn = false;
            }
        }
    }
}
