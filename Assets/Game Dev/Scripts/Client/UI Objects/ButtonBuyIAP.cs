using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonBuyIAP : MonoBehaviour
{
    public enum ItemType  
    {
        RUBY_100 = 0,
        RUBY_250,
        RUBY_500,
        RUBY_1500,
    }
    public ItemType itemmType;

    public void Click() {
        switch (itemmType)
        {
            case ItemType.RUBY_100:
                IAPManager.Instance.Buy100Ruby();
                break;
            case ItemType.RUBY_250:
                IAPManager.Instance.Buy250Ruby();
                break;
            case ItemType.RUBY_500:
                IAPManager.Instance.Buy500Ruby();
                break;
            case ItemType.RUBY_1500:
                IAPManager.Instance.Buy1500Ruby();
                break;
        }
    }
}
