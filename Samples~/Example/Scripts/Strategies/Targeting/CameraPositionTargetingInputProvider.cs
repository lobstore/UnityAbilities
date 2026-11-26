using UnityEngine;

namespace LOGIYGames.Abilities
{
    /// <summary>
    /// Example player-input adapter that converts the mouse cursor into world-space targeting data.
    /// Targeting strategies do not depend on this component or on a Camera.
    /// AI can disable this provider and write directly to AbilityController.CurrentContext.Target and TargetPoint.
    /// </summary>
    public class CameraPositionTargetingInputProvider : MonoBehaviour
    {
        [SerializeField] private AbilityController _controller;
        [SerializeField] private Camera _camera;
        [SerializeField, Min(0f)] private float _maxDistance = 50f;
        [SerializeField] private LayerMask _targetLayers = ~0;

        /// <summary>Connects this camera adapter to the controller whose active ability context it will update.</summary>
        /// <param name="controller">Controller that owns the shared targeting input data.</param>
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
                if (_controller.CurrentContext != null)
                {
                    _controller.CurrentContext.Target = null;
                    _controller.CurrentContext.TargetPoint = Vector3.zero;
                }
                return;
            }

            Ray ray = _camera.ScreenPointToRay(Input.mousePosition);

            RaycastHit[] hits = Physics.RaycastAll(ray, _maxDistance, _targetLayers);
            System.Array.Sort(hits, (a, b) => a.distance.CompareTo(b.distance));

            foreach (RaycastHit hit in hits)
            {
                if (hit.collider.transform.root == _controller.transform.root)
                    continue;

                _controller.CurrentContext.Target = hit.collider.gameObject;
                _controller.CurrentContext.TargetPoint = hit.point;
                return;
            }

            _controller.CurrentContext.Target = null;
            _controller.CurrentContext.TargetPoint = ray.GetPoint(_maxDistance);
        }
    }
}
