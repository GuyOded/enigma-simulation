using System;
using System.Linq;
using UnityEngine;
using UnityEngine.Splines;

namespace Enigma.Plugboard
{
    public class PlugboardCableConnectorController : MonoBehaviour
    {
        private static readonly int Color1 = Shader.PropertyToID("_Color");
        [SerializeField] private SplineMaterialContainer[] _splineContainers;
        [SerializeField] private LetterPlug _topPlug;
        [SerializeField] private LetterPlug _bottomPlug;
        [SerializeField] private Collider _letterPlugCollider; // Example collider to find sizes and lengths and such
        [SerializeField] private Transform _splineContainerPool;
        private PlugboardSplineGenerator _splineGenerator;

        private void Start()
        {
            foreach (SplineMaterialContainer splineMaterialContainer in _splineContainers)
            {
                splineMaterialContainer.SplineContainer.gameObject.SetActive(false);
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
            SplineMaterialContainer splineMaterialContainer =
                _splineContainers.First(container => !container.SplineContainer.gameObject.activeInHierarchy);
            Spline connection = _splineGenerator.GenerateSpline(first, second);
            splineMaterialContainer.SplineContainer.AddSpline(connection);

            // Change Color
            splineMaterialContainer.MeshRenderer.material.SetColor(Color1, color);
            splineMaterialContainer.SplineContainer.gameObject.SetActive(true);
        }
    }

    [Serializable]
    public class SplineMaterialContainer
    {
        [SerializeField] private SplineContainer _splineContainer;
        [SerializeField] private MeshRenderer _meshRenderer;

        public SplineContainer SplineContainer
        {
            get => _splineContainer;
            private set => _splineContainer = value;
        }

        public MeshRenderer MeshRenderer
        {
            get => _meshRenderer;
            private set => _meshRenderer = value;
        }
    }
}
