using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBall : MonoBehaviour
{
    public PlayerInput playerInput;
    public PlayerHealth playerHealth;
    public Rigidbody2D ballRigidBody;
    public TreeBend treeBend;
    public Wheel wheel;
    

    void Start()
    {
        wheel = FindObjectOfType<Wheel>(true);
        ballRigidBody = GetComponent<Rigidbody2D>();
        playerHealth = GetComponent<PlayerHealth>();
        playerInput = GetComponent<PlayerInput>();
        treeBend = FindObjectOfType<TreeBend>(true);
    }
    
    void Update()
    {
        
    }
}
