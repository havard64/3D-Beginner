using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float turnSpeed = 20f;

    Animator m_Animator;
    Rigidbody m_Rigidbody;
    AudioSource m_AudioSource;
    Vector3 m_Movement;
    Quaternion m_Rotation = Quaternion.identity;

    public AudioClip clash;
    public AudioClip footStep;

    float timeClipPlay = 0.0f;

    float knockRadius = 5.0f;

    IEnumerator PlayKnock()
    {
        m_AudioSource.PlayOneShot(clash);
        yield return new WaitForSeconds(clash.length);
        
    }
    void Start ()
    {
        m_Animator = GetComponent<Animator> ();
        m_Rigidbody = GetComponent<Rigidbody> ();
        m_AudioSource = GetComponent<AudioSource>();
 
    }

    void FixedUpdate ()
    {
        if (Input.GetKey("space"))
        {
            timeClipPlay = 0.0f;
            StartCoroutine(PlayKnock()); // Play audio file

            // Create the sphere collider
            Collider[] hitColliders = Physics.OverlapSphere(transform.position, knockRadius);
            for (int i = 0; i < hitColliders.Length; i ++) // check the collosions
            {
                // if it's a guard, trigger the investigation
                if (hitColliders[i].tag == "Guard")
                {
                    hitColliders[i].GetComponent<WaypointPatrol>().InvestigatePoint(this.transform.position);
                }
            }
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }
        
        float horizontal = Input.GetAxis ("Horizontal");
        float vertical = Input.GetAxis ("Vertical");
        
        m_Movement.Set(-1*horizontal, 0f, -1*vertical);
        m_Movement.Normalize ();

        bool hasHorizontalInput = !Mathf.Approximately (horizontal, 0f);
        bool hasVerticalInput = !Mathf.Approximately (vertical, 0f);
        bool isWalking = hasHorizontalInput || hasVerticalInput;
        m_Animator.SetBool ("IsWalking", isWalking);
        if (isWalking)
        {
            if (!m_AudioSource.isPlaying)
            {
                m_AudioSource.Play();  
            }
        
        //else if (Input.GetKey("space") && !isWalking && timeClipPlay < 1.0)
        //{
           //timeClipPlay += Time.deltaTime;
           //m_AudioSource.PlayOneShot(footStep, 0f);
           //m_AudioSource.PlayOneShot(clash, 1f);
        //}
        }
        else
        {
            m_AudioSource.Stop(); 
            //m_AudioSource.PlayOneShot(footStep, 0f);
        }

        Vector3 desiredForward = Vector3.RotateTowards (transform.forward, m_Movement, turnSpeed * Time.deltaTime, 0f);
        m_Rotation = Quaternion.LookRotation (desiredForward);

        
    }

    void OnAnimatorMove ()
    {
        m_Rigidbody.MovePosition (m_Rigidbody.position + m_Movement * m_Animator.deltaPosition.magnitude);
        m_Rigidbody.MoveRotation (m_Rotation);
    }
}
