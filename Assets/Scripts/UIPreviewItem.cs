using UnityEngine;
using UnityEngine.UI;

public class UIPreviewItem : MonoBehaviour {

	[SerializeField]
	private Text previewItem_Text;

	private void OnEnable() {
		UIInventory.OnSelectedItemChanged += HandleOnSelectedItemChanged;
	}

	private void OnDisable() {
		UIInventory.OnSelectedItemChanged += HandleOnSelectedItemChanged;
	}

	private void HandleOnSelectedItemChanged(int itemNumber) {
		previewItem_Text.text = itemNumber.ToString ();
	}
}
