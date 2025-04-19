using System.Linq;
using UnityEngine;
using UnityEngine.Splines;

namespace Enigma.Plugboard
{
    public class PlugboardCableConnectorController : MonoBehaviour
    {
        [SerializeField] private SplineContainer[] _splineContainers;
        [SerializeField] private LetterPlug _topPlug;
        [SerializeField] private LetterPlug _bottomPlug;
        [SerializeField] private Collider _letterPlugCollider; // Example collider to find sizes and lengths and such
        [SerializeField] private Transform _splineContainerPool;
        private PlugboardSplineGenerator _splineGenerator;

        private void Start()
        {
            foreach (SplineContainer splineContainer in _splineContainers)
            {
                splineContainer.gameObject.SetActive(false);
            }

            float plugRadius = Mathf.Abs(_letterPlugCollider.bounds.max.y - _letterPlugCollider.bounds.min.y) / 2;
            // Plugs that are less distant than this value are considered nearest neighbours and are appropriately connected via spline
            float nearestNeighboursDistanceThreshold =
                Vector3.Distance(_topPlug.transform.position, _bottomPlug.transform.position);

            _splineGenerator =
                new PlugboardSplineGenerator(plugRadius, nearestNeighboursDistanceThreshold,
                    _splineContainerPool.position);
        }

        public void RenderNewConnection(LetterPlug first, LetterPlug second, Color color)
        {
            SplineContainer splineContainer = _splineContainers.First(container => !container.gameObject.activeSelf);
            Spline connection = _splineGenerator.GenerateSpline(first, second);
            splineContainer.AddSpline(connection);
            splineContainer.gameObject.SetActive(true);
        }
    }
}
