using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerHealth : MonoBehaviour {

    [SerializeField]
    [Range(1, 20)]
    int MaxHealth = 5;
    [SerializeField]
    [Range(0, 2)]
    float MercyInvulnerability = 0;
    [SerializeField]
    Texture HeartTexture;

    PlayerControler PC;
    public RespawnController Resetter;

    bool mercy;

    int hp;
    int CurrentHealth
    {
        get { return hp; }
        set { hp = Mathf.Clamp(value, 0, MaxHealth); }
    }

    // Use this for initialization
    void Start ()
    {
        mercy = false;
        CurrentHealth = MaxHealth;
        PC = gameObject.GetComponent<PlayerControler>();
	}
	
	// Update is called once per frame
	void Update ()
    {
		if (CurrentHealth == 0 || Input.GetKeyDown(KeyCode.R))
        {
            //Death?
            PC.Respawn();
            CurrentHealth = MaxHealth;
            Resetter.ResetPositions();
        }
	}

    public void Hurt(int damage)
    {
        if (!mercy)
        {
            CurrentHealth -= damage;
            StartCoroutine(MercyTimer());
        }
    }
    public void Heal(int damage)
    {
        CurrentHealth += damage;
    }

    IEnumerator MercyTimer()
    {
        mercy = true;
        yield return new WaitForSeconds(MercyInvulnerability);
        mercy = false;
        //Possibility of graphical feedback
        //Updates 10 times per second; could notify animations and the likes
        //yield return new WaitForSeconds(.1f);
    }


    [SerializeField]
    [Range(10, 100)]
    int HeartSize;
    void OnGUI()
    {
        GUI.BeginGroup(new Rect(10, 10, HeartSize * MaxHealth, HeartSize));
        for (int i = 0; i < MaxHealth; i++)
        {
            float empty_offset = (i < CurrentHealth ? 1 : 0)/2f;
            GUI.DrawTextureWithTexCoords(new Rect(i * HeartSize, 0, HeartSize, HeartSize), HeartTexture, new Rect(0, empty_offset, 1, 1/2f));
        }
       GUI.EndGroup();
    }
}
