using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Main : MonoBehaviour
{
    static public Main S;
    static Dictionary<WeaponType, WeaponDefinition> WEAP_DICT;

    [Header("Set in Inspector")]
    public GameObject[] prefabEnemies;
    public float enemySpawnPerSecond = 0.5f;
    public float enemyDefaultPadding = 1.5f;
    public WeaponDefinition[] weaponDefinitions;
    public GameObject prefabPowerUp;
    public WeaponType[] powerUpFrequency = new WeaponType[]{
        WeaponType.blaster, WeaponType.blaster,
        WeaponType.spread, WeaponType.shield};

    private BoundsScript bndCheck;

    public void SpihDestroyed(Enemy e) 
    {
        if (Random.value <= e.powerUpDropChance)
        {
            int ndx = Random.Range(0, powerUpFrequency.Length);
            WeaponType puType = powerUpFrequency[ndx];

            GameObject go = Instantiate(prefabPowerUp) as GameObject;
            PowerUp pu = go.GetComponent<PowerUp>();
            pu.SetType(puType);

            pu.transform.position = e.transform.position;
        }
    }

    public void Awake()
    {
        S = this;
        bndCheck = GetComponent<BoundsScript>();
        Invoke("SpawnEnemy", 1f / enemySpawnPerSecond);

        WEAP_DICT = new Dictionary<WeaponType, WeaponDefinition>();
        foreach (var def in weaponDefinitions)
        {
            WEAP_DICT[def.type] = def;
        }
    }

    public void SpawnEnemy() 
    {
        int ndx = Random.Range(0, prefabEnemies.Length);
        GameObject go = Instantiate<GameObject>(prefabEnemies[ndx]);

        float enemyPadding = enemyDefaultPadding;
        if (go.GetComponent<BoundsScript>()!=null)
        {
            enemyPadding = Mathf.Abs(go.GetComponent<BoundsScript>().radius);
        }

        Vector3 pos = Vector3.zero;
        float xMin = -bndCheck.camWidth + enemyPadding;
        float xMax = bndCheck.camWidth - enemyPadding;
        pos.x = Random.Range(xMin, xMax);
        pos.y = bndCheck.camHeight + enemyPadding;
        go.transform.position = pos;

        Invoke("SpawnEnemy", 1f / enemySpawnPerSecond);
    }

    public void DelayedRestart(float delay)
    {
        Invoke("Restart", delay);
    }

    public void Restart() 
    {
        SceneManager.LoadScene("_Scene_0");
    }

    /// <summary>
    /// Ётот статическа€ функци€ возвращает данные о оружеи€ из
    /// защищенного словар€ WEAP_DICT
    /// </summary>
    /// <param name="wt">тип оружи€ WeaponType, дл€ которого требуетс€ получить
    /// WeaponDefinition</param>
    /// <returns>
    /// Ёкземлпл€р WeaponDefinition или, если нет такого определение
    /// дл€ указаного WeaponType, возвращает none 
    /// </returns>
   static public WeaponDefinition GetWeaponDefinition(WeaponType wt) 
    {
        if (WEAP_DICT.ContainsKey(wt))
        {
            return (WEAP_DICT[wt]);
        }
        return (new WeaponDefinition());
    }
}
