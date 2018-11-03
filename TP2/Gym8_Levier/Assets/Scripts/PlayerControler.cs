﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControler : MonoBehaviour
{
    // Déclaration des constantes
    private static readonly Vector3 FlipRotation = new Vector3(0, 180, 0);
    private static readonly Vector3 CameraPosition = new Vector3(10, 1, 0);
    private static readonly Vector3 InverseCameraPosition = new Vector3(-10, 1, 0);

    // Déclaration des variables
    bool _Grounded { get; set; }
    bool _Flipped { get; set; }
    Animator _Anim { get; set; }
    Rigidbody _Rb { get; set; }
    Camera _MainCamera { get; set; }

    Transform InteractZone { get; set; }

    // Valeurs exposées
    [SerializeField]
    float MoveSpeed = 5.0f;

    [SerializeField]
    [Range(0, 1)]
    float SpeedFalloff = 0.1f;

    [SerializeField]
    float JumpForce = 10f;

    [SerializeField]
    float KnockbackForce = 5f;

    [SerializeField]
    LayerMask WhatIsGround;

    [SerializeField]
    LayerMask CanInteractWith;

    float _DEPTH;

    // Awake se produit avait le Start. Il peut être bien de régler les références dans cette section.
    void Awake()
    {
        _Anim = GetComponent<Animator>();
        _Rb = GetComponent<Rigidbody>();
        _DEPTH = _Rb.position.x;
        _MainCamera = Camera.main;
        InteractZone = transform.Find("InteractZone");
    }

    // Utile pour régler des valeurs aux objets
    void Start()
    {
        _Grounded = false;
        _Flipped = false;
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
        _Rb.position = new Vector3(_DEPTH, _Rb.position.y, _Rb.position.z);
        //if (_Grounded)
        //{
        //    _Rb.velocity = new Vector3(_Rb.velocity.x, _Rb.velocity.y, horizontal);
        //}
        //else
        {
            _Rb.velocity = new Vector3(_Rb.velocity.x, _Rb.velocity.y, Mathf.Clamp((_Rb.velocity.z * (1 - SpeedFalloff)) + horizontal/2, -MoveSpeed, MoveSpeed));
        }
        _Anim.SetFloat("MoveSpeed", Mathf.Abs(_Rb.velocity.z));
    }

    // Gère le saut du personnage, ainsi que son animation de saut
    void CheckJump()
    {
        if (_Grounded)
        {
            if (Input.GetButtonDown("Jump"))
            {
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
            _MainCamera.transform.Rotate(-FlipRotation);
            _MainCamera.transform.localPosition = InverseCameraPosition;
        }
        else if (horizontal > 0 && _Flipped)
        {
            _Flipped = false;
            transform.Rotate(-FlipRotation);
            _MainCamera.transform.Rotate(FlipRotation);
            _MainCamera.transform.localPosition = CameraPosition;
        }
    }

    // Collision avec le sol
    void OnCollisionEnter(Collision coll)
    {        
        // On s'assure de bien être en contact avec le sol
        if ((WhatIsGround & (1 << coll.gameObject.layer)) == 0)
            return;

        // Évite une collision avec le plafond
        if (coll.relativeVelocity.y > 0)
        {
            _Grounded = true;
            _Anim.SetBool("Grounded", _Grounded);
        }
    }

    public void Knockback()
    {
        _Rb.velocity = new Vector3(_Rb.velocity.x, 0, _Rb.velocity.z);
        //!? il faudrait revoir mouvement; appliquer une force aide vraiment pas
        _Rb.AddForce(new Vector3(0, 0.5f, 0) * KnockbackForce, ForceMode.Impulse);
        _Grounded = false;
    }

    private void CheckInteract()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            //Test for interactable
            Collider[] hitColliders = Physics.OverlapBox(InteractZone.position, InteractZone.localScale / 2, Quaternion.identity, CanInteractWith);
            //if found activate it
            foreach (Collider c in hitColliders)
            {
                //All component attached to the gameObject that have an "ActivateInteraction" function
                //will react to the message and call the function.
                c.gameObject.SendMessage("ActivateInteraction");
            }
            //Also notify Animator
            _Anim.SetTrigger("Interact");
        }
    }
}