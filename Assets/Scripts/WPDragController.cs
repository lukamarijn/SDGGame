using UnityEngine;

public class WPDragController {

	private WPMapController _wpMapController;
	private float _tileSize;
	
	private Vector2 _startMousePos, _newMousePos;
	private bool _moveHorizontal, _moveVertical;
	private int _left, _right, _up, _down;
	private WPTile _selected;
	private float _yMove, _xMove;
	private Vector2 _min, _max;

	public WPDragController(WPMapController wpMapController, float tileSize) {
		_wpMapController = wpMapController;
		_tileSize = tileSize;
	}
	
	public void OnTouchTile(GameObject tile)
	{
		if (_selected != null || Time.timeScale == 0) return;
		_selected = tile.GetComponent<WPTile>();
		_startMousePos = GetMousePos();

		_moveHorizontal = _moveVertical = false;

		var position = _selected.transform.position;
		_yMove = position.y;
		_xMove = position.x;
	}

	public void OnDragTile(Vector2 touch)
	{
		// allow movement only in available directions
		// and limit range of movement
		if (_selected == null || Time.timeScale == 0) return;

		_selected.SetTexture(false);
		
		// if tile is in starting position, detect if mouse movement
		// is predominantly horizontal or vertical and lock to that axis
		Vector2 gridPosition = _wpMapController.GetGridPosition(_selected.xGrid, _selected.yGrid);

		if (_selected.transform.position == new Vector3(gridPosition.x, gridPosition.y, 0))
		{
			_newMousePos = GetMousePos();

			if (_newMousePos != _startMousePos)
			{
				_moveHorizontal = Mathf.Abs(_newMousePos.x - _startMousePos.x) > Mathf.Abs(_newMousePos.y - _startMousePos.y);
				_moveVertical = !_moveHorizontal;
			}
		}

		Vector2 pos = new Vector2(_selected.xGrid, _selected.yGrid);

		// based on whether the user is trying to move the tile horizontally
		// or vertically, find out if there are any empty spaces in that
		// direction, and if so how many (to work out movement limits)
		if (_moveHorizontal)
		{
			_xMove = touch.x - _selected.touchOffset.x;

			_left = _selected.xGrid - _wpMapController.GetMoveLimit(pos, WPTile.Direction.Left);
			_right = _selected.xGrid + _wpMapController.GetMoveLimit(pos, WPTile.Direction.Right);

			_min = _wpMapController.GetGridPosition(_left, _selected.yGrid);
			_max = _wpMapController.GetGridPosition(_right, _selected.yGrid);

			if (_xMove > _max.x) _xMove = _max.x;
			if (_xMove < _min.x) _xMove = _min.x;

			_selected.transform.position = new Vector3(_xMove, _yMove, 0);
		} 
		else if (_moveVertical)
		{
			_yMove = touch.y - _selected.touchOffset.y;

			_up = _selected.yGrid + _wpMapController.GetMoveLimit(pos, WPTile.Direction.Up);
			_down = _selected.yGrid - _wpMapController.GetMoveLimit(pos, WPTile.Direction.Down);

			_min = _wpMapController.GetGridPosition(_selected.xGrid, _down);
			_max = _wpMapController.GetGridPosition(_selected.xGrid, _up);

			if (_yMove < _max.y) _yMove = _max.y;
			if (_yMove > _min.y) _yMove = _min.y;

			_selected.transform.position = new Vector3(_xMove, _yMove, 0);
		}
	}

	public void OnReleaseTile(GameObject tile) {
		if (Time.timeScale != 0 && tile == _selected.gameObject && _selected != null)
		{
			int xSnap = _selected.xGrid;
			int ySnap = _selected.yGrid;

			if (_moveHorizontal)
			{
				for (int i = _left; i <= _right; i++)
				{
					if (Mathf.Abs(_selected.transform.position.x - _wpMapController.GetGridPosition(i, _selected.yGrid).x) < _tileSize / 2)
					{
						xSnap = i;
						break;
					}
				}
			}

			if (_moveVertical)
			{
				for (int i = _down; i <= _up; i++)
				{
					if (Mathf.Abs(_selected.transform.position.y - _wpMapController.GetGridPosition(_selected.xGrid, i).y) < _tileSize / 2)
					{
						ySnap = i;
						break;
					}
				}
			}

			_xMove = _wpMapController.GetGridPosition(xSnap, _selected.yGrid).x;
			_yMove = _wpMapController.GetGridPosition(_selected.xGrid, ySnap).y;

			_selected.transform.position = new Vector3(_xMove, _yMove, 0);

			if (_selected.xGrid != xSnap || _selected.yGrid != ySnap) {
				_wpMapController.SwapTiles(_selected, xSnap, ySnap);
			}

			_selected = null;

			_wpMapController.CheckFilledTiles();
		}
	}
	
	public static Vector2 GetMousePos()
	{
		var mousePosition = Input.mousePosition;
		mousePosition.z = 10;

		return Camera.main.ScreenToWorldPoint(mousePosition);
	}
}
