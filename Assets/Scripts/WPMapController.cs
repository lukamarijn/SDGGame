using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using Random = System.Random;

public class WPMapController : MonoBehaviour
{

	public event Action LevelComplete;

	[FormerlySerializedAs("tilePrefab")] public WPTile wpTilePrefab;
	public Texture2D startTexture;
	public Texture2D startTextureEmpty;
	public Texture2D endTexture;
	public Texture2D endTextureFilled;

	public List<Texture2D> tileTexturesEmpty;
	public List<Texture2D> tileTexturesFull;
	public List<Vector2> tileBehaviours;

	public GameObject background;

	[Range(3, 5)]
	public int gridSize = 5;
	public float padding;

	public float referenceScreenWidth = 360;

	private float _tileSize;
	private WPTile[,] _map;
	private Vector2[,] _gridPositions;
	private WPTile _endWpTile;

	private WPDragController _wpDragController;

	private void Start()
	{
		_tileSize = (float)Math.Floor((referenceScreenWidth - 2 * padding) / gridSize) / 75f;

		_wpDragController = new WPDragController(this, _tileSize);
		_map = GenerateTileMap(GenerateRandomIntegerMap());

		ScaleBackground();
		CheckFilledTiles();
	}

	public int GetMoveLimit(Vector2 position, WPTile.Direction direction)
	{
		Vector2 directionVector = WPTile.GetOutput(direction);
		int spaces = 0;
		bool movePossible = true;

		while (movePossible)
		{
			if (position.x + directionVector.x < 0)
			{
				movePossible = false;
			}
			else if (position.x + directionVector.x >= gridSize)
			{
				movePossible = false;
			}
			else if (position.y + directionVector.y < 1)
			{
				movePossible = false;
			}
			else if (position.y + directionVector.y >= gridSize + 1)
			{
				movePossible = false;
			}
			else if (_map[(int)(position.x + directionVector.x), (int)(position.y + directionVector.y)] != null)
			{
				movePossible = false;
			}
			else
			{
				position += directionVector;
				spaces++;
			}
		}

		return spaces;
	}

	private int[,] GenerateRandomIntegerMap()
	{
		var newMap = new int[gridSize, gridSize];

		int[] tiles = new int[gridSize * gridSize];

		if (gridSize == 5)
		{
			tiles = new int[] { 0, 0, 0, 1, 1, 1, 2, 2, 2, 3, 3, 3, 3, 4, 4, 4, 4, 5, 5, 5, 5, 6, 6, 6, 6 };
		}
		else if (gridSize == 4)
		{
			tiles = new int[] { 0, 0, 0, 1, 1, 2, 2, 3, 3, 4, 4, 4, 5, 5, 6, 6, };
		}
		else if (gridSize == 3)
		{
			tiles = new int[] { 0, 0, 1, 2, 3, 3, 4, 5, 5 };
		}

		Random rand = new Random();
		for (int i = 0; i < tiles.Length - 1; i++)
		{
			int j = rand.Next(i, tiles.Length);
			int temp = tiles[i];
			tiles[i] = tiles[j];
			tiles[j] = temp;
		}

		int index = 0;
		for (int rowNumber = gridSize - 1; rowNumber >= 0; rowNumber--)
		{
			for (int columnNumber = gridSize - 1; columnNumber >= 0; columnNumber--)
			{
				newMap[rowNumber, columnNumber] = tiles[index];
				index++;
			}
		}

		return newMap;
	}

	private WPTile[,] GenerateTileMap(int[,] map)
	{

		_gridPositions = new Vector2[gridSize, gridSize + 2];

		var tileMap = new WPTile[gridSize, gridSize + 2];

		float halfGridSize = gridSize / 2.0f;

		var startTile = WPTile.CreateTile(startTextureEmpty, startTexture, (gridSize - 1 - halfGridSize) * _tileSize, (0 - halfGridSize) * _tileSize * -1, false, _tileSize, wpTilePrefab, true);
		startTile.xGrid = gridSize - 1;
		startTile.yGrid = 0;
		startTile.end1 = WPTile.Direction.Down;
		startTile.end2 = WPTile.Direction.Down;

		tileMap[gridSize - 1, 0] = startTile;

		for (var y = 1; y < gridSize + 1; y++)
		{
			for (var x = 0; x < gridSize; x++)
			{
				var tileType = map[x, y - 1] - 1;
				var posX = (x - halfGridSize) * _tileSize;
				var posY = (y - halfGridSize) * _tileSize * -1;

				if (tileType != -1)
				{
					var tile = WPTile.CreateTile(tileTexturesEmpty[tileType], tileTexturesFull[tileType], posX, posY, true, _tileSize, wpTilePrefab);
					tile.xGrid = x;
					tile.yGrid = y;
					tile.end1 = WPTile.GetDirectionFromInt((int)tileBehaviours[tileType].x);
					tile.end2 = WPTile.GetDirectionFromInt((int)tileBehaviours[tileType].y);
					tile.TouchDelegate = _wpDragController.OnTouchTile;
					tile.DragDelegate = _wpDragController.OnDragTile;
					tile.ReleaseDelegate = _wpDragController.OnReleaseTile;

					tileMap[x, y] = tile;
				}

				_gridPositions[x, y] = new Vector2(posX, posY);
			}
		}

		var endTile = WPTile.CreateTile(endTexture, endTextureFilled, (0 - halfGridSize) * _tileSize, (gridSize + 1 - halfGridSize) * _tileSize * -1, false, _tileSize, wpTilePrefab);
		endTile.xGrid = 0;
		endTile.yGrid = gridSize + 1;

		_endWpTile = endTile;
		tileMap[0, gridSize + 1] = endTile;

		return tileMap;
	}

	private void ScaleBackground()
	{
		var newSize = _tileSize * gridSize * 100f;
		background.transform.localScale = new Vector3(newSize, newSize, 0);
	}

	public Vector2 GetGridPosition(int x, int y)
	{
		return _gridPositions[x, y];
	}

	public void SwapTiles(WPTile selected, int xSnap, int ySnap)
	{
		_map[xSnap, ySnap] = selected;
		_map[selected.xGrid, selected.yGrid] = null;
		selected.xGrid = xSnap;
		selected.yGrid = ySnap;
	}

	public void CheckFilledTiles()
	{
		WPTile previousWpTile = _map[gridSize - 1, 0];
		WPTile currentWpTile = _map[gridSize - 1, 1];

		for (var y = 1; y < gridSize + 1; y++)
		{
			for (var x = 0; x < gridSize; x++)
			{
				var tile = _map[x, y];
				if (tile != null) tile.SetTexture(false);
			}
		}

		while (currentWpTile != null && currentWpTile.IsFilled(previousWpTile))
		{
			currentWpTile.SetTexture(true);
			Vector2 newCoordinates = currentWpTile.GetNewCoordinates(previousWpTile);

			int newX = (int)newCoordinates.x;
			int newY = (int)newCoordinates.y;

			previousWpTile = currentWpTile;

			currentWpTile = newX < gridSize && newX >= 0 ? _map[newX, newY] : null;

			if (currentWpTile == _endWpTile)
			{
				LevelComplete.Invoke();
				currentWpTile = null;
			}
		}
	}
}
