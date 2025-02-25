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
    [SerializeField] GameObject upArrow;
    [SerializeField] GameObject downArrow;

    public ItemData curItemData;

    void Awake()
    {
        Deselect();
    }

    // if stack is null, then there is not item in this slot
    public void UpdateSlot(ItemStack stack)
    {
        if (stack.itemType == ItemType.none)
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
        upArrow.SetActive(true);
        downArrow.SetActive(true);
    }

    public void Deselect()
    {
        upArrow.SetActive(false);
        downArrow.SetActive(false);
    }

}
