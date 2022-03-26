using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UpgradeUIMediator : MonoBehaviour
{
    [SerializeField] public TMP_Text name;
    [SerializeField] public Image image;
    [SerializeField] public TMP_Text price;
    [SerializeField] public TMP_Text upgradeCost;
    [SerializeField] public Button upgradeButton;
}
