using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class MoveTests : MonoBehaviour
{

    public float moveTimer = 0;

    public int state = 3;

    Vector3 originalPosition;

    Vector3 originalRotation;

    // Start is called before the first frame update
    void Start()
    {

        originalPosition = transform.position;

    }

    // Update is called once per frame
    void Update()
    {

        moveTimer += Time.deltaTime;

        switch (state)
        {

            case 0: //TRANSLATE

                if (moveTimer < 5f)
                    transform.Translate(new Vector3(0, 0, 1) * Time.deltaTime);

                else
                    ResetTest();

                break;



            case 1:  //LERP


                Vector3 currentPosition = originalPosition;
                Vector3 targetPosition = new Vector3(currentPosition.x, currentPosition.y, currentPosition.z + 5);

                transform.position = Vector3.Lerp(currentPosition, targetPosition, moveTimer);


                if (moveTimer > 3)
                    ResetTest();


                break;


            case 2:  //SLERP 


                Vector3 currentPositionSlerp = originalPosition;
                Vector3 targetPositionSlerp = new Vector3(currentPositionSlerp.x, currentPositionSlerp.y, currentPositionSlerp.z + 5);

                transform.position = Vector3.Lerp(currentPositionSlerp, targetPositionSlerp, moveTimer);


                if (moveTimer > 3)
                    ResetTest();


                break;



            case 3:  //Tweening Tests

                //transform.DOLocalMove(transform.position + new Vector3(1, 0, 1), 1f);

                transform.DOLocalMoveZ(originalPosition.z + 1, 1f);
                transform.DOLocalMoveX(originalPosition.x + 1, 1f);


                if (moveTimer > 5)
                    ResetTest();


                break;



            case 4: //tweening with coroutine


                if (moveTimer < 5)
                    StartCoroutine(TestTransforms());

                else
                {
                    StopAllCoroutines();
                    moveTimer = 0;
                }



                break;

            default:
                break;
        }






    }


    public void ResetTest()
    {

        moveTimer = 0;

        StopAllCoroutines();
    }



    public IEnumerator TestTransforms()
    {

        transform.DORotate(originalRotation + new Vector3(0, 45, 0), .5f);

        yield return new WaitForSeconds(2f);

        transform.DOLocalMoveZ(originalPosition.z + 1, 1f);

        yield return new WaitForSeconds(2f);

        transform.DOLocalMoveX(originalPosition.x + 1, 1f);

        yield return new WaitForSeconds(1f);

        transform.DOMove(originalPosition, 1f);


        yield return null;

    }


}
