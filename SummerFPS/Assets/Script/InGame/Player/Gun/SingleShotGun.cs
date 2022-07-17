﻿using System;
using System.Collections;
using Photon.Pun;
using UnityEngine;

public class SingleShotGun : Gun
{
    [SerializeField] Camera cam;

    PhotonView PV;
    private bool canShoot = true;
    [SerializeField, Range(0, 1f)] private float fireRate;
    
    void Awake()
    {
        PV = GetComponent<PhotonView>();
    }

    public override void Use()
    {
        //Shoot();
        StartCoroutine(Fire());
    }
    
    private IEnumerator Fire()
    {
        canShoot = false;
        yield return new WaitForSeconds(fireRate);
        var shot = ShotPool.Instance.Get();
        shot.transform.position = new Vector3(transform.position.x - 0.1f,transform.position.y,transform.position.z);
        shot.transform.rotation = transform.rotation;
        
        shot.gameObject.SetActive(true);
        canShoot = true;
        PV.RPC("TakeHitRPC", RpcTarget.All);
        
        
    }
    

    void Shoot()
    {
        Ray ray = cam.ViewportPointToRay(new Vector3(0.5f, 0.5f));
        ray.origin = cam.transform.position;
        if(Physics.Raycast(ray, out RaycastHit hit))
        {
            Debug.Log("raytranfrom name " + hit.transform);
            hit.collider.gameObject.GetComponent<IDamageable>()?.TakeDamage(((GunInfo)itemInfo).damage);
            Health enemyHealth = hit.transform.GetComponent<Health>();
            if(enemyHealth != null)
            {
                Debug.Log("Shoot ");
                enemyHealth.TakeDamage(500);
            }
            
        }
    }

  
    [PunRPC]
    public IEnumerator TakeHitRPC()
    {
        canShoot = false;
        yield return new WaitForSeconds(fireRate);
        var shot = ShotPool.Instance.Get();
        shot.transform.position = new Vector3(transform.position.x - 0.1f,transform.position.y,transform.position.z);
        shot.transform.rotation = transform.rotation;
        
        shot.gameObject.SetActive(true);
        canShoot = true;
        
          
        Ray ray = cam.ViewportPointToRay(new Vector3(0.5f, 0.5f));
        ray.origin = cam.transform.position;
        if(Physics.Raycast(ray, out RaycastHit hit))
        {
            Debug.Log("raytranfrom name " + hit.transform);
            hit.collider.gameObject.GetComponent<IDamageable>()?.TakeDamage(((GunInfo)itemInfo).damage);
            Health enemyHealth = hit.transform.GetComponent<Health>();
            if(enemyHealth != null)
            {
                Debug.Log("Shoot ");
                enemyHealth.TakeDamage(500);
            }
            
        }
       
    }
    

}