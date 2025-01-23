using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_InventorySlot : MonoBehaviour
{
    [SerializeField] Image itemIconImage;
    [SerializeField] TextMeshProUGUI countTextfield;

    // if stack is null, then there is not item in this slot
    public void UpdateSlot(ItemStack stack)
    {
        if (stack == null)
        {
            itemIconImage.gameObject.SetActive(false);
            countTextfield.gameObject.SetActive(false);
            return;
        }

        ItemData itemData = ItemsDataManager.instance.GetItemData(stack.itemType);

        itemIconImage.gameObject.SetActive(true);
        itemIconImage.sprite = itemData.image;

        if (itemData.isStackable)
        {
            countTextfield.text = stack.count.ToString();
            countTextfield.gameObject.SetActive(true);
        }
        else
        {
            countTextfield.gameObject.SetActive(false);
        }
    }

}
