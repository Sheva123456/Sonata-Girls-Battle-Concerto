using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class RhythmInput : MonoBehaviour
{
    static readonly (RhythmLane lane, string binding)[] Bindings =
    {
        (RhythmLane.AttackUp, "<Keyboard>/w"),
        (RhythmLane.AttackLeft, "<Keyboard>/a"),
        (RhythmLane.AttackDown, "<Keyboard>/s"),
        (RhythmLane.AttackRight, "<Keyboard>/d"),
        (RhythmLane.DefenseUp, "<Keyboard>/upArrow"),
        (RhythmLane.DefenseLeft, "<Keyboard>/leftArrow"),
        (RhythmLane.DefenseDown, "<Keyboard>/downArrow"),
        (RhythmLane.DefenseRight, "<Keyboard>/rightArrow"),
    };

    [SerializeField] InputActionAsset actionAsset;

    readonly List<RhythmLane> pressed = new List<RhythmLane>();
    InputActionMap map;
    bool ownsMap;

    public IReadOnlyList<RhythmLane> PressedThisFrame => pressed;

    void Awake()
    {
        InputActionMap assetMap = null;
        if (actionAsset != null)
            assetMap = actionAsset.FindActionMap("Rhythm", false);

        if (assetMap != null)
        {
            map = assetMap;
        }
        else
        {
            map = new InputActionMap("Rhythm");
            foreach (var (lane, binding) in Bindings)
                map.AddAction(lane.ToString(), InputActionType.Button, binding);
            ownsMap = true;
        }

        map.Enable();
    }

    void Update()
    {
        pressed.Clear();
        foreach (var (lane, _) in Bindings)
        {
            var action = map.FindAction(lane.ToString(), false);
            if (action != null && action.WasPressedThisFrame())
                pressed.Add(lane);
        }
    }

    void OnDestroy()
    {
        if (map == null)
            return;

        map.Disable();
        if (ownsMap)
            map.Dispose();
    }
}
