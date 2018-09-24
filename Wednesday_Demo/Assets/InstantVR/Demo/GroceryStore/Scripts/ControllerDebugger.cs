using UnityEngine;
using System.Collections;

using IVR;

public class ControllerDebugger : MonoBehaviour {

    [System.Serializable]
    public struct ControllerSideDebugger {
        public float stickHorizontal;
        public float stickVertical;

        public bool stickButton;

        public bool up;
        public bool down;
        public bool left;
        public bool right;

        public bool[] buttons;

        public float trigger1;
        public float trigger2;

        public bool option;
    }

    public ControllerSideDebugger left;
    public ControllerSideDebugger right;

    private ControllerInput controller;

	// Use this for initialization
	void Start () {
        controller = Controllers.GetController(0);

        left.buttons = new bool[4];
        right.buttons = new bool[4];
	}
	
	// Update is called once per frame
	void Update () {
        UpdateSide(ref left, controller.left);
        UpdateSide(ref right, controller.right);
	}

    void UpdateSide(ref ControllerSideDebugger sideDebugger, ControllerInputSide controllerSide) {
        sideDebugger.stickHorizontal = controllerSide.stickHorizontal;
        sideDebugger.stickVertical = controllerSide.stickVertical;

        sideDebugger.stickButton = controllerSide.stickButton;

        sideDebugger.up = controllerSide.up;
        sideDebugger.down = controllerSide.down;
        sideDebugger.left = controllerSide.left;
        sideDebugger.right = controllerSide.right;

        sideDebugger.buttons[0] = controllerSide.buttons[0];
        sideDebugger.buttons[1] = controllerSide.buttons[1];
        sideDebugger.buttons[2] = controllerSide.buttons[2];
        sideDebugger.buttons[3] = controllerSide.buttons[3];

        sideDebugger.trigger1 = controllerSide.trigger1;
        sideDebugger.trigger2 = controllerSide.trigger2;

        sideDebugger.option = controllerSide.option;
    }
}
