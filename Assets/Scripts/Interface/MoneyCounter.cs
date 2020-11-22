using UnityEngine;
using System.Collections;

public class MoneyCounter : InterfaceCounter
{
    void Start()
    {
        Count = Player.Instance.Money;
        Player.Instance.OnMoneyChanged += SetCount;
    }

    private void OnDestroy()
    {
        Player.Instance.OnMoneyChanged -= SetCount;
    }

    private void SetCount(int count)
    {
        Pulse();
        Count = count;
    }
}
