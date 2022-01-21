using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Mover : Fighter
{
    protected BoxCollider2D boxCollider;
    public Rigidbody2D rb;
    public Vector3 moveDelta;
    protected RaycastHit2D hit;
    public float ySpeed = 0.75f;
    public float xSpeed = 1.0f;

    protected virtual void Start() {

        boxCollider = GetComponent<BoxCollider2D>();
    }

    protected virtual void UpdateMotor(Vector3 input) {

        //Reset moveDelta
        moveDelta = new Vector3(input.x * xSpeed, input.y * ySpeed, 0);

        //Swap sprite direction based on movement left or right
        if (moveDelta.x > 0)
        {
            transform.localScale = Vector3.one;
        }
        else if (moveDelta.x < 0)
        {
            transform.localScale = new Vector3(-1, 1, 1);
        }

        //add pushforce vector if any
        moveDelta += pushDirection;

        //reduce push force everyframe based off recovery speed
        pushDirection = Vector3.Lerp(pushDirection, Vector3.zero, pushRecoverySpeed);


        //Make sure we can move in the direction by casting a box there first if the box is null we are clear to move.
        hit = Physics2D.BoxCast(transform.position, boxCollider.size, 0, new Vector2(0, moveDelta.y), Mathf.Abs(moveDelta.y * Time.deltaTime), LayerMask.GetMask("Actor", "Blocking"));
        if (hit.collider == null)
        {
            //Make Move
            transform.Translate(0, moveDelta.y * Time.deltaTime, 0);

        }

        hit = Physics2D.BoxCast(transform.position, boxCollider.size, 0, new Vector2(moveDelta.x, 0), Mathf.Abs(moveDelta.x * Time.deltaTime), LayerMask.GetMask("Actor", "Blocking"));
        if (hit.collider == null)
        {
            //Make Move
            transform.Translate(moveDelta.x * Time.deltaTime, 0, 0);

        }

    }





}
