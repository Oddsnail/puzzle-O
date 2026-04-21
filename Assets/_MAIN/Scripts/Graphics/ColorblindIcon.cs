using origin.settings;
using UnityEngine;
using UnityEngine.UI;

namespace origin.graphics 
{
	[RequireComponent(typeof(Image))]
	public class ColorblindIcon : MonoBehaviour 
	{

		public Sprite[] icons;

		[Range(0f, 1f)]
		public float onTransparency = 1f;

		private Image _image;
		private Color _color;
		private int currentOrder = 0;
		private bool isValidOrder => GameSettingManager.instance.isColorblindModeOn && currentOrder > 0 && currentOrder < 4;

		void Awake() 
		{
			_image = GetComponent<Image>();
			_image.color = Color.clear;
		}
		
		void OnEnable()
        {
            GameSettingManager.OnColorblindModeToggled += Refresh;
            Refresh();
        }

		void OnDisable() 
		{
			GameSettingManager.OnColorblindModeToggled -= Refresh;
		}

		public void SetOrder(int order) => currentOrder = order;
		public void SetColor(Color color) => _color = color;

		public void Refresh() {
			if (_image == null) return;
			if (isValidOrder) {
				_image.sprite = icons[currentOrder - 1];
				_image.color = new(_color.r, _color.g, _color.b, onTransparency);
			} else {
				_image.color = new(_color.r, _color.g, _color.b, 0f);
			}
		}

	}
	
}