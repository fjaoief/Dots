using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class ToggleManager : MonoBehaviour
{
    public int toggleValue = 0;
    public List<Toggle> toggleButtons = new List<Toggle>();

    private void Start()
    {
        foreach (Toggle toggle in toggleButtons)
        {
            toggle.onValueChanged.AddListener(delegate { ToggleValueChanged(toggle); });
        }
    }

    void ToggleValueChanged(Toggle changedToggle)
    {
        if (changedToggle.isOn)
        {
            toggleValue = toggleButtons.IndexOf(changedToggle) + 1;

            foreach (Toggle toggle in toggleButtons)
            {
                if (toggle != changedToggle)
                {
                    toggle.isOn = false;
                }
            }
        }
        else
        {
            bool anyTogglesOn = false;
            foreach (Toggle toggle in toggleButtons)
            {
                if (toggle.isOn)
                {
                    anyTogglesOn = true;
                    break;
                }
            }

            if (!anyTogglesOn)
            {
                toggleValue = 0;
            }
        }
    }
}
