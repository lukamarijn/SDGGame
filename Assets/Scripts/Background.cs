using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Background : MonoBehaviour
{
    void Awake() {
     SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
 
     float cameraHeight = Camera.main.orthographicSize * 2;
     Vector2 cameraSize = new Vector2(Camera.main.aspect * cameraHeight, cameraHeight);
     Vector2 spriteSize = spriteRenderer.sprite.bounds.size;   
     
     Vector2 scale = transform.localScale;
     scale *= cameraSize.x / spriteSize.x;
     var height = cameraSize.y / spriteSize.y * 0.55;
     scale.y = (float) height;
     
     transform.localScale = scale;
    }

}
