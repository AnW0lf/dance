using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewDance", menuName = "Custom/Dance", order = 1)]
public class Dance : ScriptableObject
{
    [SerializeField] private int _animationId = 0;
    [SerializeField] private string _lableText = string.Empty;
    [SerializeField] private Sprite _iconSprite = null;
    [SerializeField] private Sprite _backgroundSprite = null;
    [SerializeField] private DanceStyle _style = DanceStyle.CLASSIC;

    public int AnimationID => _animationId;
    public string LabelText => _lableText;
    public Sprite IconSprite => _iconSprite;
    public Sprite BackgroundSprite => _backgroundSprite;
    public DanceStyle Style => _style;
}

public enum DanceStyle { CLASSIC, JAZZ, STREET }
