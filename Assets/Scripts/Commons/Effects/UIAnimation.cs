using System;
using DG.Tweening;
using UnityEngine;

namespace Effects
{
    public abstract class UIAnimation : MonoBehaviour
    {
        public abstract Tween Show();
        public abstract Tween Close();
    }
}
