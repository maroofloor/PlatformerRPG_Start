using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AllInterface : MonoBehaviour
{
    public interface IHit
    {
        public void Hit(float damage, Vector2 pos);
    }
}
