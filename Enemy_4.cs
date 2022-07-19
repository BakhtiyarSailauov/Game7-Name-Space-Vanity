using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Part 
{
    public string name;
    public float health;
    public string[] projectedBy;

    [HideInInspector]
    public GameObject go;
    [HideInInspector]
    public Material mat;
}

public class Enemy_4 : Enemy
{
    [Header("Set in Inspector: Enemy_4")]
    public Part[] parts;
    private Vector3 p0, p1;
    private float timeStart;
    private float duration = 4;

    public void Start()
    {
        p0 = p1 = pos;
        InitMovement();

        Transform t;
        foreach (var prt in parts)
        {
            t = transform.Find(prt.name);
            if (t!=null)
            {
                prt.go = t.gameObject;
                prt.mat = prt.go.GetComponent<Renderer>().material;
            }
        }
    }

    public void InitMovement() 
    {
        p0 = p1;
        float widMinRad = bndCheck.camWidth - bndCheck.radius;
        float hgtMinRad = bndCheck.camHeight - bndCheck.radius;
        p1.x = Random.Range(-widMinRad, widMinRad);
        p1.y = Random.Range(-hgtMinRad, hgtMinRad);

        timeStart = Time.time;
    }

    public override void Move()
    {
        float u = (Time.time - timeStart) / duration;

        if (u>=1)
        {
            InitMovement();
            u = 0;
        }

        u = 1 - Mathf.Pow(1 - u, 2);
        pos = (1 - u) * p0 + u * p1;
    }

    Part FindPart(string n) 
    {
        foreach (Part prt in parts)
        {
            if (prt.name == n)
            {
                return (prt);
            }
        }
        return (null);
    }

    Part FindPart(GameObject go) 
    {
        foreach (Part prt in parts)
        {
            if (prt.go == go)
            {
                return (prt);
            }
        }
        return (null);
    }

    public bool Destroyed(GameObject go) 
    {
        return (Destroyed(FindPart(go)));
    }

    public bool Destroyed(string n) 
    {
        return (Destroyed(FindPart(n)));
    }

    public bool Destroyed(Part prt) 
    {
        if (prt == null)
        {
            return (true);
        }
        return (prt.health <= 0);
    }

    void ShowLocalizedDamage(Material m) 
    {
        m.color = Color.red;
        damageDoneTime = Time.time + showDamageDuration;
        showingDomage = true;
    }

   public new void OnCollisionEnter(Collision collision)
    {
        GameObject other = collision.gameObject;
        switch (other.tag)
        {
            case "ProjectilleHero":
                Projectile p = other.GetComponent<Projectile>();
                if (!bndCheck.isOnScreen)
                {
                    Destroy(other);
                    break;
                }
                GameObject goHit = collision.contacts[0].thisCollider.gameObject;
                Part priHit = FindPart(goHit);
                if (priHit == null) 
                {
                    goHit = collision.contacts[0].otherCollider.gameObject;
                    priHit = FindPart(goHit);
                }

                if (priHit.projectedBy!=null)
                {
                    foreach (string s in priHit.projectedBy)
                    {
                        if (!Destroyed(s))
                        {
                            Destroy(other);
                            return;
                        }
                    }
                }
                priHit.health -= Main.GetWeaponDefinition(p.type).damageOnHit;
                ShowLocalizedDamage(priHit.mat);
                if (priHit.health <= 0)
                {
                    priHit.go.SetActive(false);
                }
                bool allDestroyed = true;
                foreach (var prt in parts)
                {
                    if (!Destroyed(prt))
                    {
                        allDestroyed = false;
                        break;
                    }
                }
                if (allDestroyed)
                {
                    Main.S.SpihDestroyed(this);
                    Destroy(this.gameObject);
                }
                Destroy(other);
                break;
        }
    }
}
