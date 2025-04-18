using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Splines;

namespace Enigma.Plugboard
{
    public class PlugboardSplineGenerator
    {
        private readonly float _plugModelRadius;
        private readonly float _nearestNeighboursDistanceThreshold;
        private readonly Vector3 _plugboardPlane;
        private readonly float _splinesDesignatedZPosition; // The distance from the plain the plugs are on to the plane the splines should reside upon.

        public PlugboardSplineGenerator(float plugModelRadius, float nearestNeighboursDistanceThreshold,
            float splinesDesignatedZPosition, Vector3 plugboardPlane)
        {
            _plugModelRadius = plugModelRadius;
            _nearestNeighboursDistanceThreshold = nearestNeighboursDistanceThreshold;
            _plugboardPlane = plugboardPlane;
            _splinesDesignatedZPosition = splinesDesignatedZPosition;
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
            /*// If the plugs are horizontal neighbours
            if (Mathf.Approximately(first.transform.position.y, second.transform.position.y))
            {
                float leftPlugX = Mathf.Min(first.transform.position.x, second.transform.position.x);
                float rightPlugX = Mathf.Max(first.transform.position.x, second.transform.position.x);

                BezierKnot leftKnot =
                    new(new float3(leftPlugX + _plugModelRadius, first.transform.position.y,
                        _splinesDesignatedZPosition));
                BezierKnot rightKnot =
                    new(new float3(rightPlugX - _plugModelRadius, first.transform.position.y,
                        _splinesDesignatedZPosition));

                return new Spline(new[] { leftKnot, rightKnot });
            }

            // Vertical nearest neighbours
            if (Mathf.Approximately(first.transform.position.x, second.transform.position.x))
            {
                float xPosition = first.transform.position.x;
                float bottomPlugY = Mathf.Min(first.transform.position.y, second.transform.position.y);
                float topPlugY = Mathf.Max(first.transform.position.y, second.transform.position.y);

                BezierKnot topKnot = new(new float3(xPosition, topPlugY, _splinesDesignatedZPosition));
                BezierKnot bottomKnot = new(new float3(xPosition, bottomPlugY, _splinesDesignatedZPosition));

                return new Spline(new[] { topKnot, bottomKnot });
            }*/

            // Diagonal nearest neighbours
            Vector3 firstToSecondDirection = (first.transform.position - second.transform.position).normalized;

            BezierKnot firstKnot = new(first.transform.position + _plugModelRadius * firstToSecondDirection);
            BezierKnot secondKnot = new(second.transform.position - _plugModelRadius * firstToSecondDirection);

            return new Spline(new[] { firstKnot, secondKnot });
        }

        private bool IsNearestNeighbour(LetterPlug first, LetterPlug second)
        {
            float plugsDistance = Vector3.Distance(second.transform.position, first.transform.position);
            return plugsDistance - _nearestNeighboursDistanceThreshold < 0;
        }
    }
}
