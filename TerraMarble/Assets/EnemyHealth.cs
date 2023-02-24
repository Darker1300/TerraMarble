using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityUtility;
using MathUtility;

public class EnemyHealth : MonoBehaviour
{
    [SerializeField] private LayerMask projLayerName = LayerMaskUtility.Create("Projectile");
    public float CurrentDelayTime;
    [SerializeField] private float KnockBackDuration;
    public UnityEvent<Collider2D> OnProjectileHit;
    private Rigidbody2D rb;
    [SerializeField]
    private float knockBackForce =100;

    // Start is called before the first frame update
    void Start()
    {
        
        rb = GetComponent<Rigidbody2D>();
        
    }

    // Update is called once per frame
    void Update()
    {

        if (CurrentDelayTime > 0)
        {
            CurrentDelayTime -= Time.deltaTime;
        }
       // else enabled = false;
    }
    private void OnEnable()
    {
        CurrentDelayTime += KnockBackDuration;
        
    }

    public void OnHit()
    {
        CurrentDelayTime += KnockBackDuration;
    }
    private void OnDisable()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log("KnockBack" + collision.gameObject.name);
        if (collision.gameObject.CompareLayerMask(projLayerName))
        {
            OnProjectileHit?.Invoke(collision);
        }
    }
    public void KnockBack(Collider2D colider)
    {
        
        if (rb == null)
            return;

        rb.velocity = Vector2.zero;

        rb.AddForce(colider.transform.Towards(transform).normalized * knockBackForce);


        
    }


}
