using UnityEngine;

public class PuzzleUI : MonoBehaviour
{
    public static PuzzleUI Instance;
    public GameObject panel; // assign the UI panel in inspector

    private HallwayObject currentObject; // the object the player is interacting with

    void Awake() => Instance = this;

    // shows UI and frees cursor
    public void Show(HallwayObject obj)
    {
        currentObject = obj;
        panel.SetActive(true);
        Cursor.lockState = CursorLockMode.None;
    }

    // hides UI and locks cursor back
    public void Hide()
    {
        panel.SetActive(false);
        Cursor.lockState = CursorLockMode.Locked;
    }

    // hooked to anomaly button in inspector
    public void OnAnomalyBtn() => currentObject.OnAnomalyChosen(true);

    // hooked to not anomaly button in inspector
    public void OnNotAnomalyBtn() => currentObject.OnAnomalyChosen(false);
}