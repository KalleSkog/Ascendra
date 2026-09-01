#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using Ascendra.CameraRig;
using Ascendra.Player;

namespace Ascendra.Editor
{
    public static class FlatWorldCreator
    {
        private const string MaterialPath = "Assets/_Project/Art/Materials/FlatGround.mat";
        private const string ScenePath = "Assets/_Project/Scenes/FlatWorld.unity";

        [MenuItem("Tools/Ascendra/Create Flat Green World")]
        public static void CreateFlatGreenWorld()
        {
            Scene scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);

            GameObject ground = GameObject.CreatePrimitive(PrimitiveType.Plane);
            ground.name = "Ground";
            ground.transform.localScale = new Vector3(10f, 1f, 10f);

            Material groundMaterial = CreateGroundMaterial();
            ground.GetComponent<Renderer>().sharedMaterial = groundMaterial;

            CreateSunlight();
            GameObject player = CreateStickPerson();
            CreateCamera(player.transform);

            EditorSceneManager.SaveScene(scene, ScenePath);
            Selection.activeGameObject = player;
            Debug.Log("Created FlatWorld with a green ground, controllable stick person, and third-person camera.");
        }

        private static Material CreateGroundMaterial()
        {
            AssetDatabase.DeleteAsset(MaterialPath);

            Shader shader = Shader.Find("Universal Render Pipeline/Lit");
            if (shader == null)
            {
                shader = Shader.Find("Standard");
            }

            if (shader == null)
            {
                throw new MissingReferenceException("No supported Lit shader was found for the ground material.");
            }

            Material material = new Material(shader)
            {
                color = new Color(0.18f, 0.65f, 0.22f)
            };

            AssetDatabase.CreateAsset(material, MaterialPath);
            AssetDatabase.SaveAssets();
            return material;
        }

        private static void CreateSunlight()
        {
            GameObject lightObject = new GameObject("Sunlight");
            Light sunlight = lightObject.AddComponent<Light>();
            sunlight.type = LightType.Directional;
            sunlight.intensity = 1.2f;
            lightObject.transform.rotation = Quaternion.Euler(50f, -30f, 0f);
        }

        private static GameObject CreateStickPerson()
        {
            GameObject player = new GameObject("Stick Person");
            player.transform.position = new Vector3(0f, 0.05f, 0f);

            CharacterController characterController = player.AddComponent<CharacterController>();
            characterController.center = new Vector3(0f, 1.2f, 0f);
            characterController.height = 2.4f;
            characterController.radius = 0.3f;
            player.AddComponent<ThirdPersonStickController>();

            Material bodyMaterial = CreateBodyMaterial();
            CreateBodyPart("Body", PrimitiveType.Capsule, player.transform, new Vector3(0f, 1.25f, 0f), new Vector3(0.45f, 0.85f, 0.45f), bodyMaterial);
            CreateBodyPart("Head", PrimitiveType.Sphere, player.transform, new Vector3(0f, 2.35f, 0f), Vector3.one * 0.55f, bodyMaterial);
            CreateBodyPart("Left Arm", PrimitiveType.Cylinder, player.transform, new Vector3(-0.55f, 1.55f, 0f), new Vector3(0.12f, 0.55f, 0.12f), bodyMaterial, new Vector3(0f, 0f, 25f));
            CreateBodyPart("Right Arm", PrimitiveType.Cylinder, player.transform, new Vector3(0.55f, 1.55f, 0f), new Vector3(0.12f, 0.55f, 0.12f), bodyMaterial, new Vector3(0f, 0f, -25f));
            CreateBodyPart("Left Leg", PrimitiveType.Cylinder, player.transform, new Vector3(-0.2f, 0.5f, 0f), new Vector3(0.14f, 0.55f, 0.14f), bodyMaterial, new Vector3(0f, 0f, 8f));
            CreateBodyPart("Right Leg", PrimitiveType.Cylinder, player.transform, new Vector3(0.2f, 0.5f, 0f), new Vector3(0.14f, 0.55f, 0.14f), bodyMaterial, new Vector3(0f, 0f, -8f));
            return player;
        }

        private static void CreateBodyPart(string partName, PrimitiveType primitiveType, Transform parent, Vector3 localPosition, Vector3 localScale, Material material, Vector3 localEulerAngles = default)
        {
            GameObject part = GameObject.CreatePrimitive(primitiveType);
            part.name = partName;
            part.transform.SetParent(parent);
            part.transform.localPosition = localPosition;
            part.transform.localEulerAngles = localEulerAngles;
            part.transform.localScale = localScale;
            part.GetComponent<Renderer>().sharedMaterial = material;
            Object.DestroyImmediate(part.GetComponent<Collider>());
        }

        private static Material CreateBodyMaterial()
        {
            Shader shader = Shader.Find("Universal Render Pipeline/Lit") ?? Shader.Find("Standard");
            Material material = new Material(shader)
            {
                color = new Color(0.9f, 0.22f, 0.12f)
            };
            return material;
        }

        private static void CreateCamera(Transform player)
        {
            GameObject cameraObject = new GameObject("Main Camera");
            cameraObject.tag = "MainCamera";
            cameraObject.AddComponent<Camera>();
            ThirdPersonCameraFollow cameraFollow = cameraObject.AddComponent<ThirdPersonCameraFollow>();
            cameraFollow.Initialize(player);
            player.GetComponent<ThirdPersonStickController>().Initialize(cameraObject.transform);
        }
    }
}
#endif
