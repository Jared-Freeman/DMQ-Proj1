// GENERATED AUTOMATICALLY FROM 'Assets/Ryan Assets/Resources/PlayerControls.inputactions'

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;

public class @PlayerControls : IInputActionCollection, IDisposable
{
    public InputActionAsset asset { get; }
    public @PlayerControls()
    {
        asset = InputActionAsset.FromJson(@"{
    ""name"": ""PlayerControls"",
    ""maps"": [
        {
            ""name"": ""Gamepad"",
            ""id"": ""0cd07d5a-cfe1-4ba2-955a-8d7883ac4fe1"",
            ""actions"": [
                {
                    ""name"": ""Movement"",
                    ""type"": ""Value"",
                    ""id"": ""638fd9c8-cf64-49aa-b8c5-3a8a013219f6"",
                    ""expectedControlType"": """",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Attack"",
                    ""type"": ""Button"",
                    ""id"": ""5c5896be-a4a2-4d8b-b893-2b31f839ef76"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Jump"",
                    ""type"": ""Button"",
                    ""id"": ""8c15af6b-83cf-47b3-a7c7-be70e0dcca1b"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""SpecialAction"",
                    ""type"": ""Button"",
                    ""id"": ""7cdde358-5d0d-42d3-bece-bf66a3420315"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Wepon1Equip"",
                    ""type"": ""Button"",
                    ""id"": ""71437366-b9f9-4c6f-9914-f3f2f38fd393"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Wepon2Equip"",
                    ""type"": ""Button"",
                    ""id"": ""8a075c6b-2964-47c7-b5bf-54d3b4ae6eec"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Aim"",
                    ""type"": ""Value"",
                    ""id"": ""02866cb5-9d7b-45e0-a662-362ab5d7c798"",
                    ""expectedControlType"": ""Vector2"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Interact"",
                    ""type"": ""Button"",
                    ""id"": ""1b5e7d1d-8795-472e-ab89-f750e2fb86b1"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Ability1"",
                    ""type"": ""Button"",
                    ""id"": ""afc92a77-60af-4284-bdfe-6a69599f3197"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Ability2"",
                    ""type"": ""Button"",
                    ""id"": ""c26963c4-fec5-4c51-9b44-c092f3cb78bc"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Pause"",
                    ""type"": ""Button"",
                    ""id"": ""313fe730-6372-42c5-be7e-412b98d03097"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                }
            ],
            ""bindings"": [
                {
                    ""name"": """",
                    ""id"": ""cdb09d61-334f-46ae-8c5a-8db1d4bf5e60"",
                    ""path"": ""<Gamepad>/leftStick"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Gamepad"",
                    ""action"": ""Movement"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""fc1d4f66-5fe7-43c7-9417-67e0da2ef84f"",
                    ""path"": ""<Gamepad>/buttonEast"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Gamepad"",
                    ""action"": ""Jump"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""41a95257-e253-4864-974e-28fcd94ad8a9"",
                    ""path"": ""<Gamepad>/buttonSouth"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Gamepad"",
                    ""action"": ""SpecialAction"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""dfa53766-c22f-44cc-ab00-85709cc64d33"",
                    ""path"": ""<Gamepad>/dpad/up"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Gamepad"",
                    ""action"": ""Wepon1Equip"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""87eb0762-98e2-41a9-8826-c16a0cd0f2f2"",
                    ""path"": ""<Gamepad>/dpad/right"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Gamepad"",
                    ""action"": ""Wepon2Equip"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""364b6292-25af-45dc-80cf-30e8a2ddf83a"",
                    ""path"": ""<Gamepad>/rightTrigger"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Gamepad"",
                    ""action"": ""Attack"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""3b69bbcc-501e-4c25-bef2-5469ac915403"",
                    ""path"": ""<Gamepad>/rightStick"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Gamepad"",
                    ""action"": ""Aim"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""e209e6c9-728e-4ad1-a4ee-4e8fe26fea20"",
                    ""path"": ""<Gamepad>/buttonWest"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Gamepad"",
                    ""action"": ""Interact"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""eb2b8cdb-0c55-43bf-89bc-ebbbb8f90181"",
                    ""path"": ""<Gamepad>/leftTrigger"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Gamepad"",
                    ""action"": ""Ability1"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""b6453937-ecb2-424a-bce6-f32c311ee94e"",
                    ""path"": ""<Gamepad>/leftShoulder"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Gamepad"",
                    ""action"": ""Ability2"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""73f73108-d848-469e-81c4-3c49af3651f6"",
                    ""path"": ""<Gamepad>/start"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Gamepad"",
                    ""action"": ""Pause"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                }
            ]
        },
        {
            ""name"": ""MouseAndKeyboard"",
            ""id"": ""447c37c3-3213-4d8a-8fa5-b983762a9c03"",
            ""actions"": [
                {
                    ""name"": ""Movement"",
                    ""type"": ""Button"",
                    ""id"": ""2aa3d0fd-bdfd-4dfb-a5fa-8325558228fd"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""SpecialAction"",
                    ""type"": ""Button"",
                    ""id"": ""142af1e0-5842-4592-afcc-1ab3ff68fe39"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Jump"",
                    ""type"": ""Button"",
                    ""id"": ""7b9ce615-3bfc-4608-b1d8-8d4d3645e798"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Attack"",
                    ""type"": ""Button"",
                    ""id"": ""ab36fc5a-e598-4fc2-8e4a-4378c7c3be71"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Wepon2Equip"",
                    ""type"": ""Button"",
                    ""id"": ""01728c93-a3e7-4af0-98dc-8be8b7ee5616"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Wepon1Equip"",
                    ""type"": ""Button"",
                    ""id"": ""fddaf0f9-9659-4e82-9205-ac814015b4c0"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Aim"",
                    ""type"": ""Value"",
                    ""id"": ""9bdd143a-98db-4024-aa26-5013e3a36e5f"",
                    ""expectedControlType"": ""Vector2"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Interact"",
                    ""type"": ""Button"",
                    ""id"": ""e00d0f1a-26f7-4413-9a4f-4095c6b50243"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Ability1"",
                    ""type"": ""Button"",
                    ""id"": ""4cac9b5a-16f3-4174-ae88-e6edda1a394d"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Ability2"",
                    ""type"": ""Button"",
                    ""id"": ""62c3dc45-9d9b-4261-be4c-f975b00080ba"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Pause"",
                    ""type"": ""Button"",
                    ""id"": ""397a140e-eb16-4bc0-8790-a1839e439c62"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                }
            ],
            ""bindings"": [
                {
                    ""name"": ""WASD"",
                    ""id"": ""c1e1f2f8-4afc-45cb-b02f-a0c13294c654"",
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
                    ""id"": ""c3432a14-e5df-42ec-96ff-b16ed7f87bdc"",
                    ""path"": ""<Keyboard>/w"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""MouseAndKeyboard"",
                    ""action"": ""Movement"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""down"",
                    ""id"": ""b2abd864-7af9-4a71-8881-7fa82fe34dfd"",
                    ""path"": ""<Keyboard>/s"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""MouseAndKeyboard"",
                    ""action"": ""Movement"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""left"",
                    ""id"": ""2db42d36-6a33-4e46-b8bb-46f482dadb88"",
                    ""path"": ""<Keyboard>/a"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""MouseAndKeyboard"",
                    ""action"": ""Movement"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""right"",
                    ""id"": ""adff538c-8aef-4eb2-b8ab-5f0d8af52602"",
                    ""path"": ""<Keyboard>/d"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""MouseAndKeyboard"",
                    ""action"": ""Movement"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": """",
                    ""id"": ""c161ee2f-0c0b-49cb-a723-1baa44b7ac21"",
                    ""path"": ""<Mouse>/leftButton"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""MouseAndKeyboard"",
                    ""action"": ""Attack"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""b5f04dfe-9ec1-46f1-8074-c303865b1f5c"",
                    ""path"": ""<Keyboard>/space"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""MouseAndKeyboard"",
                    ""action"": ""Jump"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""6786fc1e-1fef-46ca-bc76-f8d5409aaada"",
                    ""path"": ""<Keyboard>/leftShift"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""MouseAndKeyboard"",
                    ""action"": ""SpecialAction"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""76fccebf-c64b-4841-87f1-9a51cef6956c"",
                    ""path"": ""<Keyboard>/1"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""MouseAndKeyboard"",
                    ""action"": ""Wepon1Equip"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""7e310859-c263-4e92-9002-b460f4061490"",
                    ""path"": ""<Keyboard>/2"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""MouseAndKeyboard"",
                    ""action"": ""Wepon2Equip"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""7ec9fbba-a1d2-446d-888f-75e445e59454"",
                    ""path"": ""<Mouse>/position"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""MouseAndKeyboard"",
                    ""action"": ""Aim"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""37f1aacf-62b9-4a51-ab26-c89443726e96"",
                    ""path"": ""<Keyboard>/f"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""MouseAndKeyboard"",
                    ""action"": ""Interact"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""5a3500fb-832d-4210-bdfa-1e1a08f88165"",
                    ""path"": ""<Keyboard>/e"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""MouseAndKeyboard"",
                    ""action"": ""Ability1"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""0aa4ce71-c2e7-4fcc-b654-32daf895693d"",
                    ""path"": ""<Keyboard>/r"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""MouseAndKeyboard"",
                    ""action"": ""Ability2"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""56b459b0-5ab5-4c52-bc5e-d4e84daaf74d"",
                    ""path"": ""<Keyboard>/escape"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""MouseAndKeyboard"",
                    ""action"": ""Pause"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                }
            ]
        },
        {
            ""name"": ""Player"",
            ""id"": ""f840efd3-0628-420f-a381-521012eb72b8"",
            ""actions"": [
                {
                    ""name"": ""Movement"",
                    ""type"": ""Value"",
                    ""id"": ""822c120e-aa06-476b-8b24-3f46fb2e2cc6"",
                    ""expectedControlType"": """",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Jump"",
                    ""type"": ""Button"",
                    ""id"": ""967d8b77-692d-4291-8190-13516d9a817b"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Attack"",
                    ""type"": ""Button"",
                    ""id"": ""a82f6022-213e-4432-b3bd-f3f958f5bd78"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""SpecialAction"",
                    ""type"": ""Button"",
                    ""id"": ""3670f9d0-6413-4e4b-a47f-c5f605b16346"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Wepon1Equip"",
                    ""type"": ""Button"",
                    ""id"": ""29968628-a977-44f0-a342-8b289216bbf8"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Wepon2Equip"",
                    ""type"": ""Button"",
                    ""id"": ""71834edd-1541-470a-879b-1c712ee1f204"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""QuitGame"",
                    ""type"": ""Button"",
                    ""id"": ""08bc675e-131a-4b0b-b55e-851452e5dbc0"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                }
            ],
            ""bindings"": [
                {
                    ""name"": ""WASD"",
                    ""id"": ""48286497-0f1d-4270-b5f5-7de2ea52b224"",
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
                    ""id"": ""75f450ac-68a4-4209-90b3-c69f1b05efbb"",
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
                    ""id"": ""8804e3f7-825a-4412-ab3c-9f67728ee130"",
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
                    ""id"": ""d53b764f-03a5-4fe9-87eb-33151efe535b"",
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
                    ""id"": ""0473e915-6ca1-415f-b426-2e00d5802387"",
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
                    ""id"": ""a505b4e1-c875-400f-9922-d47f670b2a3c"",
                    ""path"": ""<Gamepad>/leftStick"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Movement"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""80f2e6d3-5a08-4eea-9246-e24f00f1dd37"",
                    ""path"": ""<Gamepad>/buttonSouth"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Jump"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""e4e3fde3-0672-45c1-81a2-1eed6ee75d0e"",
                    ""path"": ""<Gamepad>/buttonWest"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Attack"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""9aa6dbee-a1f0-4bc3-a6dd-747a028f9326"",
                    ""path"": ""<Keyboard>/f"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Attack"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""ae9e3405-b0b4-40b5-80dd-6a84e2dd4a88"",
                    ""path"": ""<Gamepad>/leftTrigger"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""SpecialAction"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""5e4be3cd-bc0c-46a0-a1ca-ab6e103df223"",
                    ""path"": ""<Keyboard>/leftShift"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""SpecialAction"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""ff3fd011-9c1c-4285-bafa-a894851ff550"",
                    ""path"": ""<Gamepad>/dpad/up"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Wepon1Equip"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""0f5484cf-2f4c-4d2f-a76d-2489ed0bebdd"",
                    ""path"": ""<Keyboard>/1"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Wepon1Equip"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""2694bc06-0eae-4296-8088-8ead16da5510"",
                    ""path"": ""<Gamepad>/dpad/right"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Wepon2Equip"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""ae757537-7b9c-45e0-bb70-01153ab9537c"",
                    ""path"": ""<Keyboard>/escape"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""QuitGame"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                }
            ]
        }
    ],
    ""controlSchemes"": [
        {
            ""name"": ""MouseAndKeyboard"",
            ""bindingGroup"": ""MouseAndKeyboard"",
            ""devices"": [
                {
                    ""devicePath"": ""<Mouse>"",
                    ""isOptional"": false,
                    ""isOR"": false
                },
                {
                    ""devicePath"": ""<Keyboard>"",
                    ""isOptional"": false,
                    ""isOR"": false
                }
            ]
        },
        {
            ""name"": ""Gamepad"",
            ""bindingGroup"": ""Gamepad"",
            ""devices"": [
                {
                    ""devicePath"": ""<Gamepad>"",
                    ""isOptional"": false,
                    ""isOR"": false
                }
            ]
        }
    ]
}");
        // Gamepad
        m_Gamepad = asset.FindActionMap("Gamepad", throwIfNotFound: true);
        m_Gamepad_Movement = m_Gamepad.FindAction("Movement", throwIfNotFound: true);
        m_Gamepad_Attack = m_Gamepad.FindAction("Attack", throwIfNotFound: true);
        m_Gamepad_Jump = m_Gamepad.FindAction("Jump", throwIfNotFound: true);
        m_Gamepad_SpecialAction = m_Gamepad.FindAction("SpecialAction", throwIfNotFound: true);
        m_Gamepad_Wepon1Equip = m_Gamepad.FindAction("Wepon1Equip", throwIfNotFound: true);
        m_Gamepad_Wepon2Equip = m_Gamepad.FindAction("Wepon2Equip", throwIfNotFound: true);
        m_Gamepad_Aim = m_Gamepad.FindAction("Aim", throwIfNotFound: true);
        m_Gamepad_Interact = m_Gamepad.FindAction("Interact", throwIfNotFound: true);
        m_Gamepad_Ability1 = m_Gamepad.FindAction("Ability1", throwIfNotFound: true);
        m_Gamepad_Ability2 = m_Gamepad.FindAction("Ability2", throwIfNotFound: true);
        m_Gamepad_Pause = m_Gamepad.FindAction("Pause", throwIfNotFound: true);
        // MouseAndKeyboard
        m_MouseAndKeyboard = asset.FindActionMap("MouseAndKeyboard", throwIfNotFound: true);
        m_MouseAndKeyboard_Movement = m_MouseAndKeyboard.FindAction("Movement", throwIfNotFound: true);
        m_MouseAndKeyboard_SpecialAction = m_MouseAndKeyboard.FindAction("SpecialAction", throwIfNotFound: true);
        m_MouseAndKeyboard_Jump = m_MouseAndKeyboard.FindAction("Jump", throwIfNotFound: true);
        m_MouseAndKeyboard_Attack = m_MouseAndKeyboard.FindAction("Attack", throwIfNotFound: true);
        m_MouseAndKeyboard_Wepon2Equip = m_MouseAndKeyboard.FindAction("Wepon2Equip", throwIfNotFound: true);
        m_MouseAndKeyboard_Wepon1Equip = m_MouseAndKeyboard.FindAction("Wepon1Equip", throwIfNotFound: true);
        m_MouseAndKeyboard_Aim = m_MouseAndKeyboard.FindAction("Aim", throwIfNotFound: true);
        m_MouseAndKeyboard_Interact = m_MouseAndKeyboard.FindAction("Interact", throwIfNotFound: true);
        m_MouseAndKeyboard_Ability1 = m_MouseAndKeyboard.FindAction("Ability1", throwIfNotFound: true);
        m_MouseAndKeyboard_Ability2 = m_MouseAndKeyboard.FindAction("Ability2", throwIfNotFound: true);
        m_MouseAndKeyboard_Pause = m_MouseAndKeyboard.FindAction("Pause", throwIfNotFound: true);
        // Player
        m_Player = asset.FindActionMap("Player", throwIfNotFound: true);
        m_Player_Movement = m_Player.FindAction("Movement", throwIfNotFound: true);
        m_Player_Jump = m_Player.FindAction("Jump", throwIfNotFound: true);
        m_Player_Attack = m_Player.FindAction("Attack", throwIfNotFound: true);
        m_Player_SpecialAction = m_Player.FindAction("SpecialAction", throwIfNotFound: true);
        m_Player_Wepon1Equip = m_Player.FindAction("Wepon1Equip", throwIfNotFound: true);
        m_Player_Wepon2Equip = m_Player.FindAction("Wepon2Equip", throwIfNotFound: true);
        m_Player_QuitGame = m_Player.FindAction("QuitGame", throwIfNotFound: true);
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

    // Gamepad
    private readonly InputActionMap m_Gamepad;
    private IGamepadActions m_GamepadActionsCallbackInterface;
    private readonly InputAction m_Gamepad_Movement;
    private readonly InputAction m_Gamepad_Attack;
    private readonly InputAction m_Gamepad_Jump;
    private readonly InputAction m_Gamepad_SpecialAction;
    private readonly InputAction m_Gamepad_Wepon1Equip;
    private readonly InputAction m_Gamepad_Wepon2Equip;
    private readonly InputAction m_Gamepad_Aim;
    private readonly InputAction m_Gamepad_Interact;
    private readonly InputAction m_Gamepad_Ability1;
    private readonly InputAction m_Gamepad_Ability2;
    private readonly InputAction m_Gamepad_Pause;
    public struct GamepadActions
    {
        private @PlayerControls m_Wrapper;
        public GamepadActions(@PlayerControls wrapper) { m_Wrapper = wrapper; }
        public InputAction @Movement => m_Wrapper.m_Gamepad_Movement;
        public InputAction @Attack => m_Wrapper.m_Gamepad_Attack;
        public InputAction @Jump => m_Wrapper.m_Gamepad_Jump;
        public InputAction @SpecialAction => m_Wrapper.m_Gamepad_SpecialAction;
        public InputAction @Wepon1Equip => m_Wrapper.m_Gamepad_Wepon1Equip;
        public InputAction @Wepon2Equip => m_Wrapper.m_Gamepad_Wepon2Equip;
        public InputAction @Aim => m_Wrapper.m_Gamepad_Aim;
        public InputAction @Interact => m_Wrapper.m_Gamepad_Interact;
        public InputAction @Ability1 => m_Wrapper.m_Gamepad_Ability1;
        public InputAction @Ability2 => m_Wrapper.m_Gamepad_Ability2;
        public InputAction @Pause => m_Wrapper.m_Gamepad_Pause;
        public InputActionMap Get() { return m_Wrapper.m_Gamepad; }
        public void Enable() { Get().Enable(); }
        public void Disable() { Get().Disable(); }
        public bool enabled => Get().enabled;
        public static implicit operator InputActionMap(GamepadActions set) { return set.Get(); }
        public void SetCallbacks(IGamepadActions instance)
        {
            if (m_Wrapper.m_GamepadActionsCallbackInterface != null)
            {
                @Movement.started -= m_Wrapper.m_GamepadActionsCallbackInterface.OnMovement;
                @Movement.performed -= m_Wrapper.m_GamepadActionsCallbackInterface.OnMovement;
                @Movement.canceled -= m_Wrapper.m_GamepadActionsCallbackInterface.OnMovement;
                @Attack.started -= m_Wrapper.m_GamepadActionsCallbackInterface.OnAttack;
                @Attack.performed -= m_Wrapper.m_GamepadActionsCallbackInterface.OnAttack;
                @Attack.canceled -= m_Wrapper.m_GamepadActionsCallbackInterface.OnAttack;
                @Jump.started -= m_Wrapper.m_GamepadActionsCallbackInterface.OnJump;
                @Jump.performed -= m_Wrapper.m_GamepadActionsCallbackInterface.OnJump;
                @Jump.canceled -= m_Wrapper.m_GamepadActionsCallbackInterface.OnJump;
                @SpecialAction.started -= m_Wrapper.m_GamepadActionsCallbackInterface.OnSpecialAction;
                @SpecialAction.performed -= m_Wrapper.m_GamepadActionsCallbackInterface.OnSpecialAction;
                @SpecialAction.canceled -= m_Wrapper.m_GamepadActionsCallbackInterface.OnSpecialAction;
                @Wepon1Equip.started -= m_Wrapper.m_GamepadActionsCallbackInterface.OnWepon1Equip;
                @Wepon1Equip.performed -= m_Wrapper.m_GamepadActionsCallbackInterface.OnWepon1Equip;
                @Wepon1Equip.canceled -= m_Wrapper.m_GamepadActionsCallbackInterface.OnWepon1Equip;
                @Wepon2Equip.started -= m_Wrapper.m_GamepadActionsCallbackInterface.OnWepon2Equip;
                @Wepon2Equip.performed -= m_Wrapper.m_GamepadActionsCallbackInterface.OnWepon2Equip;
                @Wepon2Equip.canceled -= m_Wrapper.m_GamepadActionsCallbackInterface.OnWepon2Equip;
                @Aim.started -= m_Wrapper.m_GamepadActionsCallbackInterface.OnAim;
                @Aim.performed -= m_Wrapper.m_GamepadActionsCallbackInterface.OnAim;
                @Aim.canceled -= m_Wrapper.m_GamepadActionsCallbackInterface.OnAim;
                @Interact.started -= m_Wrapper.m_GamepadActionsCallbackInterface.OnInteract;
                @Interact.performed -= m_Wrapper.m_GamepadActionsCallbackInterface.OnInteract;
                @Interact.canceled -= m_Wrapper.m_GamepadActionsCallbackInterface.OnInteract;
                @Ability1.started -= m_Wrapper.m_GamepadActionsCallbackInterface.OnAbility1;
                @Ability1.performed -= m_Wrapper.m_GamepadActionsCallbackInterface.OnAbility1;
                @Ability1.canceled -= m_Wrapper.m_GamepadActionsCallbackInterface.OnAbility1;
                @Ability2.started -= m_Wrapper.m_GamepadActionsCallbackInterface.OnAbility2;
                @Ability2.performed -= m_Wrapper.m_GamepadActionsCallbackInterface.OnAbility2;
                @Ability2.canceled -= m_Wrapper.m_GamepadActionsCallbackInterface.OnAbility2;
                @Pause.started -= m_Wrapper.m_GamepadActionsCallbackInterface.OnPause;
                @Pause.performed -= m_Wrapper.m_GamepadActionsCallbackInterface.OnPause;
                @Pause.canceled -= m_Wrapper.m_GamepadActionsCallbackInterface.OnPause;
            }
            m_Wrapper.m_GamepadActionsCallbackInterface = instance;
            if (instance != null)
            {
                @Movement.started += instance.OnMovement;
                @Movement.performed += instance.OnMovement;
                @Movement.canceled += instance.OnMovement;
                @Attack.started += instance.OnAttack;
                @Attack.performed += instance.OnAttack;
                @Attack.canceled += instance.OnAttack;
                @Jump.started += instance.OnJump;
                @Jump.performed += instance.OnJump;
                @Jump.canceled += instance.OnJump;
                @SpecialAction.started += instance.OnSpecialAction;
                @SpecialAction.performed += instance.OnSpecialAction;
                @SpecialAction.canceled += instance.OnSpecialAction;
                @Wepon1Equip.started += instance.OnWepon1Equip;
                @Wepon1Equip.performed += instance.OnWepon1Equip;
                @Wepon1Equip.canceled += instance.OnWepon1Equip;
                @Wepon2Equip.started += instance.OnWepon2Equip;
                @Wepon2Equip.performed += instance.OnWepon2Equip;
                @Wepon2Equip.canceled += instance.OnWepon2Equip;
                @Aim.started += instance.OnAim;
                @Aim.performed += instance.OnAim;
                @Aim.canceled += instance.OnAim;
                @Interact.started += instance.OnInteract;
                @Interact.performed += instance.OnInteract;
                @Interact.canceled += instance.OnInteract;
                @Ability1.started += instance.OnAbility1;
                @Ability1.performed += instance.OnAbility1;
                @Ability1.canceled += instance.OnAbility1;
                @Ability2.started += instance.OnAbility2;
                @Ability2.performed += instance.OnAbility2;
                @Ability2.canceled += instance.OnAbility2;
                @Pause.started += instance.OnPause;
                @Pause.performed += instance.OnPause;
                @Pause.canceled += instance.OnPause;
            }
        }
    }
    public GamepadActions @Gamepad => new GamepadActions(this);

    // MouseAndKeyboard
    private readonly InputActionMap m_MouseAndKeyboard;
    private IMouseAndKeyboardActions m_MouseAndKeyboardActionsCallbackInterface;
    private readonly InputAction m_MouseAndKeyboard_Movement;
    private readonly InputAction m_MouseAndKeyboard_SpecialAction;
    private readonly InputAction m_MouseAndKeyboard_Jump;
    private readonly InputAction m_MouseAndKeyboard_Attack;
    private readonly InputAction m_MouseAndKeyboard_Wepon2Equip;
    private readonly InputAction m_MouseAndKeyboard_Wepon1Equip;
    private readonly InputAction m_MouseAndKeyboard_Aim;
    private readonly InputAction m_MouseAndKeyboard_Interact;
    private readonly InputAction m_MouseAndKeyboard_Ability1;
    private readonly InputAction m_MouseAndKeyboard_Ability2;
    private readonly InputAction m_MouseAndKeyboard_Pause;
    public struct MouseAndKeyboardActions
    {
        private @PlayerControls m_Wrapper;
        public MouseAndKeyboardActions(@PlayerControls wrapper) { m_Wrapper = wrapper; }
        public InputAction @Movement => m_Wrapper.m_MouseAndKeyboard_Movement;
        public InputAction @SpecialAction => m_Wrapper.m_MouseAndKeyboard_SpecialAction;
        public InputAction @Jump => m_Wrapper.m_MouseAndKeyboard_Jump;
        public InputAction @Attack => m_Wrapper.m_MouseAndKeyboard_Attack;
        public InputAction @Wepon2Equip => m_Wrapper.m_MouseAndKeyboard_Wepon2Equip;
        public InputAction @Wepon1Equip => m_Wrapper.m_MouseAndKeyboard_Wepon1Equip;
        public InputAction @Aim => m_Wrapper.m_MouseAndKeyboard_Aim;
        public InputAction @Interact => m_Wrapper.m_MouseAndKeyboard_Interact;
        public InputAction @Ability1 => m_Wrapper.m_MouseAndKeyboard_Ability1;
        public InputAction @Ability2 => m_Wrapper.m_MouseAndKeyboard_Ability2;
        public InputAction @Pause => m_Wrapper.m_MouseAndKeyboard_Pause;
        public InputActionMap Get() { return m_Wrapper.m_MouseAndKeyboard; }
        public void Enable() { Get().Enable(); }
        public void Disable() { Get().Disable(); }
        public bool enabled => Get().enabled;
        public static implicit operator InputActionMap(MouseAndKeyboardActions set) { return set.Get(); }
        public void SetCallbacks(IMouseAndKeyboardActions instance)
        {
            if (m_Wrapper.m_MouseAndKeyboardActionsCallbackInterface != null)
            {
                @Movement.started -= m_Wrapper.m_MouseAndKeyboardActionsCallbackInterface.OnMovement;
                @Movement.performed -= m_Wrapper.m_MouseAndKeyboardActionsCallbackInterface.OnMovement;
                @Movement.canceled -= m_Wrapper.m_MouseAndKeyboardActionsCallbackInterface.OnMovement;
                @SpecialAction.started -= m_Wrapper.m_MouseAndKeyboardActionsCallbackInterface.OnSpecialAction;
                @SpecialAction.performed -= m_Wrapper.m_MouseAndKeyboardActionsCallbackInterface.OnSpecialAction;
                @SpecialAction.canceled -= m_Wrapper.m_MouseAndKeyboardActionsCallbackInterface.OnSpecialAction;
                @Jump.started -= m_Wrapper.m_MouseAndKeyboardActionsCallbackInterface.OnJump;
                @Jump.performed -= m_Wrapper.m_MouseAndKeyboardActionsCallbackInterface.OnJump;
                @Jump.canceled -= m_Wrapper.m_MouseAndKeyboardActionsCallbackInterface.OnJump;
                @Attack.started -= m_Wrapper.m_MouseAndKeyboardActionsCallbackInterface.OnAttack;
                @Attack.performed -= m_Wrapper.m_MouseAndKeyboardActionsCallbackInterface.OnAttack;
                @Attack.canceled -= m_Wrapper.m_MouseAndKeyboardActionsCallbackInterface.OnAttack;
                @Wepon2Equip.started -= m_Wrapper.m_MouseAndKeyboardActionsCallbackInterface.OnWepon2Equip;
                @Wepon2Equip.performed -= m_Wrapper.m_MouseAndKeyboardActionsCallbackInterface.OnWepon2Equip;
                @Wepon2Equip.canceled -= m_Wrapper.m_MouseAndKeyboardActionsCallbackInterface.OnWepon2Equip;
                @Wepon1Equip.started -= m_Wrapper.m_MouseAndKeyboardActionsCallbackInterface.OnWepon1Equip;
                @Wepon1Equip.performed -= m_Wrapper.m_MouseAndKeyboardActionsCallbackInterface.OnWepon1Equip;
                @Wepon1Equip.canceled -= m_Wrapper.m_MouseAndKeyboardActionsCallbackInterface.OnWepon1Equip;
                @Aim.started -= m_Wrapper.m_MouseAndKeyboardActionsCallbackInterface.OnAim;
                @Aim.performed -= m_Wrapper.m_MouseAndKeyboardActionsCallbackInterface.OnAim;
                @Aim.canceled -= m_Wrapper.m_MouseAndKeyboardActionsCallbackInterface.OnAim;
                @Interact.started -= m_Wrapper.m_MouseAndKeyboardActionsCallbackInterface.OnInteract;
                @Interact.performed -= m_Wrapper.m_MouseAndKeyboardActionsCallbackInterface.OnInteract;
                @Interact.canceled -= m_Wrapper.m_MouseAndKeyboardActionsCallbackInterface.OnInteract;
                @Ability1.started -= m_Wrapper.m_MouseAndKeyboardActionsCallbackInterface.OnAbility1;
                @Ability1.performed -= m_Wrapper.m_MouseAndKeyboardActionsCallbackInterface.OnAbility1;
                @Ability1.canceled -= m_Wrapper.m_MouseAndKeyboardActionsCallbackInterface.OnAbility1;
                @Ability2.started -= m_Wrapper.m_MouseAndKeyboardActionsCallbackInterface.OnAbility2;
                @Ability2.performed -= m_Wrapper.m_MouseAndKeyboardActionsCallbackInterface.OnAbility2;
                @Ability2.canceled -= m_Wrapper.m_MouseAndKeyboardActionsCallbackInterface.OnAbility2;
                @Pause.started -= m_Wrapper.m_MouseAndKeyboardActionsCallbackInterface.OnPause;
                @Pause.performed -= m_Wrapper.m_MouseAndKeyboardActionsCallbackInterface.OnPause;
                @Pause.canceled -= m_Wrapper.m_MouseAndKeyboardActionsCallbackInterface.OnPause;
            }
            m_Wrapper.m_MouseAndKeyboardActionsCallbackInterface = instance;
            if (instance != null)
            {
                @Movement.started += instance.OnMovement;
                @Movement.performed += instance.OnMovement;
                @Movement.canceled += instance.OnMovement;
                @SpecialAction.started += instance.OnSpecialAction;
                @SpecialAction.performed += instance.OnSpecialAction;
                @SpecialAction.canceled += instance.OnSpecialAction;
                @Jump.started += instance.OnJump;
                @Jump.performed += instance.OnJump;
                @Jump.canceled += instance.OnJump;
                @Attack.started += instance.OnAttack;
                @Attack.performed += instance.OnAttack;
                @Attack.canceled += instance.OnAttack;
                @Wepon2Equip.started += instance.OnWepon2Equip;
                @Wepon2Equip.performed += instance.OnWepon2Equip;
                @Wepon2Equip.canceled += instance.OnWepon2Equip;
                @Wepon1Equip.started += instance.OnWepon1Equip;
                @Wepon1Equip.performed += instance.OnWepon1Equip;
                @Wepon1Equip.canceled += instance.OnWepon1Equip;
                @Aim.started += instance.OnAim;
                @Aim.performed += instance.OnAim;
                @Aim.canceled += instance.OnAim;
                @Interact.started += instance.OnInteract;
                @Interact.performed += instance.OnInteract;
                @Interact.canceled += instance.OnInteract;
                @Ability1.started += instance.OnAbility1;
                @Ability1.performed += instance.OnAbility1;
                @Ability1.canceled += instance.OnAbility1;
                @Ability2.started += instance.OnAbility2;
                @Ability2.performed += instance.OnAbility2;
                @Ability2.canceled += instance.OnAbility2;
                @Pause.started += instance.OnPause;
                @Pause.performed += instance.OnPause;
                @Pause.canceled += instance.OnPause;
            }
        }
    }
    public MouseAndKeyboardActions @MouseAndKeyboard => new MouseAndKeyboardActions(this);

    // Player
    private readonly InputActionMap m_Player;
    private IPlayerActions m_PlayerActionsCallbackInterface;
    private readonly InputAction m_Player_Movement;
    private readonly InputAction m_Player_Jump;
    private readonly InputAction m_Player_Attack;
    private readonly InputAction m_Player_SpecialAction;
    private readonly InputAction m_Player_Wepon1Equip;
    private readonly InputAction m_Player_Wepon2Equip;
    private readonly InputAction m_Player_QuitGame;
    public struct PlayerActions
    {
        private @PlayerControls m_Wrapper;
        public PlayerActions(@PlayerControls wrapper) { m_Wrapper = wrapper; }
        public InputAction @Movement => m_Wrapper.m_Player_Movement;
        public InputAction @Jump => m_Wrapper.m_Player_Jump;
        public InputAction @Attack => m_Wrapper.m_Player_Attack;
        public InputAction @SpecialAction => m_Wrapper.m_Player_SpecialAction;
        public InputAction @Wepon1Equip => m_Wrapper.m_Player_Wepon1Equip;
        public InputAction @Wepon2Equip => m_Wrapper.m_Player_Wepon2Equip;
        public InputAction @QuitGame => m_Wrapper.m_Player_QuitGame;
        public InputActionMap Get() { return m_Wrapper.m_Player; }
        public void Enable() { Get().Enable(); }
        public void Disable() { Get().Disable(); }
        public bool enabled => Get().enabled;
        public static implicit operator InputActionMap(PlayerActions set) { return set.Get(); }
        public void SetCallbacks(IPlayerActions instance)
        {
            if (m_Wrapper.m_PlayerActionsCallbackInterface != null)
            {
                @Movement.started -= m_Wrapper.m_PlayerActionsCallbackInterface.OnMovement;
                @Movement.performed -= m_Wrapper.m_PlayerActionsCallbackInterface.OnMovement;
                @Movement.canceled -= m_Wrapper.m_PlayerActionsCallbackInterface.OnMovement;
                @Jump.started -= m_Wrapper.m_PlayerActionsCallbackInterface.OnJump;
                @Jump.performed -= m_Wrapper.m_PlayerActionsCallbackInterface.OnJump;
                @Jump.canceled -= m_Wrapper.m_PlayerActionsCallbackInterface.OnJump;
                @Attack.started -= m_Wrapper.m_PlayerActionsCallbackInterface.OnAttack;
                @Attack.performed -= m_Wrapper.m_PlayerActionsCallbackInterface.OnAttack;
                @Attack.canceled -= m_Wrapper.m_PlayerActionsCallbackInterface.OnAttack;
                @SpecialAction.started -= m_Wrapper.m_PlayerActionsCallbackInterface.OnSpecialAction;
                @SpecialAction.performed -= m_Wrapper.m_PlayerActionsCallbackInterface.OnSpecialAction;
                @SpecialAction.canceled -= m_Wrapper.m_PlayerActionsCallbackInterface.OnSpecialAction;
                @Wepon1Equip.started -= m_Wrapper.m_PlayerActionsCallbackInterface.OnWepon1Equip;
                @Wepon1Equip.performed -= m_Wrapper.m_PlayerActionsCallbackInterface.OnWepon1Equip;
                @Wepon1Equip.canceled -= m_Wrapper.m_PlayerActionsCallbackInterface.OnWepon1Equip;
                @Wepon2Equip.started -= m_Wrapper.m_PlayerActionsCallbackInterface.OnWepon2Equip;
                @Wepon2Equip.performed -= m_Wrapper.m_PlayerActionsCallbackInterface.OnWepon2Equip;
                @Wepon2Equip.canceled -= m_Wrapper.m_PlayerActionsCallbackInterface.OnWepon2Equip;
                @QuitGame.started -= m_Wrapper.m_PlayerActionsCallbackInterface.OnQuitGame;
                @QuitGame.performed -= m_Wrapper.m_PlayerActionsCallbackInterface.OnQuitGame;
                @QuitGame.canceled -= m_Wrapper.m_PlayerActionsCallbackInterface.OnQuitGame;
            }
            m_Wrapper.m_PlayerActionsCallbackInterface = instance;
            if (instance != null)
            {
                @Movement.started += instance.OnMovement;
                @Movement.performed += instance.OnMovement;
                @Movement.canceled += instance.OnMovement;
                @Jump.started += instance.OnJump;
                @Jump.performed += instance.OnJump;
                @Jump.canceled += instance.OnJump;
                @Attack.started += instance.OnAttack;
                @Attack.performed += instance.OnAttack;
                @Attack.canceled += instance.OnAttack;
                @SpecialAction.started += instance.OnSpecialAction;
                @SpecialAction.performed += instance.OnSpecialAction;
                @SpecialAction.canceled += instance.OnSpecialAction;
                @Wepon1Equip.started += instance.OnWepon1Equip;
                @Wepon1Equip.performed += instance.OnWepon1Equip;
                @Wepon1Equip.canceled += instance.OnWepon1Equip;
                @Wepon2Equip.started += instance.OnWepon2Equip;
                @Wepon2Equip.performed += instance.OnWepon2Equip;
                @Wepon2Equip.canceled += instance.OnWepon2Equip;
                @QuitGame.started += instance.OnQuitGame;
                @QuitGame.performed += instance.OnQuitGame;
                @QuitGame.canceled += instance.OnQuitGame;
            }
        }
    }
    public PlayerActions @Player => new PlayerActions(this);
    private int m_MouseAndKeyboardSchemeIndex = -1;
    public InputControlScheme MouseAndKeyboardScheme
    {
        get
        {
            if (m_MouseAndKeyboardSchemeIndex == -1) m_MouseAndKeyboardSchemeIndex = asset.FindControlSchemeIndex("MouseAndKeyboard");
            return asset.controlSchemes[m_MouseAndKeyboardSchemeIndex];
        }
    }
    private int m_GamepadSchemeIndex = -1;
    public InputControlScheme GamepadScheme
    {
        get
        {
            if (m_GamepadSchemeIndex == -1) m_GamepadSchemeIndex = asset.FindControlSchemeIndex("Gamepad");
            return asset.controlSchemes[m_GamepadSchemeIndex];
        }
    }
    public interface IGamepadActions
    {
        void OnMovement(InputAction.CallbackContext context);
        void OnAttack(InputAction.CallbackContext context);
        void OnJump(InputAction.CallbackContext context);
        void OnSpecialAction(InputAction.CallbackContext context);
        void OnWepon1Equip(InputAction.CallbackContext context);
        void OnWepon2Equip(InputAction.CallbackContext context);
        void OnAim(InputAction.CallbackContext context);
        void OnInteract(InputAction.CallbackContext context);
        void OnAbility1(InputAction.CallbackContext context);
        void OnAbility2(InputAction.CallbackContext context);
        void OnPause(InputAction.CallbackContext context);
    }
    public interface IMouseAndKeyboardActions
    {
        void OnMovement(InputAction.CallbackContext context);
        void OnSpecialAction(InputAction.CallbackContext context);
        void OnJump(InputAction.CallbackContext context);
        void OnAttack(InputAction.CallbackContext context);
        void OnWepon2Equip(InputAction.CallbackContext context);
        void OnWepon1Equip(InputAction.CallbackContext context);
        void OnAim(InputAction.CallbackContext context);
        void OnInteract(InputAction.CallbackContext context);
        void OnAbility1(InputAction.CallbackContext context);
        void OnAbility2(InputAction.CallbackContext context);
        void OnPause(InputAction.CallbackContext context);
    }
    public interface IPlayerActions
    {
        void OnMovement(InputAction.CallbackContext context);
        void OnJump(InputAction.CallbackContext context);
        void OnAttack(InputAction.CallbackContext context);
        void OnSpecialAction(InputAction.CallbackContext context);
        void OnWepon1Equip(InputAction.CallbackContext context);
        void OnWepon2Equip(InputAction.CallbackContext context);
        void OnQuitGame(InputAction.CallbackContext context);
    }
}
