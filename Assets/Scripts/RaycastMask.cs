using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(RectTransform))]
[RequireComponent(typeof(Image))]
public class RaycastMask : MonoBehaviour, ICanvasRaycastFilter {

    private Sprite _sprite;
    private RectTransform myRectTransform;

    void Start() {
        _sprite = GetComponent<Image>().sprite;
        myRectTransform = (RectTransform)transform;
    }

    public bool IsRaycastLocationValid(Vector2 sp, Camera eventCamera) {
        Vector2 local;
        RectTransformUtility.ScreenPointToLocalPointInRectangle((RectTransform)transform, sp, eventCamera, out local);
        // normalize local coordinates
        Vector2 normalized = new Vector2(
            (local.x + myRectTransform.pivot.x * myRectTransform.rect.width) / myRectTransform.rect.width,
            (local.y + myRectTransform.pivot.y * myRectTransform.rect.height) / myRectTransform.rect.width);
        // convert to texture space
        Rect rect = _sprite.textureRect;
        int x = Mathf.FloorToInt(rect.x + rect.width * normalized.x);
        int y = Mathf.FloorToInt(rect.y + rect.height * normalized.y);
        // destroy component if texture import settings are wrong
        try {
            return _sprite.texture.GetPixel(x, y).a > 0;

        }
        catch (UnityException e) {
            Debug.LogError("Mask texture not readable, set your sprite to Texture Type 'Advanced' and check 'Read/Write Enabled'");
            Debug.LogError(e);
            Destroy(this);
            return false;
        }
    }
}

