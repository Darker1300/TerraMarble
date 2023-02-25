using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityUtility;
using MathUtility;

public class EnemyHealth : MonoBehaviour
{
    [SerializeField] private LayerMask projLayerName;
    public float CurrentDelayTime;
    [SerializeField] private float KnockBackDuration = 1.25f;
    public UnityEvent<Collider2D> OnProjectileHit;
    public UnityEvent OnStunEnd;
    private Rigidbody2D rb;
    [SerializeField]
    private float knockBackForce = 100;

    public int HitAmount;

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
        else
        {
            OnStunEnd?.Invoke();
            
        }
    }
   

    public void OnHit()
    {
        CurrentDelayTime = KnockBackDuration;
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

        HitAmount++;
        rb.velocity = Vector2.zero;
        OnHit();
        rb.AddForce(colider.transform.Towards(transform).normalized * knockBackForce);




    }


}
