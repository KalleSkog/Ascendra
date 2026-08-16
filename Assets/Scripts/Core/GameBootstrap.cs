using Ascendra.CameraRig;
using Ascendra.Player;
using Ascendra.World;
using UnityEngine;

namespace Ascendra.Core
{
    /// Entry point for the World scene: builds terrain, player and camera at runtime.
    public class GameBootstrap : MonoBehaviour
    {
        [SerializeField] private WorldGenerationSettings worldSettings = WorldGenerationSettings.Default;
        [SerializeField] private float playerHeight = 2f;

        private void Start()
        {
            CreateSun();
            Terrain terrain = WorldGenerator.Generate(worldSettings);
            Transform player = CreatePlayer(terrain);
            CreateCamera(player);
        }

        private void CreateSun()
        {
            GameObject sunObject = new GameObject("Sun");
            Light sun = sunObject.AddComponent<Light>();
            sun.type = LightType.Directional;
            sun.intensity = 1.2f;
            sun.shadows = LightShadows.Soft;
            sunObject.transform.rotation = Quaternion.Euler(50f, -30f, 0f);

            RenderSettings.sun = sun;
            RenderSettings.ambientIntensity = 1f;
        }

        private Transform CreatePlayer(Terrain terrain)
        {
            GameObject playerObject = GameObject.CreatePrimitive(PrimitiveType.Capsule);
            playerObject.name = "Player";

            // The primitive ships with a CapsuleCollider; a CharacterController replaces it for movement.
            Destroy(playerObject.GetComponent<CapsuleCollider>());
            CharacterController controller = playerObject.AddComponent<CharacterController>();
            controller.height = playerHeight;
            controller.center = new Vector3(0f, playerHeight * 0.5f, 0f);

            Renderer playerRenderer = playerObject.GetComponent<Renderer>();
            playerRenderer.material = PipelineMaterial.CreateLit(new Color(0.85f, 0.35f, 0.2f));

            Vector3 worldCenter = new Vector3(worldSettings.WorldSize * 0.5f, 0f, worldSettings.WorldSize * 0.5f);
            float spawnHeight = terrain.SampleHeight(worldCenter) + 0.1f;
            playerObject.transform.position = new Vector3(worldCenter.x, spawnHeight, worldCenter.z);

            playerObject.AddComponent<PlayerController>();

            return playerObject.transform;
        }

        private void CreateCamera(Transform player)
        {
            Camera cam = Camera.main;
            GameObject cameraObject = cam != null ? cam.gameObject : new GameObject("Main Camera");
            if (cam == null)
            {
                cam = cameraObject.AddComponent<Camera>();
                cameraObject.tag = "MainCamera";
            }

            cam.nearClipPlane = 0.1f;
            cam.farClipPlane = 1000f;

            OrbitCameraController orbitCamera = cameraObject.AddComponent<OrbitCameraController>();
            orbitCamera.Target = player;

            PlayerController playerController = player.GetComponent<PlayerController>();
            playerController.CameraPivot = cameraObject.transform;
        }
    }
}
