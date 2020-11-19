using System;
using System.Collections.Generic;
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
    [SerializeField] private Dance _nextLevel = null;

    public int AnimationID => _animationId;
    public string LabelText => _lableText;
    public Sprite IconSprite => _iconSprite;
    public Sprite BackgroundSprite => _backgroundSprite;
    public DanceStyle Style => _style;
    public Dance NextLevel => _nextLevel;
    public bool HasNextLevel => _nextLevel != null;
    public ReadOnlyCollection<BonusMove> BonusMoves => new ReadOnlyCollection<BonusMove>(_bonusMoves);

    public override bool Equals(object obj)
    {
        return obj is Dance dance &&
               base.Equals(obj) &&
               _animationId == dance._animationId &&
               _lableText == dance._lableText &&
               EqualityComparer<Sprite>.Default.Equals(_iconSprite, dance._iconSprite) &&
               EqualityComparer<Sprite>.Default.Equals(_backgroundSprite, dance._backgroundSprite) &&
               _style == dance._style &&
               EqualityComparer<BonusMove[]>.Default.Equals(_bonusMoves, dance._bonusMoves) &&
               EqualityComparer<Dance>.Default.Equals(_nextLevel, dance._nextLevel);
    }

    public override int GetHashCode()
    {
        var hashCode = 749546344;
        hashCode = hashCode * -1521134295 + base.GetHashCode();
        hashCode = hashCode * -1521134295 + _animationId.GetHashCode();
        hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(_lableText);
        hashCode = hashCode * -1521134295 + EqualityComparer<Sprite>.Default.GetHashCode(_iconSprite);
        hashCode = hashCode * -1521134295 + EqualityComparer<Sprite>.Default.GetHashCode(_backgroundSprite);
        hashCode = hashCode * -1521134295 + _style.GetHashCode();
        hashCode = hashCode * -1521134295 + EqualityComparer<BonusMove[]>.Default.GetHashCode(_bonusMoves);
        hashCode = hashCode * -1521134295 + EqualityComparer<Dance>.Default.GetHashCode(_nextLevel);
        return hashCode;
    }

    public static bool operator ==(Dance d1, Dance d2) => d1.Equals(d2);

    public static bool operator !=(Dance d1, Dance d2) => !d1.Equals(d2);
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

    public override bool Equals(object obj)
    {
        return obj is BonusMove move &&
               _start == move._start &&
               _length == move._length;
    }

    public override int GetHashCode()
    {
        var hashCode = 1992257784;
        hashCode = hashCode * -1521134295 + _start.GetHashCode();
        hashCode = hashCode * -1521134295 + _length.GetHashCode();
        return hashCode;
    }
}

public enum DanceStyle { CLASSIC = 1, JAZZ = 2, STREET = 3, UNSTYLED = 0 }
