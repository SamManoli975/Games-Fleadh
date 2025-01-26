using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_InventorySlot : MonoBehaviour
{
    [SerializeField] Image backgroundImage;
    [SerializeField] Image itemIconImage;
    [SerializeField] TextMeshProUGUI countTextfield;

    public ItemData curItemData;

    // if stack is null, then there is not item in this slot
    public void UpdateSlot(ItemStack stack)
    {
        if (stack == null)
        {
            itemIconImage.gameObject.SetActive(false);
            countTextfield.gameObject.SetActive(false);
            curItemData = null;
            return;
        }

        curItemData = ItemsDataManager.instance.GetItemData(stack.itemType);

        itemIconImage.gameObject.SetActive(true);
        itemIconImage.sprite = curItemData.image;

        if (curItemData.isStackable)
        {
            countTextfield.text = stack.count.ToString();
            countTextfield.gameObject.SetActive(true);
        }
        else
        {
            countTextfield.gameObject.SetActive(false);
        }
    }

    public void Select()
    {
        backgroundImage.color = Color.gray;
    }

    public void Deselect()
    {
        backgroundImage.color = Color.white;
    }

}
