using System;
using System.Collections.ObjectModel;
using UnityEngine;

[CreateAssetMenu(fileName = "NewDance", menuName = "Custom/Dance", order = 1)]
public class Dance : ScriptableObject
{
    [SerializeField] private int _animationId = 0;
    [SerializeField] private string _lableText = string.Empty;
    [SerializeField] private Sprite _iconSprite = null;
    [SerializeField] private Sprite _backgroundSprite = null;
    [SerializeField] private DanceStyle _style = DanceStyle.CLASSIC;
    [SerializeField] private BonusMove[] _bonusMoves = null;

    public int AnimationID => _animationId;
    public string LabelText => _lableText;
    public Sprite IconSprite => _iconSprite;
    public Sprite BackgroundSprite => _backgroundSprite;
    public DanceStyle Style => _style;
    public ReadOnlyCollection<BonusMove> BonusMoves => new ReadOnlyCollection<BonusMove>(_bonusMoves);
}

[Serializable]
public class BonusMove
{
    [Range(0f, 1f)]
    [SerializeField] private float _start = 0.5f;
    [Range(0f, 1f)]
    [SerializeField] private float _length = 0.05f;

    public BonusMove(float position, float range)
    {
        _start = position;
        _length = range;
    }

    public float Start => _start;
    public float Length => _length;
    public float End => _start + _length;

}

public enum DanceStyle { CLASSIC, JAZZ, STREET, UNSTYLED }
