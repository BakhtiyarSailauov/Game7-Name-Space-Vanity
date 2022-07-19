using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    private BoundsScript bndCheck;
    private Renderer rend;

    [Header("Set in Inspector")]
    public Rigidbody rigid;
    [SerializeField]
    private WeaponType _type;

    public WeaponType type 
    {
        get { return (_type); }
        set { SetType(value); }
    }

    public void Awake()
    {
        bndCheck = GetComponent<BoundsScript>();
        rend = GetComponent<Renderer>();
        rigid = GetComponent<Rigidbody>();
    }

    public void Update()
    {
        if (bndCheck.offUp)
        {
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// Изменяет скрытое поле _type , и меняет цвет
    /// этого снаряда, как определено в главном обьекте.
    /// </summary>
    /// <param name="eType">тип используемого оружие</param>
    public void SetType(WeaponType eType) 
    {
        _type = eType;
        WeaponDefinition def = Main.GetWeaponDefinition(_type);
        rend.material.color = def.projectileColor;
    }
}
