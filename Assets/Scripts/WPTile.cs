using System;
using UnityEngine;

public class WPTile : MonoBehaviour
{
	public Vector2 touchOffset;
	public int xGrid;
	public int yGrid;
	public Direction end1;
	public Direction end2;

	private float _tileSize;
	private Texture2D _emptyTexture;
	private Texture2D _filledTexture;

	public enum Direction
	{
		Up,
		Right,
		Down,
		Left
	}

	public delegate void OnTouchDelegate(GameObject touchedObject);
	public delegate void OnDragDelegate(Vector2 touchPos);
	public delegate void OnReleaseDelegate(GameObject touchedObject);

	OnTouchDelegate touchDelegate;
	OnDragDelegate dragDelegate;
	OnReleaseDelegate releaseDelegate;

	public OnTouchDelegate TouchDelegate
	{
		set { touchDelegate = value; }
	}

	public OnDragDelegate DragDelegate
	{
		set { dragDelegate = value; }
	}

	public OnReleaseDelegate ReleaseDelegate
	{
		set { releaseDelegate = value; }
	}

	public static WPTile CreateTile(Texture2D emptyTexture, Texture2D filledTexture, float posX, float posY, bool collision, float tileSize, WPTile wpTilePrefab, bool filled = false)
	{
		WPTile wpTile = Instantiate(wpTilePrefab, new Vector3(posX, posY, 0), Quaternion.identity);
		wpTile.SetTextures(emptyTexture, filledTexture);
		wpTile.SetTileSize(tileSize);

		wpTile.SetTexture(filled);

		BoxCollider2D c = wpTile.GetComponent<BoxCollider2D>();

		if (collision)
		{
			c.size = new Vector2(tileSize, tileSize);
			c.offset = new Vector2(tileSize / 2, tileSize / 2);
		}
		else
		{
			Destroy(c);
		}

		return wpTile;
	}

	private void SetTextures(Texture2D empty, Texture2D filled)
	{
		_filledTexture = filled;
		_emptyTexture = empty;
	}

	public void SetTileSize(float tileSize)
	{
		_tileSize = tileSize;
	}

	public void SetTexture(bool filled)
	{
		Texture2D texture = filled ? _filledTexture : _emptyTexture;

		float ppu = texture.width / _tileSize;

		Sprite sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0, 0), ppu);
		SpriteRenderer sr = GetComponent<SpriteRenderer>();
		sr.sprite = sprite;
	}

	public Vector2 GetNewCoordinates(WPTile previousWpTile)
	{

		int newX = xGrid;
		int newY = yGrid;

		switch (end1)
		{
			case Direction.Up:
				newY -= 1;
				break;
			case Direction.Right:
				newX += 1;
				break;
			case Direction.Down:
				newY += 1;
				break;
			case Direction.Left:
				newX -= 1;
				break;
		}

		if (newX == previousWpTile.xGrid && newY == previousWpTile.yGrid)
		{
			newX = xGrid;
			newY = yGrid;

			switch (end2)
			{
				case Direction.Up:
					newY -= 1;
					break;
				case Direction.Right:
					newX += 1;
					break;
				case Direction.Down:
					newY += 1;
					break;
				case Direction.Left:
					newX -= 1;
					break;
			}
		}

		return new Vector2(newX, newY);
	}

	private static Direction RevertEnd(Direction direction)
	{
		return GetDirectionFromInt((GetIntFromDirection(direction) + Enum.GetNames(typeof(Direction)).Length / 2) % 4);
	}

	public bool IsFilled(WPTile previousWpTile)
	{
		if (previousWpTile.xGrid < xGrid && (previousWpTile.end1 == Direction.Right || previousWpTile.end2 == Direction.Right))
		{
			return end1 == Direction.Left || end2 == Direction.Left;
		}
		else if (previousWpTile.xGrid > xGrid && (previousWpTile.end1 == Direction.Left || previousWpTile.end2 == Direction.Left))
		{
			return end1 == Direction.Right || end2 == Direction.Right;
		}
		else if (previousWpTile.yGrid < yGrid && (previousWpTile.end1 == Direction.Down || previousWpTile.end2 == Direction.Down))
		{
			return end1 == Direction.Up || end2 == Direction.Up;
		}
		else if (previousWpTile.yGrid > yGrid && (previousWpTile.end1 == Direction.Up || previousWpTile.end2 == Direction.Up))
		{
			return end1 == Direction.Down || end2 == Direction.Down;
		}

		return false;
	}

	private void OnMouseDown()
	{
		Vector2 touchPos = WPDragController.GetMousePos();
		touchOffset = new Vector2(touchPos.x - transform.position.x, touchPos.y - transform.position.y);

		touchDelegate(gameObject);
	}

	private void OnMouseDrag()
	{
		Vector2 touchPos = WPDragController.GetMousePos();
		dragDelegate(touchPos);
	}

	private void OnMouseUp()
	{
		touchOffset = new Vector2();

		releaseDelegate(gameObject);
	}

	private static int GetIntFromDirection(Direction direction)
	{
		switch (direction)
		{
			case Direction.Up: return 0;
			case Direction.Right: return 1;
			case Direction.Down: return 2;
			case Direction.Left: return 3;
			default: return 0;
		}
	}

	public static Direction GetDirectionFromInt(int directionInt)
	{
		switch (directionInt)
		{
			case 0: return Direction.Up;
			case 1: return Direction.Right;
			case 2: return Direction.Down;
			case 3: return Direction.Left;
			default: return Direction.Up;
		}
	}

	public static Vector2 GetOutput(Direction direction)
	{
		switch (direction)
		{
			// returns Vector2 movement from given output
			case Direction.Up: return Vector2.up;
			case Direction.Right: return Vector2.right;
			case Direction.Down: return Vector2.down;
			case Direction.Left: return Vector2.left;
			default: return Vector2.zero;
		}
	}
}
