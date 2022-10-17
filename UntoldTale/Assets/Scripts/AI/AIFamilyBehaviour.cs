using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIFamilyBehaviour : MonoBehaviour
{
    Transform wormi;
    Rigidbody2D centerRb;
    float calculatedRadius;
    [SerializeField] float followSpeed;
    [SerializeField] float defaultForce = 500f;

    void Start()
    {
        wormi = HeadMovement.Instance.transform;
        centerRb = GetComponentInChildren<Rigidbody2D>();
        calculatedRadius = transform.localScale.x * GetComponent<CircleCollider2D>().radius +0.5f;
        GameManager.Instance.GameStartEvent.AddListener(GameStarInit);
    }

    IEnumerator FamilyOnGameStartBehaviour()
    {
        //hug
        var timeElapsed = 0f;
        while(timeElapsed < 5f)
        {
            if(Vector2.Distance(centerRb.position,wormi.position) > calculatedRadius)
            {
                Vector2 newPosition = Vector2.Lerp(centerRb.position,wormi.position, followSpeed * Time.fixedDeltaTime);
                centerRb.MovePosition(newPosition);
            }
            else {yield break;}

            timeElapsed += Time.deltaTime;
            yield return null;
        }
        yield break;

        var startPos = centerRb.position;

        while(timeElapsed < 3f)
        {

            //Vector2 newPosition = Vector2.Lerp(startPos, wormi.position, timeElapsed/3f);
            centerRb.MovePosition(wormi.position * Time.fixedDeltaTime);
            timeElapsed += Time.fixedDeltaTime;
            yield return null;
        }

/*
        //move away
        timeElapsed = 0f;
        Debug.Log("start moving away");
        Vector2 direction = transform.position - wormi.transform.position;
        direction.Normalize();
        centerRb.transform.right = direction;
        //centerRb.AddForce(centerRb.transform.right * force,ForceMode2D.Impulse);
        while(timeElapsed < 2f)
        {
            centerRb.AddForce(centerRb.transform.right *5f * centerRb.mass);

            timeElapsed += Time.fixedDeltaTime;
            yield return null;
        }
        Debug.Log("stop moving away");
          */

        /*behaviour:
            - slowly march towards wormi and hug tight
            - after a few seconds, move away slowly and leave her a trail
            - now you can move, the path to friends and tunnel 
            - pathfinding? follow a path
          */  
    }

    void GameStarInit()
    {
        StartCoroutine(FamilyOnGameStartBehaviour());
    }

    void ForceBehaviour(float force)
    {
        centerRb.transform.right = (Vector2)wormi.position - centerRb.position;
        centerRb.AddForce(centerRb.transform.right * force,ForceMode2D.Impulse);
    }
    IEnumerator FollowBehaviour()
    {
        var timeElapsed = 0f;
        while(timeElapsed < 5f)
        {
            if(Vector2.Distance(centerRb.position,wormi.position) > calculatedRadius)
            {
                Vector2 newPosition = Vector2.Lerp(centerRb.position,wormi.position, followSpeed * Time.deltaTime);
                centerRb.MovePosition(newPosition);
            }
            else {yield break;}

            timeElapsed += Time.deltaTime;
            yield return null;
        }
    }
    void FloatBehaviour()
    {
        //randomly move around, like a boil animation
    }

}
