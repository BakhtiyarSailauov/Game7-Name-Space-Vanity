using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hero : MonoBehaviour
{
    static public Hero S;
    [Header("Set in Inspector")]
    public float speed = 30;
    public float rollMult = -45;
    public float pitchMult = 30;
    public float gameRestartDelay = 2f;
    public GameObject projectilePrefab;
    public float projectileSpeed = 40;
    public Weapon[] weapons;

    [Header("Set Dynamycally")]

    [SerializeField]
    private float _shieldLevel = 1;

    private GameObject lastTriggerGo = null;

    public delegate void WeaponFireDelegat();
    public WeaponFireDelegat fireDelegat;

    public void Start()
    {
        if (S == null)
        {
            S = this;
        }
        // fireDelegat += TempFire;
        ClearWeapons();
        weapons[0].SetType(WeaponType.blaster);
    }

    public void Update()
    {
        float xAxis = Input.GetAxis("Horizontal");
        float yAxis = Input.GetAxis("Vertical");

        Vector3 pos = transform.position;
        pos.x += xAxis * speed * Time.deltaTime;
        pos.y += yAxis * speed * Time.deltaTime;
        transform.position = pos;

        transform.rotation = Quaternion.Euler(yAxis * pitchMult, xAxis * rollMult, 0);

        if (Input.GetAxis("Jump") == 1&&fireDelegat!= null)
        {
            fireDelegat();
        }
    }

    /*public void TempFire() 
    {
        GameObject proGo = Instantiate<GameObject>(projectilePrefab);
        proGo.transform.position = transform.position;
        Rigidbody rigidB = proGo.GetComponent<Rigidbody>();

        Projectile proj = proGo.GetComponent<Projectile>();
        proj.type = WeaponType.blaster;
        float tSpeed = Main.GetWeaponDefinition(proj.type).veclocity;
        rigidB.velocity = Vector3.up * tSpeed;
    }*/

    public void OnTriggerEnter(Collider other)
    {
        Transform rootT = other.gameObject.transform.root;
        GameObject go = rootT.gameObject;
        //print("Triggered: " + go.name);

        if (go == lastTriggerGo)
        {
            return;
        }
        lastTriggerGo = go;

        if (go.tag == "Enemy")
        {
            shieldLevel--;
            Destroy(go);
        }
        else if(go.tag == "PowerUp")
        {
            AbsorbPowerUp(go);
        }
        else
        {
            print("Triggered by no - Enemy" + go.name);
        }
    }

    public void AbsorbPowerUp(GameObject go) 
    {
        PowerUp pu = go.GetComponent<PowerUp>();
        switch (pu.type) 
        {
            case WeaponType.shield:
                shieldLevel++;
                break;
            default:
                if (pu.type == weapons[0].type)
                {
                    Weapon w = GetEmtyWeaponSlot();
                    if (w != null)
                    {
                        w.SetType(pu.type);
                    }
                    else
                    {
                        ClearWeapons();
                        weapons[0].SetType(pu.type);
                    }
                }
                break;
        }
        pu.AbsorbedBy(this.gameObject);
    }

    public float shieldLevel 
    {
        get { return (_shieldLevel); }
        set 
        { 
            _shieldLevel = Mathf.Min(value, 4);

            if (value < 0)
            {
                Destroy(this.gameObject);
                Main.S.DelayedRestart(gameRestartDelay);
            }
        }
    }

    public Weapon GetEmtyWeaponSlot() 
    {
        for (int i = 0; i < weapons.Length; i++)
        {
            if (weapons[i].type == WeaponType.none)
            {
                return (weapons[i]);
            }
        }
        return (null);
    }

    public void ClearWeapons() 
    {
        foreach (var w in weapons)
        {
            w.SetType(WeaponType.none);
        }
    }
}
