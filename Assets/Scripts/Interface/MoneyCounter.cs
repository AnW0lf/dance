using UnityEngine;
using System.Collections;

public class MoneyCounter : InterfaceCounter
{
    void Start()
    {
        Count = Player.Instance.Money;
        OnCountChanged += (count) => Player.Instance.Money = count;
        Player.Instance.OnMoneyChanged += (count) =>
        {
            Pulse();
            Count = Player.Instance.Money;
        };
    }
}
