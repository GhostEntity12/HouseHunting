//------------------------------------------------------------------------------
// <auto-generated>
//     This code was auto-generated by com.unity.inputsystem:InputActionCodeGenerator
//     version 1.4.4
//     from Assets/Input/PlayerInput.inputactions
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;

public partial class @PlayerInput : IInputActionCollection2, IDisposable
{
    public InputActionAsset asset { get; }
    public @PlayerInput()
    {
        asset = InputActionAsset.FromJson(@"{
    ""name"": ""PlayerInput"",
    ""maps"": [
        {
            ""name"": ""Forest"",
            ""id"": ""88fbae35-2339-47e1-bf2e-670c5074a3d0"",
            ""actions"": [
                {
                    ""name"": ""Movement"",
                    ""type"": ""Value"",
                    ""id"": ""41fc90b8-f290-412c-8e01-6f045cc51ed6"",
                    ""expectedControlType"": ""Vector2"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": true
                },
                {
                    ""name"": ""Look"",
                    ""type"": ""Value"",
                    ""id"": ""4dc9a193-4b46-4e77-a803-bdaa09db786e"",
                    ""expectedControlType"": ""Vector2"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": true
                },
                {
                    ""name"": ""Shoot"",
                    ""type"": ""Button"",
                    ""id"": ""e9511adc-4a0a-4f3c-835b-2b89b425e10f"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""Interact"",
                    ""type"": ""Button"",
                    ""id"": ""f8907868-8ff6-4002-8967-96d1f6f64d90"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""Crouch"",
                    ""type"": ""Button"",
                    ""id"": ""92bbdb1f-9de3-4754-83b4-0835b6658d9b"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                }
            ],
            ""bindings"": [
                {
                    ""name"": ""WASD"",
                    ""id"": ""4169e3c3-5f6f-4e5a-ad0e-81a2417e3a38"",
                    ""path"": ""2DVector"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Movement"",
                    ""isComposite"": true,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""up"",
                    ""id"": ""f06fb747-9da4-4f0a-92a4-bbf6299d20c7"",
                    ""path"": ""<Keyboard>/w"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Movement"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""down"",
                    ""id"": ""6dbc9dc4-e39a-4c1f-bd55-9c1e0470ad1d"",
                    ""path"": ""<Keyboard>/s"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Movement"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""left"",
                    ""id"": ""f8d78899-7872-4ddb-8e1d-ac8135251b23"",
                    ""path"": ""<Keyboard>/a"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Movement"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""right"",
                    ""id"": ""b3a6ee9f-a3e9-4f7d-b64e-e8a354add721"",
                    ""path"": ""<Keyboard>/d"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Movement"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": """",
                    ""id"": ""88701c8f-98e4-4efd-bde5-89174f0c22a6"",
                    ""path"": ""<Mouse>/delta"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Look"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""a8f2be82-5fa7-44d4-be3c-075a452ac005"",
                    ""path"": ""<Mouse>/leftButton"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Shoot"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""c3912e66-e20e-471c-b30e-e814beae9da5"",
                    ""path"": ""<Keyboard>/e"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Interact"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""88c3a650-5e4d-47a0-b7ad-57fa07d6f094"",
                    ""path"": ""<Keyboard>/leftShift"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Crouch"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                }
            ]
        },
        {
            ""name"": ""House"",
            ""id"": ""e2eb5588-af7f-430e-a97a-0efb99db963e"",
            ""actions"": [
                {
                    ""name"": ""Movement"",
                    ""type"": ""Value"",
                    ""id"": ""bbac8a3a-7485-46d2-8269-1c1ae3328873"",
                    ""expectedControlType"": ""Vector2"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": true
                },
                {
                    ""name"": ""Interact"",
                    ""type"": ""Button"",
                    ""id"": ""4acd3443-5c22-41f4-b310-457c0f2f7bc5"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""Look"",
                    ""type"": ""Value"",
                    ""id"": ""1b52ccde-94dc-4f4c-bfcd-951059b9f6fc"",
                    ""expectedControlType"": ""Vector2"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": true
                },
                {
                    ""name"": ""Decorate"",
                    ""type"": ""Button"",
                    ""id"": ""7e9746d0-f0cf-4e6c-9be9-4b73937d77a3"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""Crouch"",
                    ""type"": ""Button"",
                    ""id"": ""f644904a-704c-4b1c-9920-6caa545d9152"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                }
            ],
            ""bindings"": [
                {
                    ""name"": ""WASD"",
                    ""id"": ""23c829c8-ea79-4625-8a71-32aafcc69673"",
                    ""path"": ""2DVector"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Movement"",
                    ""isComposite"": true,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""up"",
                    ""id"": ""01d590b5-3dfb-454f-a378-fe342e31a1b8"",
                    ""path"": ""<Keyboard>/w"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Movement"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""down"",
                    ""id"": ""cecfc51d-a5a7-4dd6-a6df-7127505060ff"",
                    ""path"": ""<Keyboard>/s"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Movement"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""left"",
                    ""id"": ""b773c027-bec8-43d8-9bb2-532daa1343fd"",
                    ""path"": ""<Keyboard>/a"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Movement"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""right"",
                    ""id"": ""95a1acac-6f66-4f95-bceb-5dd270c2b026"",
                    ""path"": ""<Keyboard>/d"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Movement"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": """",
                    ""id"": ""40d42b41-163b-4085-a326-8937fb016af3"",
                    ""path"": ""<Mouse>/delta"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Look"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""22b841ec-5fa7-4698-b4cc-a16c48ac7c86"",
                    ""path"": ""<Keyboard>/e"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Interact"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""139e4998-57d7-4c49-9cfb-119bff9f3499"",
                    ""path"": ""<Keyboard>/r"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Decorate"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""a0b47c94-3485-46e6-a38c-5e653de1e61e"",
                    ""path"": ""<Keyboard>/leftShift"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Crouch"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                }
            ]
        },
        {
            ""name"": ""Decorate"",
            ""id"": ""31ef6493-b5d7-4519-a93e-3d786b0e6a9b"",
            ""actions"": [
                {
                    ""name"": ""ExitToHouse"",
                    ""type"": ""Button"",
                    ""id"": ""81912909-1cc6-4ea0-a8ad-c9c82b2b9649"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""MouseDown"",
                    ""type"": ""Button"",
                    ""id"": ""68631c52-b9ad-4bc0-8217-da1ad3ade2ed"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""MouseMove"",
                    ""type"": ""Value"",
                    ""id"": ""d1f17d3b-3d07-4fc5-9693-9babf1d39fd7"",
                    ""expectedControlType"": ""Vector2"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": true
                },
                {
                    ""name"": ""MoveCamera"",
                    ""type"": ""Value"",
                    ""id"": ""5f187f97-6da1-4864-917f-3fb0233554c9"",
                    ""expectedControlType"": ""Vector2"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": true
                }
            ],
            ""bindings"": [
                {
                    ""name"": """",
                    ""id"": ""8851e204-249b-478f-8f9f-c73f4784d1d6"",
                    ""path"": ""<Keyboard>/r"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""ExitToHouse"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""8d90880f-051e-4f93-ab82-74055d02211c"",
                    ""path"": ""<Mouse>/leftButton"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""MouseDown"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""e8b35563-b439-40b8-894d-5b34997c7b3a"",
                    ""path"": ""<Mouse>/delta"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""MouseMove"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""WASD"",
                    ""id"": ""433a2608-6b16-46b1-a3ad-f786a713d323"",
                    ""path"": ""2DVector"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""MoveCamera"",
                    ""isComposite"": true,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""up"",
                    ""id"": ""5edcd2f4-8084-48ee-8e24-329189621939"",
                    ""path"": ""<Keyboard>/w"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""MoveCamera"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""down"",
                    ""id"": ""6410b02f-7a78-4ea3-be21-42277cb961fe"",
                    ""path"": ""<Keyboard>/s"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""MoveCamera"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""left"",
                    ""id"": ""33c94c81-34b2-436e-9741-b6c63d842cc3"",
                    ""path"": ""<Keyboard>/a"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""MoveCamera"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""right"",
                    ""id"": ""0ff1fc5d-3c98-4959-b49d-43b9724a16c5"",
                    ""path"": ""<Keyboard>/d"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""MoveCamera"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                }
            ]
        }
    ],
    ""controlSchemes"": []
}");
        // Forest
        m_Forest = asset.FindActionMap("Forest", throwIfNotFound: true);
        m_Forest_Movement = m_Forest.FindAction("Movement", throwIfNotFound: true);
        m_Forest_Look = m_Forest.FindAction("Look", throwIfNotFound: true);
        m_Forest_Shoot = m_Forest.FindAction("Shoot", throwIfNotFound: true);
        m_Forest_Interact = m_Forest.FindAction("Interact", throwIfNotFound: true);
        m_Forest_Crouch = m_Forest.FindAction("Crouch", throwIfNotFound: true);
        // House
        m_House = asset.FindActionMap("House", throwIfNotFound: true);
        m_House_Movement = m_House.FindAction("Movement", throwIfNotFound: true);
        m_House_Interact = m_House.FindAction("Interact", throwIfNotFound: true);
        m_House_Look = m_House.FindAction("Look", throwIfNotFound: true);
        m_House_Decorate = m_House.FindAction("Decorate", throwIfNotFound: true);
        m_House_Crouch = m_House.FindAction("Crouch", throwIfNotFound: true);
        // Decorate
        m_Decorate = asset.FindActionMap("Decorate", throwIfNotFound: true);
        m_Decorate_ExitToHouse = m_Decorate.FindAction("ExitToHouse", throwIfNotFound: true);
        m_Decorate_MouseDown = m_Decorate.FindAction("MouseDown", throwIfNotFound: true);
        m_Decorate_MouseMove = m_Decorate.FindAction("MouseMove", throwIfNotFound: true);
        m_Decorate_MoveCamera = m_Decorate.FindAction("MoveCamera", throwIfNotFound: true);
    }

    public void Dispose()
    {
        UnityEngine.Object.Destroy(asset);
    }

    public InputBinding? bindingMask
    {
        get => asset.bindingMask;
        set => asset.bindingMask = value;
    }

    public ReadOnlyArray<InputDevice>? devices
    {
        get => asset.devices;
        set => asset.devices = value;
    }

    public ReadOnlyArray<InputControlScheme> controlSchemes => asset.controlSchemes;

    public bool Contains(InputAction action)
    {
        return asset.Contains(action);
    }

    public IEnumerator<InputAction> GetEnumerator()
    {
        return asset.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    public void Enable()
    {
        asset.Enable();
    }

    public void Disable()
    {
        asset.Disable();
    }
    public IEnumerable<InputBinding> bindings => asset.bindings;

    public InputAction FindAction(string actionNameOrId, bool throwIfNotFound = false)
    {
        return asset.FindAction(actionNameOrId, throwIfNotFound);
    }
    public int FindBinding(InputBinding bindingMask, out InputAction action)
    {
        return asset.FindBinding(bindingMask, out action);
    }

    // Forest
    private readonly InputActionMap m_Forest;
    private IForestActions m_ForestActionsCallbackInterface;
    private readonly InputAction m_Forest_Movement;
    private readonly InputAction m_Forest_Look;
    private readonly InputAction m_Forest_Shoot;
    private readonly InputAction m_Forest_Interact;
    private readonly InputAction m_Forest_Crouch;
    public struct ForestActions
    {
        private @PlayerInput m_Wrapper;
        public ForestActions(@PlayerInput wrapper) { m_Wrapper = wrapper; }
        public InputAction @Movement => m_Wrapper.m_Forest_Movement;
        public InputAction @Look => m_Wrapper.m_Forest_Look;
        public InputAction @Shoot => m_Wrapper.m_Forest_Shoot;
        public InputAction @Interact => m_Wrapper.m_Forest_Interact;
        public InputAction @Crouch => m_Wrapper.m_Forest_Crouch;
        public InputActionMap Get() { return m_Wrapper.m_Forest; }
        public void Enable() { Get().Enable(); }
        public void Disable() { Get().Disable(); }
        public bool enabled => Get().enabled;
        public static implicit operator InputActionMap(ForestActions set) { return set.Get(); }
        public void SetCallbacks(IForestActions instance)
        {
            if (m_Wrapper.m_ForestActionsCallbackInterface != null)
            {
                @Movement.started -= m_Wrapper.m_ForestActionsCallbackInterface.OnMovement;
                @Movement.performed -= m_Wrapper.m_ForestActionsCallbackInterface.OnMovement;
                @Movement.canceled -= m_Wrapper.m_ForestActionsCallbackInterface.OnMovement;
                @Look.started -= m_Wrapper.m_ForestActionsCallbackInterface.OnLook;
                @Look.performed -= m_Wrapper.m_ForestActionsCallbackInterface.OnLook;
                @Look.canceled -= m_Wrapper.m_ForestActionsCallbackInterface.OnLook;
                @Shoot.started -= m_Wrapper.m_ForestActionsCallbackInterface.OnShoot;
                @Shoot.performed -= m_Wrapper.m_ForestActionsCallbackInterface.OnShoot;
                @Shoot.canceled -= m_Wrapper.m_ForestActionsCallbackInterface.OnShoot;
                @Interact.started -= m_Wrapper.m_ForestActionsCallbackInterface.OnInteract;
                @Interact.performed -= m_Wrapper.m_ForestActionsCallbackInterface.OnInteract;
                @Interact.canceled -= m_Wrapper.m_ForestActionsCallbackInterface.OnInteract;
                @Crouch.started -= m_Wrapper.m_ForestActionsCallbackInterface.OnCrouch;
                @Crouch.performed -= m_Wrapper.m_ForestActionsCallbackInterface.OnCrouch;
                @Crouch.canceled -= m_Wrapper.m_ForestActionsCallbackInterface.OnCrouch;
            }
            m_Wrapper.m_ForestActionsCallbackInterface = instance;
            if (instance != null)
            {
                @Movement.started += instance.OnMovement;
                @Movement.performed += instance.OnMovement;
                @Movement.canceled += instance.OnMovement;
                @Look.started += instance.OnLook;
                @Look.performed += instance.OnLook;
                @Look.canceled += instance.OnLook;
                @Shoot.started += instance.OnShoot;
                @Shoot.performed += instance.OnShoot;
                @Shoot.canceled += instance.OnShoot;
                @Interact.started += instance.OnInteract;
                @Interact.performed += instance.OnInteract;
                @Interact.canceled += instance.OnInteract;
                @Crouch.started += instance.OnCrouch;
                @Crouch.performed += instance.OnCrouch;
                @Crouch.canceled += instance.OnCrouch;
            }
        }
    }
    public ForestActions @Forest => new ForestActions(this);

    // House
    private readonly InputActionMap m_House;
    private IHouseActions m_HouseActionsCallbackInterface;
    private readonly InputAction m_House_Movement;
    private readonly InputAction m_House_Interact;
    private readonly InputAction m_House_Look;
    private readonly InputAction m_House_Decorate;
    private readonly InputAction m_House_Crouch;
    public struct HouseActions
    {
        private @PlayerInput m_Wrapper;
        public HouseActions(@PlayerInput wrapper) { m_Wrapper = wrapper; }
        public InputAction @Movement => m_Wrapper.m_House_Movement;
        public InputAction @Interact => m_Wrapper.m_House_Interact;
        public InputAction @Look => m_Wrapper.m_House_Look;
        public InputAction @Decorate => m_Wrapper.m_House_Decorate;
        public InputAction @Crouch => m_Wrapper.m_House_Crouch;
        public InputActionMap Get() { return m_Wrapper.m_House; }
        public void Enable() { Get().Enable(); }
        public void Disable() { Get().Disable(); }
        public bool enabled => Get().enabled;
        public static implicit operator InputActionMap(HouseActions set) { return set.Get(); }
        public void SetCallbacks(IHouseActions instance)
        {
            if (m_Wrapper.m_HouseActionsCallbackInterface != null)
            {
                @Movement.started -= m_Wrapper.m_HouseActionsCallbackInterface.OnMovement;
                @Movement.performed -= m_Wrapper.m_HouseActionsCallbackInterface.OnMovement;
                @Movement.canceled -= m_Wrapper.m_HouseActionsCallbackInterface.OnMovement;
                @Interact.started -= m_Wrapper.m_HouseActionsCallbackInterface.OnInteract;
                @Interact.performed -= m_Wrapper.m_HouseActionsCallbackInterface.OnInteract;
                @Interact.canceled -= m_Wrapper.m_HouseActionsCallbackInterface.OnInteract;
                @Look.started -= m_Wrapper.m_HouseActionsCallbackInterface.OnLook;
                @Look.performed -= m_Wrapper.m_HouseActionsCallbackInterface.OnLook;
                @Look.canceled -= m_Wrapper.m_HouseActionsCallbackInterface.OnLook;
                @Decorate.started -= m_Wrapper.m_HouseActionsCallbackInterface.OnDecorate;
                @Decorate.performed -= m_Wrapper.m_HouseActionsCallbackInterface.OnDecorate;
                @Decorate.canceled -= m_Wrapper.m_HouseActionsCallbackInterface.OnDecorate;
                @Crouch.started -= m_Wrapper.m_HouseActionsCallbackInterface.OnCrouch;
                @Crouch.performed -= m_Wrapper.m_HouseActionsCallbackInterface.OnCrouch;
                @Crouch.canceled -= m_Wrapper.m_HouseActionsCallbackInterface.OnCrouch;
            }
            m_Wrapper.m_HouseActionsCallbackInterface = instance;
            if (instance != null)
            {
                @Movement.started += instance.OnMovement;
                @Movement.performed += instance.OnMovement;
                @Movement.canceled += instance.OnMovement;
                @Interact.started += instance.OnInteract;
                @Interact.performed += instance.OnInteract;
                @Interact.canceled += instance.OnInteract;
                @Look.started += instance.OnLook;
                @Look.performed += instance.OnLook;
                @Look.canceled += instance.OnLook;
                @Decorate.started += instance.OnDecorate;
                @Decorate.performed += instance.OnDecorate;
                @Decorate.canceled += instance.OnDecorate;
                @Crouch.started += instance.OnCrouch;
                @Crouch.performed += instance.OnCrouch;
                @Crouch.canceled += instance.OnCrouch;
            }
        }
    }
    public HouseActions @House => new HouseActions(this);

    // Decorate
    private readonly InputActionMap m_Decorate;
    private IDecorateActions m_DecorateActionsCallbackInterface;
    private readonly InputAction m_Decorate_ExitToHouse;
    private readonly InputAction m_Decorate_MouseDown;
    private readonly InputAction m_Decorate_MouseMove;
    private readonly InputAction m_Decorate_MoveCamera;
    public struct DecorateActions
    {
        private @PlayerInput m_Wrapper;
        public DecorateActions(@PlayerInput wrapper) { m_Wrapper = wrapper; }
        public InputAction @ExitToHouse => m_Wrapper.m_Decorate_ExitToHouse;
        public InputAction @MouseDown => m_Wrapper.m_Decorate_MouseDown;
        public InputAction @MouseMove => m_Wrapper.m_Decorate_MouseMove;
        public InputAction @MoveCamera => m_Wrapper.m_Decorate_MoveCamera;
        public InputActionMap Get() { return m_Wrapper.m_Decorate; }
        public void Enable() { Get().Enable(); }
        public void Disable() { Get().Disable(); }
        public bool enabled => Get().enabled;
        public static implicit operator InputActionMap(DecorateActions set) { return set.Get(); }
        public void SetCallbacks(IDecorateActions instance)
        {
            if (m_Wrapper.m_DecorateActionsCallbackInterface != null)
            {
                @ExitToHouse.started -= m_Wrapper.m_DecorateActionsCallbackInterface.OnExitToHouse;
                @ExitToHouse.performed -= m_Wrapper.m_DecorateActionsCallbackInterface.OnExitToHouse;
                @ExitToHouse.canceled -= m_Wrapper.m_DecorateActionsCallbackInterface.OnExitToHouse;
                @MouseDown.started -= m_Wrapper.m_DecorateActionsCallbackInterface.OnMouseDown;
                @MouseDown.performed -= m_Wrapper.m_DecorateActionsCallbackInterface.OnMouseDown;
                @MouseDown.canceled -= m_Wrapper.m_DecorateActionsCallbackInterface.OnMouseDown;
                @MouseMove.started -= m_Wrapper.m_DecorateActionsCallbackInterface.OnMouseMove;
                @MouseMove.performed -= m_Wrapper.m_DecorateActionsCallbackInterface.OnMouseMove;
                @MouseMove.canceled -= m_Wrapper.m_DecorateActionsCallbackInterface.OnMouseMove;
                @MoveCamera.started -= m_Wrapper.m_DecorateActionsCallbackInterface.OnMoveCamera;
                @MoveCamera.performed -= m_Wrapper.m_DecorateActionsCallbackInterface.OnMoveCamera;
                @MoveCamera.canceled -= m_Wrapper.m_DecorateActionsCallbackInterface.OnMoveCamera;
            }
            m_Wrapper.m_DecorateActionsCallbackInterface = instance;
            if (instance != null)
            {
                @ExitToHouse.started += instance.OnExitToHouse;
                @ExitToHouse.performed += instance.OnExitToHouse;
                @ExitToHouse.canceled += instance.OnExitToHouse;
                @MouseDown.started += instance.OnMouseDown;
                @MouseDown.performed += instance.OnMouseDown;
                @MouseDown.canceled += instance.OnMouseDown;
                @MouseMove.started += instance.OnMouseMove;
                @MouseMove.performed += instance.OnMouseMove;
                @MouseMove.canceled += instance.OnMouseMove;
                @MoveCamera.started += instance.OnMoveCamera;
                @MoveCamera.performed += instance.OnMoveCamera;
                @MoveCamera.canceled += instance.OnMoveCamera;
            }
        }
    }
    public DecorateActions @Decorate => new DecorateActions(this);
    public interface IForestActions
    {
        void OnMovement(InputAction.CallbackContext context);
        void OnLook(InputAction.CallbackContext context);
        void OnShoot(InputAction.CallbackContext context);
        void OnInteract(InputAction.CallbackContext context);
        void OnCrouch(InputAction.CallbackContext context);
    }
    public interface IHouseActions
    {
        void OnMovement(InputAction.CallbackContext context);
        void OnInteract(InputAction.CallbackContext context);
        void OnLook(InputAction.CallbackContext context);
        void OnDecorate(InputAction.CallbackContext context);
        void OnCrouch(InputAction.CallbackContext context);
    }
    public interface IDecorateActions
    {
        void OnExitToHouse(InputAction.CallbackContext context);
        void OnMouseDown(InputAction.CallbackContext context);
        void OnMouseMove(InputAction.CallbackContext context);
        void OnMoveCamera(InputAction.CallbackContext context);
    }
}
