using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LerpMovement : MonoBehaviour
{
    public float m_moveTime = 0.1f;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void MoveTo(Vector3 newPos)
    {
        StartCoroutine(MoveCoroutine(newPos));
    }

    private IEnumerator MoveCoroutine(Vector3 newPos)
    {
        Vector3 oldPos = transform.position;

        for (float t = 0; t < m_moveTime; t += Time.deltaTime)
        {
            float p = t / m_moveTime;
            transform.position = Vector3.Lerp(oldPos, newPos, p);
            yield return null;
        }

        transform.position = newPos;
    }
}
