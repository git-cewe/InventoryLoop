using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class UIInventory : MonoBehaviour, IBeginDragHandler, IEndDragHandler {

	public delegate void OnSelectedItemChangedHandler(int itemNumber);
	public static event OnSelectedItemChangedHandler OnSelectedItemChanged;

	[SerializeField]
	private Transform inventoryItemContainerTransform;

	[SerializeField]
	private UIInventoryItem inventoryItem;

	[SerializeField]
	private RectTransform scrollPanelRectTransform;

	[SerializeField]
	private RectTransform centerToCompareRect;

	private RectTransform[] inventoryItemRectTransforms;

    private float[] absoluteDistancesToCenter;
	private float[] distancesToCenter;

	private float maxDistanceToCenter = 30f;

	private const int inventoryItemCount = 20;
	private const int distanceBetweenButtons = 100;

	private int buttonNumberWithSmallestDistance;

	private bool isDragging;
	private bool isTappedOnItem;
	private bool isInitDone;

	public void OnBeginDrag(PointerEventData pointerEventData) {
		isDragging = true;
		isTappedOnItem = false;
	}

	public void OnEndDrag(PointerEventData pointerEventData) {
		isDragging = false;
	}

	private void OnEnable() {
		InitInventoryItems ();
	}

	private void Update() {
		if (isInitDone) {
			for (var i = 0; i < inventoryItemCount; i++) {
				distancesToCenter [i] = centerToCompareRect.position.x - inventoryItemRectTransforms [i].position.x;
				absoluteDistancesToCenter [i] = Mathf.Abs (distancesToCenter [i]);

				CheckInventoryItemPosition(distancesToCenter[i], i);
			}

			if (!isTappedOnItem) {
				var minDistance = Mathf.Min (absoluteDistancesToCenter);

				for (var j = 0; j < inventoryItemCount; j++) {
					if (minDistance == absoluteDistancesToCenter [j]) {
						ChangeCurrentButtonNumber (j);
					}
				}
			}
		} 

		if(!isDragging) {
			LerpToInventoryItem (-inventoryItemRectTransforms[buttonNumberWithSmallestDistance].anchoredPosition.x);
		}
	}

	private void HandleOnInventoryItemClicked(int itemIndex) {
		isTappedOnItem = true;
		isDragging = false;

		ChangeCurrentButtonNumber (itemIndex);
	}

	private void InitInventoryItems() {
		inventoryItemRectTransforms = new RectTransform[inventoryItemCount];

		for (var i = 0; i < inventoryItemCount; i++) {
			var item = Instantiate (inventoryItem);
			item.transform.SetParent (inventoryItemContainerTransform);
			item.transform.localScale = Vector3.one;

			item.Init (i);

			inventoryItemRectTransforms [i] = item.GetComponent<RectTransform> ();
			inventoryItemRectTransforms [i].anchoredPosition3D = new Vector3 (i * distanceBetweenButtons, 0f, 0f);

			CheckInventoryItemPosition(centerToCompareRect.position.x - inventoryItemRectTransforms [i].position.x, i);
		}
			
		absoluteDistancesToCenter = new float[inventoryItemCount];
		distancesToCenter = new float[inventoryItemCount];

		UIInventoryItem.OnClicked += HandleOnInventoryItemClicked;

		isInitDone = true;
	}

	private void ChangeCurrentButtonNumber(int index) {
		buttonNumberWithSmallestDistance = index;

		if (OnSelectedItemChanged != null) {
			OnSelectedItemChanged (index);
		}
	}

	private void LerpToInventoryItem(float position) {
		var xPosition = Mathf.Lerp (scrollPanelRectTransform.anchoredPosition.x, position, Time.deltaTime * 5f);
		var targetPosition = new Vector3 (xPosition, scrollPanelRectTransform.anchoredPosition.y, 0f);

		scrollPanelRectTransform.anchoredPosition = targetPosition;
	}

	private void CheckInventoryItemPosition(float distanceToCenter, int itemIndex) {
		if (distanceToCenter > maxDistanceToCenter) {
			SetNewInventoryItemPosition (false, itemIndex);            
            return;
		}

		if (distanceToCenter < -(maxDistanceToCenter+1)) {
			SetNewInventoryItemPosition (true, itemIndex);
		}
	}

	private void SetNewInventoryItemPosition(bool setToFirstPosition, int itemIndex) {
		var currentAnchorPosition = inventoryItemRectTransforms [itemIndex].anchoredPosition;
		var positionOffest = (setToFirstPosition ? -1 : 1) * (inventoryItemCount * distanceBetweenButtons);

		var newAnchoredPosition = new Vector3 (currentAnchorPosition.x + positionOffest, currentAnchorPosition.y, 0f);
		inventoryItemRectTransforms [itemIndex].anchoredPosition3D = newAnchoredPosition;
	}
}
