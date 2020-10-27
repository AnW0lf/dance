using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewDance", menuName = "Custom/Dance", order = 1)]
public class Dance : ScriptableObject
{
    [SerializeField] private int _animationId = 0;
    [SerializeField] private string _lableText = string.Empty;
    [SerializeField] private Sprite _iconSprite = null;
    [SerializeField] private Color _backgroundColor = Color.white;

    public int AnimationID => _animationId;
    public string LabelText => _lableText;
    public Sprite IconSprite => _iconSprite;
    public Color BackgroundColor => _backgroundColor;
}
