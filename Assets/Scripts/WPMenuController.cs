using UnityEngine;
using UnityEngine.SceneManagement;

public class WPMenuController : MonoBehaviour {
	
	public string gameScene;
	public RectTransform btnTextRect;
	public float textOffsetAmount = 3.0f;

	private Vector3 _textPos;
	private Vector3 _offsetPos;

	private void Start() {
		_textPos = btnTextRect.localPosition;
		_offsetPos = new Vector3(_textPos.x, _textPos.y - textOffsetAmount, _textPos.z);
	}

	public void OnClick() {
		SceneManager.LoadScene(gameScene);
	}

	public void MoveTextDown() {
		btnTextRect.localPosition = _offsetPos;
	}

	public void MoveTextUp() {
		btnTextRect.localPosition = _textPos;
	}

}