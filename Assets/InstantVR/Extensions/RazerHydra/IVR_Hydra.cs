/* InstantVR Razer Hydra extension
 * Copyright (c) 2016 by Passer VR
 * author: Pascal Serrarens
 * email: support@passervr.com
 * version: 3.8.0
 * date: April 11, 2017
 * 
 * - included IVR_HYDRA define check
 */

using UnityEngine;
using System;

namespace IVR {
    [HelpURL("http://passervr.com/documentation/instantvr-extensions/razer-hydra/")]
    public class IVR_Hydra : IVR_Extension {
#if UNITY_STANDALONE_WIN && IVR_HYDRA
        void OnDestroy() {
            InstantVR ivr = this.GetComponent<InstantVR>();

            if (ivr != null) {
                IVR_HydraHand hydraLeftHand = ivr.leftHandTarget.GetComponent<IVR_HydraHand>();
                if (hydraLeftHand != null)
                    DestroyImmediate(hydraLeftHand);

                IVR_HydraHand hydraRightHand = ivr.rightHandTarget.GetComponent<IVR_HydraHand>();
                if (hydraRightHand != null)
                    DestroyImmediate(hydraRightHand);
            }
        }

        public override void StartExtension(InstantVR ivr) {
            base.StartExtension(ivr);

            InitHydra();
        }

        public override void UpdateExtension() {
            base.UpdateExtension();

            UpdateHydraData();
        }

        public class HydraController {
            internal HydraController() {
                m_Enabled = false;
                m_Docked = false;
                m_Hand = InstantVR.BodySide.Unknown;
                m_HandBind = InstantVR.BodySide.Unknown;
                m_Buttons = 0;
                m_ButtonsPrevious = 0;
                m_Trigger = 0.0f;
                m_JoystickX = 0.0f;
                m_JoystickY = 0.0f;
                m_Position.Set(0.0f, 0.0f, 0.0f);
                m_Rotation.Set(0.0f, 0.0f, 0.0f, 1.0f);
            }

            internal void Update(ref SixensePlugin.sixenseControllerData cd) {
                m_Docked = (cd.is_docked != 0);
                m_Hand = (InstantVR.BodySide)cd.which_hand;
                m_ButtonsPrevious = m_Buttons;
                m_Buttons = (HydraButtons)cd.buttons;
                m_Trigger = cd.trigger;
                m_JoystickX = cd.joystick_x;
                m_JoystickY = cd.joystick_y;
                m_Position.Set(cd.pos[0], cd.pos[1], cd.pos[2]);
                m_Rotation.Set(cd.rot_quat[0], cd.rot_quat[1], cd.rot_quat[2], cd.rot_quat[3]);
                if (m_Trigger > m_TriggerButtonThreshold) {
                    m_Buttons |= HydraButtons.TRIGGER;
                }
            }

            public InstantVR.BodySide BodySide { get { return ((m_Hand == InstantVR.BodySide.Unknown) ? m_HandBind : m_Hand); } }
            public Vector3 Position { get { return new Vector3(m_Position.x, m_Position.y, -m_Position.z); } }
            public Quaternion Rotation { get { return new Quaternion(-m_Rotation.x, -m_Rotation.y, m_Rotation.z, m_Rotation.w); } }
            public bool GetButton(HydraButtons button) {
                return ((button & m_Buttons) != 0);
            }
            public bool GetButtonDown(HydraButtons button) {
                return ((button & m_Buttons) != 0) && ((button & m_ButtonsPrevious) == 0);
            }

            public bool m_Enabled;
            public bool m_Docked;
            private InstantVR.BodySide m_Hand;
            internal InstantVR.BodySide m_HandBind;
            private HydraButtons m_Buttons;
            private HydraButtons m_ButtonsPrevious;
            public float m_Trigger;
            public const float DefaultTriggerButtonThreshold = 0.9f;
            private float m_TriggerButtonThreshold = DefaultTriggerButtonThreshold;
            public float m_JoystickX;
            public float m_JoystickY;
            private Vector3 m_Position;
            private Quaternion m_Rotation;
        }

        public enum HydraButtons {
            START = 1,
            ONE = 32,
            TWO = 64,
            THREE = 8,
            FOUR = 16,
            BUMPER = 128,
            JOYSTICK = 256,
            TRIGGER = 512,
        }

        public const uint MAX_CONTROLLERS = 2;
        private static HydraController[] m_Controllers = new HydraController[MAX_CONTROLLERS];

        [HideInInspector]
        private bool dllPresent = false;

        void InitHydra() {
            try {
                SixensePlugin.sixenseInit();
                for (int i = 0; i < MAX_CONTROLLERS; i++) {
                    m_Controllers[i] = new HydraController();
                }
                dllPresent = true;
            }
            catch (Exception) {
                Debug.LogWarning("Sixense DLL not present, please download DLL from the SixenseUnityPlugin in the Asset Store");
            }
        }

        public static bool ControllerManagerEnabled = true;
        private ControllerManagerState m_ControllerManagerState = ControllerManagerState.NONE;
        private enum ControllerManagerState {
            NONE,
            BIND_CONTROLLER_ONE,
            BIND_CONTROLLER_TWO,
        }

        void UpdateHydraData() {
            if (dllPresent) {
                uint numControllersBound = 0;
                uint numControllersEnabled = 0;
                SixensePlugin.sixenseControllerData cd = new SixensePlugin.sixenseControllerData();
                for (int i = 0; i < MAX_CONTROLLERS; i++) {
                    if (m_Controllers[i] != null) {
                        if (SixensePlugin.sixenseIsControllerEnabled(i) == 1) {
                            SixensePlugin.sixenseGetNewestData(i, ref cd);
                            m_Controllers[i].Update(ref cd);
                            m_Controllers[i].m_Enabled = true;
                            numControllersEnabled++;
                            if (ControllerManagerEnabled && (m_Controllers[i].BodySide != InstantVR.BodySide.Unknown)) {
                                numControllersBound++;
                            }
                        } else {
                            m_Controllers[i].m_Enabled = false;
                        }
                    }
                }

                if (ControllerManagerEnabled) {
                    if (numControllersEnabled < 2) {
                        m_ControllerManagerState = ControllerManagerState.NONE;
                    }

                    switch (m_ControllerManagerState) {
                        case ControllerManagerState.NONE:
                            if (SixensePlugin.sixenseIsBaseConnected(0) != 0 && (numControllersEnabled > 1)) {
                                if (numControllersBound == 0) {
                                    m_ControllerManagerState = ControllerManagerState.BIND_CONTROLLER_ONE;
                                } else if (numControllersBound == 1) {
                                    m_ControllerManagerState = ControllerManagerState.BIND_CONTROLLER_TWO;
                                }
                            }
                            break;

                        case ControllerManagerState.BIND_CONTROLLER_ONE:
                            if (numControllersBound > 0) {
                                m_ControllerManagerState = ControllerManagerState.BIND_CONTROLLER_TWO;
                            } else {
                                for (int i = 0; i < MAX_CONTROLLERS; i++) {
                                    if ((m_Controllers[i] != null) && m_Controllers[i].GetButtonDown(HydraButtons.TRIGGER) && (m_Controllers[i].BodySide == InstantVR.BodySide.Unknown)) {
                                        m_Controllers[i].m_HandBind = InstantVR.BodySide.Left;
                                        SixensePlugin.sixenseAutoEnableHemisphereTracking(i);
                                        m_ControllerManagerState = ControllerManagerState.BIND_CONTROLLER_TWO;
                                        break;
                                    }
                                }
                            }
                            break;

                        case ControllerManagerState.BIND_CONTROLLER_TWO:
                            if (numControllersBound > 1) {
                                m_ControllerManagerState = ControllerManagerState.NONE;
                            } else {
                                for (int i = 0; i < MAX_CONTROLLERS; i++) {
                                    if ((m_Controllers[i] != null) && m_Controllers[i].GetButtonDown(HydraButtons.TRIGGER) && (m_Controllers[i].BodySide == InstantVR.BodySide.Unknown)) {
                                        m_Controllers[i].m_HandBind = InstantVR.BodySide.Right;
                                        SixensePlugin.sixenseAutoEnableHemisphereTracking(i);
                                        m_ControllerManagerState = ControllerManagerState.NONE;
                                        break;
                                    }
                                }
                            }
                            break;

                    }
                }
            }
        }

        public HydraController GetController(InstantVR.BodySide bodySide) {
            for (int i = 0; i < MAX_CONTROLLERS; i++) {

                if ((m_Controllers[i] != null) && (m_Controllers[i].BodySide == bodySide)) {
                    return m_Controllers[i];
                }
            }

            return null;
        }
#endif
    }
}