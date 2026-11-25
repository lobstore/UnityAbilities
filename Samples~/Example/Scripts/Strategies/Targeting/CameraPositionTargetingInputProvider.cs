using UnityEngine;

namespace LOGIYGames.Abilities
{
    /// <summary>
    /// Example player-input adapter that converts the mouse cursor into world-space targeting data.
    /// Targeting strategies do not depend on this component or on a Camera.
    /// AI can disable this provider and write directly to AbilityController.TargetingInput.
    /// </summary>
    public class CameraPositionTargetingInputProvider : MonoBehaviour
    {
        [SerializeField] private AbilityController _controller;
        [SerializeField] private Camera _camera;
        [SerializeField, Min(0f)] private float _maxDistance = 50f;
        [SerializeField] private LayerMask _targetLayers = ~0;

        public void Initialize(AbilityController controller)
        {
            _controller = controller;
        }

        private void Awake()
        {
            if (_controller == null)
                _controller = GetComponent<AbilityController>();

            if (_camera == null)
                _camera = Camera.main;
        }

        private void Update()
        {
            if (_controller == null ||
                !_controller.IsCasting ||
                !(_controller.CurrentState is TargetingState))
                return;

            if (_camera == null)
                _camera = Camera.main;

            if (_camera == null)
            {
                _controller.TargetingInput.Clear();
                return;
            }

            Ray ray = _camera.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out RaycastHit hit, _maxDistance, _targetLayers))
            {
                _controller.TargetingInput.SetPosition(hit.point, hit.collider.gameObject);
            }
            else
            {
                _controller.TargetingInput.SetPosition(ray.GetPoint(_maxDistance));
            }
        }
    }
}
