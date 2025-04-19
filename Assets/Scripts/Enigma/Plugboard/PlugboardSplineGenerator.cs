using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Splines;

namespace Enigma.Plugboard
{
    public class PlugboardSplineGenerator
    {
        private readonly float _plugModelRadius;
        private readonly float _nearestNeighboursDistanceThreshold;
        private readonly float _splinesDistanceFromPlugboard; // The distance from the plain the plugs are on to the plane the splines should reside upon.
        private readonly Vector3 _parentPosition;

        public PlugboardSplineGenerator(float plugModelRadius, float nearestNeighboursDistanceThreshold,
            Vector3 parentPosition, float splinesDistanceFromPlugboard = 0.001f)
        {
            _plugModelRadius = plugModelRadius;
            _nearestNeighboursDistanceThreshold = nearestNeighboursDistanceThreshold;
            _splinesDistanceFromPlugboard = splinesDistanceFromPlugboard;
            _parentPosition = parentPosition;
        }

        public Spline GenerateSpline(LetterPlug first, LetterPlug second)
        {
            if (IsNearestNeighbour(first, second))
            {
                return GenerateNearestNeighbourSpline(first, second);
            }

            return new Spline();
        }

        private Spline GenerateNearestNeighbourSpline(LetterPlug first, LetterPlug second)
        {
            Vector3 firstToSecondDirection = (first.transform.position - second.transform.position).normalized;

            BezierKnot firstKnot = new(first.transform.position - _plugModelRadius * firstToSecondDirection +
                Vector3.right * _splinesDistanceFromPlugboard - _parentPosition);
            BezierKnot secondKnot = new(second.transform.position + _plugModelRadius * firstToSecondDirection +
                Vector3.right * _splinesDistanceFromPlugboard - _parentPosition);

            return new Spline(new[] { firstKnot, secondKnot });
        }

        private bool IsNearestNeighbour(LetterPlug first, LetterPlug second)
        {
            float plugsDistance = Vector3.Distance(second.transform.position, first.transform.position);
            float plugsDistanceComparisonValue = plugsDistance - _nearestNeighboursDistanceThreshold;
            return plugsDistanceComparisonValue < 0 || Mathf.Approximately(plugsDistanceComparisonValue, 0);
        }
    }
}
