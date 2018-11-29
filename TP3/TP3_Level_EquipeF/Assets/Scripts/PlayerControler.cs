using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControler : MonoBehaviour
{
    // Déclaration des constantes
    private static readonly Vector3 FlipRotation = new Vector3(0, 180, 0);
    public Vector3 SpawnPos;

    // Déclaration des variables
    bool _Grounded { get; set; }
    bool _Flipped { get; set; }
    bool _Knockedback { get; set; }
    Animator _Anim { get; set; }
    Rigidbody _Rb { get; set; }

    // Valeurs exposées
    [SerializeField]
    float MoveSpeed = 5.0f;
    //[SerializeField]
    //[Range(0, 1)]
    //float SpeedFalloff = 0.1f;
    [SerializeField]
    [Range(0.0f, 100)]
    float JumpForce = 10f;
    [SerializeField]
    [Range(0.0f, 10)]
    float KnockbackForce = 5f;

    [SerializeField]
    LayerMask WhatIsGround;
    [SerializeField]
    LayerMask CanInteractWith;

    [SerializeField]
    BoxCollider InteractZone;

    TorchActions torchAction;

    float _DEPTH;

    // Awake se produit avait le Start. Il peut être bien de régler les références dans cette section.
    void Awake()
    {
        _Anim = GetComponent<Animator>();
        _Rb = GetComponent<Rigidbody>();
        _DEPTH = _Rb.position.z;
        torchAction = GetComponentInChildren<TorchActions>();
    }

    // Utile pour régler des valeurs aux objets
    void Start()
    {
        SpawnPos = transform.position;
        _Grounded = false;
        _Flipped = false;
        _Knockedback = false;
    }

    // Vérifie les entrées de commandes du joueur
    void Update()
    {
        var horizontal = Input.GetAxis("Horizontal") * MoveSpeed;
        HorizontalMove(horizontal);
        FlipCharacter(horizontal);
        CheckJump();
        CheckInteract();
    }

    // Gère le mouvement horizontal
    void HorizontalMove(float horizontal)
    {
        //Lock onto plane
        _Rb.position = new Vector3(_Rb.position.x, _Rb.position.y, _DEPTH);

        /*
        //EXPERIMENTAL PHYSIC-Y MOVEMENT
        float acceleration = 0;
        if (!_Knockedback)
        {
            acceleration = horizontal / 2;
        }
        _Rb.velocity = new Vector3(_Rb.velocity.x, _Rb.velocity.y, Mathf.Clamp((_Rb.velocity.z * (1 - SpeedFalloff)) + acceleration, -MoveSpeed, MoveSpeed));
        */

        if (!_Knockedback)
            _Rb.velocity = new Vector3(Mathf.Clamp(horizontal, -MoveSpeed, MoveSpeed), _Rb.velocity.y, _Rb.velocity.z);
        _Anim.SetFloat("MoveSpeed", Mathf.Abs(_Rb.velocity.x));
    }

    private bool isGrounded()
    {
        CapsuleCollider coll = GetComponent<CapsuleCollider>();
        RaycastHit hit;
        return Physics.SphereCast(transform.position + new Vector3(0, coll.radius, 0), 3 * coll.radius / 4, -transform.up, out hit, 0.25f);
    }

    // Gère le saut du personnage, ainsi que son animation de saut
    void CheckJump()
    {
        if (Input.GetButtonDown("Jump"))
        {
            if (isGrounded())
            {
                //Debug.DrawLine(transform.position + new Vector3(0, coll.radius , 0), transform.position + new Vector3(0, coll.radius - 0.2f - 3 * coll.radius / 4, 0), Color.white, 25);
                //Debug.DrawLine(hit.transform.position, transform.position, Color.blue, 60);
                _Rb.velocity = new Vector3(_Rb.velocity.x, 0.0f, _Rb.velocity.z);
                _Rb.AddForce(new Vector3(0, JumpForce, 0), ForceMode.Impulse);
                _Grounded = false;
                _Anim.SetBool("Grounded", false);
                _Anim.SetBool("Jump", true);
            }
        }
    }

    // Gère l'orientation du joueur et les ajustements de la camera
    void FlipCharacter(float horizontal)
    {
        if (horizontal < 0 && !_Flipped)
        {
            _Flipped = true;
            transform.Rotate(FlipRotation);
        }
        else if (horizontal > 0 && _Flipped)
        {
            _Flipped = false;
            transform.Rotate(-FlipRotation);
        }
    }

    // Collision avec le sol
    void OnCollisionEnter(Collision coll)
    {        
        // On s'assure de bien être en contact avec le sol
        if ((WhatIsGround & (1 << coll.gameObject.layer)) == 0)
            return;

        // Évite une collision avec le plafond
        if (isGrounded())
        {
            _Grounded = true;
            _Knockedback = false;
            _Anim.SetBool("Grounded", _Grounded);
        }
    }

    public void Knockback(Vector3 direction)
    {
        if (!_Knockedback)
        {
            direction = new Vector3(0, /*direction.y*/0, direction.z).normalized;
            _Rb.velocity = new Vector3(_Rb.velocity.x, 0, _Rb.velocity.z);
            //Slight Upwards kick
            _Rb.AddForce(new Vector3(0, 0.5f * KnockbackForce, 0), ForceMode.Impulse);
            _Rb.AddForce(direction * KnockbackForce, ForceMode.Impulse);
            _Knockedback = true;
            StartCoroutine(KnockbackTimer());
        }

    }

    private void CheckInteract()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            bool interacted = false;
            if (!torchAction.currentlyHeld)
                interacted = torchAction.Pickup();

            if (!interacted)
            {
                //Test for interactable
                Collider[] hitColliders = Physics.OverlapBox(InteractZone.transform.position + InteractZone.center, InteractZone.size / 2, Quaternion.identity, CanInteractWith);
                //if found activate it
                foreach (Collider c in hitColliders)
                {
                    interacted = true;
                    Interactible inter = c.GetComponent<Interactible>();
                    inter.ActivateInteraction();
                }
            }

            //Still nothing? attempt drop
            if (!interacted)
                torchAction.Drop();
            else //Also notify Animator
                _Anim.SetTrigger("Pickup");
        }
    }

    void OnTriggerEnter(Collider col)
    {
        if (col.gameObject.tag == "Lava")
            Respawn();
    }

    public void Respawn()
    {
        transform.position = SpawnPos;
        _Rb.velocity = new Vector3(0,0,0);
        _Grounded = true;
        _Knockedback = false;
        _Anim.SetBool("Grounded", _Grounded);
    }

    IEnumerator KnockbackTimer()
    {
        yield return new WaitForSeconds(1);
        _Knockedback = false;
    }
}
