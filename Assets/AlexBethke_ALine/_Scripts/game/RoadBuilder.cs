using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace ALine
{
    public class RoadBuilder : MonoBehaviour
    {
        protected void Awake()
        {
            ServiceManager.instance.Add<RoadBuilder>(this);
        }
        protected void Start()
        {
            _game = ServiceManager.instance.Get<IGameController>();
        }
        public void Init(CalculatedValues in_values)
        {
            _calculated = in_values;
            CreateObjectPoolForWalls();
            CreateRoadGenerationData();
        }
        protected void OnDestroy()
        {
            ServiceManager.instance.Remove<RoadBuilder>();
        }
        public void Reset()
        {
            pathConnection = 3;

            while (_walls.Count > 0)
            {
                WallSegment wall = _walls[0];
                RemoveWall(wall);
            }
        }
        protected void CreateObjectPoolForWalls()
        {
            // max pool is a full screen of wall segments minus the one we're in and
            // the one we're moving to, plus the build row
            int maxPoolSize = (GameConfiguration.instance.wallRows + 1) * GameConfiguration.instance.wallColumns - 2;
            Utils.Log($"Initializing object pool for walls with max calculated size: {maxPoolSize}", GameDebugger.instance.debugAppLogic);
            _wallPool.Init(maxPoolSize, _calculated.wallWidth, _calculated.wallHeight);
        }
        protected void CreateRoadGenerationData()
        {
            // create the data for road generation 
            int index = 0;
            _segmentQueue = new int[_numBendsInQueue + _numStraightsInQueue];
            for (int i = 0; i < _numBendsInQueue; i++)
            {
                _segmentQueue[index] = (int)RoadTypes.Bend;
                index++;
            }
            for (int i = 0; i < _numStraightsInQueue; i++)
            {
                _segmentQueue[index] = (int)RoadTypes.Straight;
                index++;
            }
        }
        void Update()
        {
            if (_game.state != GameStates.ActiveGame)
                return;

            foreach (WallSegment wall in _walls)
            {
                wall.transform.position = wall.transform.position.PlusY(_calculated.moveSpeed * Time.deltaTime);
            }

            UpdateForLineRemoval();
            UpdateForNewLineCreation();
        }
        protected void UpdateForNewLineCreation()
        {
            if (_walls.Count == 0)
                return;

            WallSegment lastWall = _walls[_walls.Count - 1];
            if (lastWall.transform.position.y > _calculated.addWallsAboveY)
                return;

            _calculated.buildY = lastWall.transform.position.y + _calculated.wallHeight;
            GenerateNextRoadSegment();
        }
        protected void UpdateForLineRemoval()
        {
            if (_walls.Count == 0)
                return;

            bool clearing = true;
            while (clearing)
            {
                if (_walls.Count == 0)
                    break;

                WallSegment wall = _walls[0];
                if (wall.transform.position.y < _calculated.removeWallsBelowY)
                {
                    RemoveWall(wall);
                }
                else
                {
                    clearing = false;
                }
            }
        }
        public void GenerateStartWalls()
        {
            // set the build position to default
            _calculated.buildY = _calculated.buildYStart;
            // shuffle the segment queue when rebuilding the starting layout
            _segmentQueue.Shuffle();
            _segmentIndex = 0;
            Utils.Log($"Shuffled road segment queue: {_segmentQueue.ToStringForReal()}", GameDebugger.instance.debugRoadGeneration);

            for (int i = 0; i < GameConfiguration.instance.wallLayoutAtStart.Length; i++)
            {
                for (int j = 0; j < GameConfiguration.instance.wallLayoutAtStart[i].Length; j++)
                {
                    float x = _calculated.buildXStart +
                        _calculated.wallWidth * 0.5f + j * _calculated.wallWidth;

                    if (GameConfiguration.instance.wallLayoutAtStart[i][j] == '0')
                        continue;

                    WallSegment wall = GetWall();
                    wall.transform.position = new Vector2(x, _calculated.buildY);
                }
                _calculated.buildY += _calculated.wallHeight;
            }

            _lastPathDelta = 0;
        }
        protected void GenerateNextRoadSegment()
        {
            bool isStraight = (_segmentQueue[_segmentIndex] == (int)RoadTypes.Straight);
            Utils.Log($"Generating next road segment: {(isStraight ? "Straight" : "Bend")}", GameDebugger.instance.debugRoadGeneration);
            if (isStraight)
            {
                // build a straight path from the current connection
                for (int i = 0; i < GameConfiguration.instance.wallColumns; i++)
                {
                    if (i == pathConnection)
                        continue;

                    WallSegment wall = GetWall();
                    float x = _calculated.buildXStart + _calculated.wallWidth * 0.5f + i * _calculated.wallWidth;
                    wall.transform.position = new Vector2(x, _calculated.buildY);
                }

                _lastPathDelta = 0;
            }
            else
            {
                // build a bend segment
                Vector2Int pathways = GameConfiguration.instance.pathsByPosition[pathConnection - 1];
                int offset = Random.Range(pathways.x, pathways.y);
                // stop unintended straight spawns
                while (offset == 0)
                {
                    offset = Random.Range(pathways.x, pathways.y);
                }

                int start = pathConnection;
                int end = pathConnection + offset;

                int pathDelta = end - start;
                // check for potential bend overlaps
                if ((pathDelta < 0 && _lastPathDelta > 0) || (pathDelta > 0 && _lastPathDelta < 0))
                {
                    if (pathConnection == 1 || pathConnection == GameConfiguration.instance.wallColumns - 2)
                    {
                        Utils.Log("Road special case: Overlapping bend at edge switching to straight", GameDebugger.instance.debugRoadGeneration);
                        end = start;
                    }
                    else
                    {
                        Utils.Log("Road special case: Overlapping bend", GameDebugger.instance.debugRoadGeneration);
                        end = start + _lastPathDelta / Mathf.Abs(_lastPathDelta);
                        pathDelta = end - start;
                    }
                }
                _lastPathDelta = pathDelta;

                // generate the segment
                for (int i = 0; i < GameConfiguration.instance.wallColumns; i++)
                {
                    if (i < Mathf.Min(start, end) || i > Mathf.Max(start, end))
                    {
                        WallSegment wall = GetWall();
                        float x = _calculated.buildXStart + _calculated.wallWidth * 0.5f + i * _calculated.wallWidth;
                        wall.transform.position = new Vector2(x, _calculated.buildY);
                    }
                }

                pathConnection = end;
            }

            // check for queue roll over
            _segmentIndex++;
            if (_segmentIndex >= _segmentQueue.Length)
            {
                _segmentQueue.Shuffle();
                _segmentIndex = 0;
                Utils.Log($"Shuffled road segment queue: {_segmentQueue.ToStringForReal()}", GameDebugger.instance.debugRoadGeneration);
            }

            RoadSegmentSpawned?.Invoke();
        }
        protected WallSegment GetWall()
        {
            WallSegment wall = _wallPool.Get();
            _walls.Add(wall);
            return wall;
        }
        public void RemoveWall(WallSegment in_wall)
        {
            _walls.Remove(in_wall);
            _wallPool.Release(in_wall);
        }
        protected enum RoadTypes
        {
            Bend = 0,
            Straight = 1
        }

        protected CalculatedValues _calculated;

        protected int[] _segmentQueue;
        protected int _segmentIndex = 0;
        protected int _lastPathDelta;

        protected IGameController _game;

        [Header("Dynamic")]
        public UnityEvent RoadSegmentSpawned = new UnityEvent();
        public int pathConnection = 3;
        [SerializeField]
        protected List<WallSegment> _walls = new List<WallSegment>();

        [Header("Configuration")]
        [SerializeField]
        protected int _numStraightsInQueue = 10;
        [SerializeField]
        protected int _numBendsInQueue = 15;

        [Header("References")]
        [SerializeField]
        protected WallObjectPool _wallPool;
    }
}