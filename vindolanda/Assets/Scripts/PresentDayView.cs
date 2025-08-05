using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace Vindolanda
{
    [RequireComponent(typeof(Image))]
    public class PresentDayView : MonoBehaviour
    {
#if UNITY_EDITOR
        [Tooltip("Max width/height in metres. Will resize to fit aspect ratio")]
        public float targetSize = 1.5f;

        private void OnValidate()
        {
            var rect = transform.parent.GetComponent<RectTransform>();
            var image = GetComponent<Image>();
            if (image?.sprite == null) return;

            float aspect = (float)image.sprite.texture.width / (float)image.sprite.texture.height;

            if (aspect > 1.0f) // Wider than tall
            {
                rect.sizeDelta = new Vector2(targetSize, targetSize / aspect) / rect.localScale;
            }
            else // Taller than wide
            {
                rect.sizeDelta = new Vector2(targetSize * aspect, targetSize) / rect.localScale;
            }
        }
#endif
    }
}