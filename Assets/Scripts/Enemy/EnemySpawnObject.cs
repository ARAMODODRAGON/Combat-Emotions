using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawnObject : MonoBehaviour
{
    public void Die() //Called at the end of animation
    {
        Destroy(gameObject);
    }
}
