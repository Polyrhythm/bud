﻿// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;
using UnityEngine.VR.WSA.Input;
using Pathfinding;

namespace HoloToolkit.Unity
{
    /// <summary>
    /// GestureManager creates a gesture recognizer and signs up for a tap gesture.
    /// When a tap gesture is detected, GestureManager uses GazeManager to find the game object.
    /// GestureManager then sends a message to that game object.
    /// </summary>
    [RequireComponent(typeof(GazeManager))]
    public partial class GestureManager : Singleton<GestureManager>
    {
        private const int GROUND_LAYER = 8;

        /// <summary>
        /// To select even when a hologram is not being gazed at,
        /// set the override focused object.
        /// If its null, then the gazed at object will be selected.
        /// </summary>
        public GameObject OverrideFocusedObject
        {
            get; set;
        }

        /// <summary>
        /// Gets the currently focused object, or null if none.
        /// </summary>
        public GameObject FocusedObject
        {
            get { return focusedObject; }
        }

        private GestureRecognizer gestureRecognizer;
        private GameObject focusedObject;

        void Awake()
        {
            DontDestroyOnLoad(transform.gameObject);
        }

        void Start()
        {
            // Create a new GestureRecognizer. Sign up for tapped events.
            gestureRecognizer = new GestureRecognizer();
            gestureRecognizer.SetRecognizableGestures(GestureSettings.Tap);

            gestureRecognizer.TappedEvent += GestureRecognizer_TappedEvent;

            // Start looking for gestures.
            gestureRecognizer.StartCapturingGestures();
        }

        private void InputHandler(InteractionSourceKind source, int tapCount, Ray headRay)
        {
            switch (GameController.Instance.state)
            {
                case GameController.GameStates.Interaction:
                    BroadcastMessage("OnInteract");
                    GameController.Instance.SetState(GameController.GameStates.Play);
                    break;

                case GameController.GameStates.Menu:
                    Debug.Log(focusedObject);
                    if (focusedObject.name == "StartButton")
                    {
                        GameController.Instance.LoadScene(1);
                    }

                    if (focusedObject.name == "ObjectCollisionToggle")
                    {
                        CollisionToggle.Instance.OnToggle();
                    }
                    break;

                case GameController.GameStates.Scanning:
                    GameController.Instance.StopScan();
                    break;

                case GameController.GameStates.PetSelect:
                    if (focusedObject.name == "PandaSelect")
                    {
                        GameController.Instance.PetSelect(GameController.PetTypes.Panda);
                    }

                    if (focusedObject.name == "nameSelect")
                    {
                        NameSelect.Instance.OnNameSelect();
                    }
                    break;

                case GameController.GameStates.PrePetSpawn:
                    if (focusedObject.name == "SurfacePlane(Clone)" &&
                        focusedObject.layer == GROUND_LAYER)
                    {
                        GameController.Instance.SpawnPet(headRay);
                    }
                    break;

                default:
                    if (focusedObject != null)
                    {
                        focusedObject.SendMessage("OnSelect");
                    }
                    break;
            }
        }

        private void GestureRecognizer_TappedEvent(InteractionSourceKind source, int tapCount, Ray headRay)
        {
            InputHandler(source, tapCount, headRay);           
        }

        void Update()
        {
            if (Input.GetMouseButtonDown(0))
            {
                Ray ray = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
                InputHandler(InteractionSourceKind.Controller, 1, ray);
            }
        }

        void LateUpdate()
        {
            GameObject oldFocusedObject = focusedObject;

            if (GazeManager.Instance.Hit &&
                OverrideFocusedObject == null &&
                GazeManager.Instance.HitInfo.collider != null)
            {
                // If gaze hits a hologram, set the focused object to that game object.
                // Also if the caller has not decided to override the focused object.
                focusedObject = GazeManager.Instance.HitInfo.collider.gameObject;
            }
            else
            {
                // If our gaze doesn't hit a hologram, set the focused object to null or override focused object.
                focusedObject = OverrideFocusedObject;
            }

            if (focusedObject != oldFocusedObject)
            {
                // If the currently focused object doesn't match the old focused object, cancel the current gesture.
                // Start looking for new gestures.  This is to prevent applying gestures from one hologram to another.
                gestureRecognizer.CancelGestures();
                gestureRecognizer.StartCapturingGestures();
            }
        }

        void OnDestroy()
        {
            gestureRecognizer.StopCapturingGestures();
            gestureRecognizer.TappedEvent -= GestureRecognizer_TappedEvent;
        }
    }
}