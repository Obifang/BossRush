using UnityEngine;
using UnityEngine.InputSystem;
public class RebindSaveLoad : MonoBehaviour
{
    public InputActionAsset actions;

    private void OnEnable()
    {
        var rebinds = PlayerPrefs.GetString("rebinds");

        if (!string.IsNullOrEmpty(rebinds)) {
            actions.LoadBindingOverridesFromJson(rebinds);
        }
    }

    private void OnDisable()
    {
        var rebinds = actions.SaveBindingOverridesAsJson();
        PlayerPrefs.SetString("rebinds", rebinds);
    }
}
