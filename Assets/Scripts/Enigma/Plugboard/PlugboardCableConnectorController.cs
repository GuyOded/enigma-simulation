using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Consts;
using UnityEngine.Splines;

namespace Enigma.Plugboard
{
    public class PlugboardCableConnectorController : MonoBehaviour
    {
        private static readonly int Color1 = Shader.PropertyToID("_Color");

        [SerializeField] private SplineMaterialContainer[] _splineContainers;
        [SerializeField] private LetterPlug _APlug;
        [SerializeField] private LetterPlug _RPlug;
        [SerializeField] private Collider _letterPlugCollider; // Example collider to find sizes and lengths and such
        [SerializeField] private Transform _splineContainerPool;

        private PlugboardSplineGenerator _splineGenerator;
        private readonly Dictionary<SplineContainer, (LetterPlug, LetterPlug)> _splineToConnectedPlugs = new();
        private readonly List<RowPath> _topConnections = new();
        private readonly List<RowPath> _midConnections = new();
        private readonly List<RowPath> _botConnections = new();
        private float _initialSplineXPosition;
        private float _rowConnectionXInterval = 0.00001f;


        private void Start()
        {
            _initialSplineXPosition = _splineContainers.First().SplineContainer.transform.position.x;

            foreach (SplineMaterialContainer splineMaterialContainer in _splineContainers)
            {
                splineMaterialContainer.SplineContainer.gameObject.SetActive(false);
            }

            float plugRadius = Mathf.Abs(_letterPlugCollider.bounds.max.y - _letterPlugCollider.bounds.min.y) / 2;
            // This is currently irrelevant because I have decided it is not worth calculating a path
            float nearestNeighborsDistanceThreshold =
                Vector3.Distance(_APlug.transform.position, _RPlug.transform.position);

            _splineGenerator =
                new PlugboardSplineGenerator(plugRadius, nearestNeighborsDistanceThreshold,
                    _splineContainerPool.position, 0);
        }

        public void RenderNewConnection(LetterPlug first, LetterPlug second, Color color)
        {
            SplineMaterialContainer splineMaterialContainer =
                _splineContainers.First(container => !container.SplineContainer.gameObject.activeInHierarchy);
            Spline connection = _splineGenerator.GenerateSpline(first, second);
            splineMaterialContainer.SplineContainer.AddSpline(connection);
            _splineToConnectedPlugs.Add(splineMaterialContainer.SplineContainer, (first, second));

            // Change Color
            splineMaterialContainer.MeshRenderer.material.SetColor(Color1, color);
            splineMaterialContainer.SplineContainer.gameObject.SetActive(true);

            List<RowPath> rowConnections;
            char rowFirstLetter;

            (List<RowPath>, (char, char))? rowConnectionsToRange = GetRowConnectionsFromLetterPlugs(first, second);
            if (!rowConnectionsToRange.HasValue)
            {
                return;
            }

            rowConnections = rowConnectionsToRange.Value.Item1;
            rowFirstLetter = rowConnectionsToRange.Value.Item2.Item1;

            char firstLetter = first.tag[0].ToString().ToUpper()[0];
            char secondLetter = second.tag[0].ToString().ToUpper()[0];

            int lowerIndex = Math.Min(firstLetter - rowFirstLetter, secondLetter - rowFirstLetter);
            int upperIndex = Math.Max(firstLetter - rowFirstLetter, secondLetter - rowFirstLetter);
            RowPath rowPath = new(lowerIndex, upperIndex, splineMaterialContainer.SplineContainer);
            rowConnections.Add(rowPath);
            rowConnections.Sort();
            AdjustZPositionForRowConnections(rowConnections);
        }

        public (char, char)? FindLettersFromSpline(GameObject spline)
        {
            return _splineToConnectedPlugs.Where(kvp => kvp.Key.gameObject == spline).Select(kvp => LetterPlugsToCorrespondingLetters(kvp.Value)).FirstOrDefault();
        }

        public void RemoveTranspositionSpline(char first, char second)
        {
            KeyValuePair<SplineContainer, (LetterPlug, LetterPlug)> splineToPlugsKvp = _splineToConnectedPlugs.FirstOrDefault(kvp => LetterPlugsToCorrespondingLetters(kvp.Value) == (first, second) ||
            LetterPlugsToCorrespondingLetters(kvp.Value) == (second, first));

            splineToPlugsKvp.Key.gameObject.SetActive(false);
            _splineToConnectedPlugs.Remove(splineToPlugsKvp.Key);
            splineToPlugsKvp.Key.RemoveSpline(splineToPlugsKvp.Key.Spline);

            LetterPlug firstPlug = splineToPlugsKvp.Value.Item1;
            LetterPlug secondPlug = splineToPlugsKvp.Value.Item2;

            (List<RowPath>, (char, char))? rowPathsToLetterRange = GetRowConnectionsFromLetterPlugs(firstPlug, secondPlug);
            if (!rowPathsToLetterRange.HasValue)
            {
                return;
            }

            int removedElements = rowPathsToLetterRange.Value.Item1.RemoveAll(rowPath => rowPath.SplineProp.gameObject == splineToPlugsKvp.Key.gameObject);
            if (removedElements != 1)
            {
                throw new Exception($"Removed unexpected amount of row paths from list {first}:{second}. Removed amount: {removedElements}");
            }
        }

        private void AdjustZPositionForRowConnections(List<RowPath> rowConnections)
        {
            int index = 0;
            foreach (RowPath connection in rowConnections)
            {
                Vector3 currentPosition = connection.SplineProp.transform.position;
                connection.SplineProp.transform.position = new Vector3(_initialSplineXPosition + index * _rowConnectionXInterval, currentPosition.y, currentPosition.z);
                index++;
            }
        }

        /// <summary>
        /// Returns a two-tuple of relevant rowPaths list and the corresponding row range of the row if both first and second are in the same plug row.
        /// Null if they are not in the same plug row.
        /// </summary>
        /// <param name="first"></param>
        /// <param name="second"></param>
        /// <returns></returns>
        private (List<RowPath>, (char, char))? GetRowConnectionsFromLetterPlugs(LetterPlug first, LetterPlug second)
        {
            if (IsTopRow(first) && IsTopRow(second))
            {
                return (_topConnections, LetterPlacement.TopRowRange);
            }
            else if (IsMidRow(first) && IsMidRow(second))
            {
                return (_midConnections, LetterPlacement.MidRowRange);
            }
            else if (IsBotRow(first) && IsBotRow(second))
            {
                return (_botConnections, LetterPlacement.BotRowRange);
            }
            else
            {
                return null;
            }
        }

        private static bool IsTopRow(LetterPlug letterPlug)
        {
            char letter = letterPlug.tag[0];
            return IsInRow(LetterPlacement.TopRowRange, letter);
        }

        private static bool IsMidRow(LetterPlug letterPlug)
        {
            char letter = letterPlug.tag[0];
            return IsInRow(LetterPlacement.MidRowRange, letter);
        }

        private static bool IsBotRow(LetterPlug letterPlug)
        {
            char letter = letterPlug.tag[0];
            return IsInRow(LetterPlacement.BotRowRange, letter);
        }

        private static bool IsInRow((char, char) rowLetterRange, char letter)
        {
            string strLetter = letter.ToString();
            if (!Utils.StringUtils.IsLetter(strLetter))
            {
                throw new ArgumentException($"{letter} is not a string");
            }
            char upper = strLetter.ToUpper()[0];

            return upper >= rowLetterRange.Item1 && upper <= rowLetterRange.Item2;
        }

        private static (char, char) LetterPlugsToCorrespondingLetters((LetterPlug, LetterPlug) plugs)
        {
            return (plugs.Item1.tag.ToUpper()[0], plugs.Item2.tag.ToUpper()[0]);
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

    class RowPath : IComparable<RowPath>
    {
        public RowPath(int lowerIndex, int upperIndex, SplineContainer spline)
        {
            if (upperIndex <= lowerIndex)
            {
                throw new ArgumentException($"Lower index must be less than upper index {lowerIndex} >= {upperIndex}");
            }

            LowerIndex = lowerIndex;
            UpperIndex = upperIndex;
            SplineProp = spline;
        }

        public int LowerIndex { get; }
        public int UpperIndex { get; }
        public SplineContainer SplineProp { get; }

        public int CompareTo(RowPath rowPath)
        {
            if (rowPath.LowerIndex < LowerIndex && rowPath.UpperIndex > UpperIndex)
            {
                return 1;
            }

            if (rowPath.LowerIndex > LowerIndex && rowPath.UpperIndex < UpperIndex)
            {
                return -1;
            }

            return 0;
        }
    }
}
