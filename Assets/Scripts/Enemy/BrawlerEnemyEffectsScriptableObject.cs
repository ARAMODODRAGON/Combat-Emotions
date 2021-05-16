using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[CreateAssetMenu(fileName = "EnemyEffectsList", menuName = "Enemies/EnemyEffectsListScriptableObject")]
public class BrawlerEnemyEffectsScriptableObject : ScriptableObject
{
    [SerializeField]
    public List<EnemyEffects> enemyEffects = new List<EnemyEffects>();

    public EnemySpawnObject GetEffect(string name_)
    {
        foreach (EnemyEffects e in enemyEffects)
            if (e.name.Equals(name_))
                return e.effectObject;

        return null;
    }
}

[Serializable]
public class EnemyEffects
{
    public string name = "";
    public EnemySpawnObject effectObject = null;

}