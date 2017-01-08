using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIInventoryItem : MonoBehaviour {

	public delegate void OnClickedHandler(int itemNumber);
	public static event OnClickedHandler OnClicked;

	[SerializeField]
	private Text itemNumber;

	[SerializeField]
	private Button itemButton;

	private int number;

	public void Init(int n) {
		number = n;

		itemNumber.text = number.ToString ();
		name = "InventoryItem_" + number;

		itemButton.onClick.AddListener (() => HandleOnClick());
	}

	private void HandleOnClick() {
		if (OnClicked != null) {
			OnClicked (number);
		}
	}
}
