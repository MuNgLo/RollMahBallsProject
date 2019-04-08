using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Twinstick;

public class LookAtObject : MonoBehaviour
{
    public LayerMask blockers;
    public GameObject target;
    public TwinstickInput tw;
    public float maxDistance = 2.0f;
    public float vOffset = 1.5f;
    public float vFocus = 2.5f;
    public bool mouseCam = true;
    public bool debug = true;
    private Vector3 hOffset = Vector3.forward;
    private void Start()
    {
        SetTargetToLookAt(target);
    }
    // Update is called once per frame
    void Update()
    {
        if (target)
        {
            if (!mouseCam)
            {
                if (tw.RightStickVector != Vector3.zero)
                {
                    hOffset = Quaternion.Euler(0.0f, tw.RightStickVector.x, 0.0f) * hOffset;
                    vOffset = Mathf.Clamp(vOffset + tw.RightStickVector.z * 0.25f, 0.5f, 2.5f);
                }
            }
            else
            {
                hOffset = Quaternion.Euler(0.0f, Input.GetAxis("Mouse X"), 0.0f) * hOffset;
                vOffset = Mathf.Clamp(vOffset + Input.GetAxis("Mouse Y") * 0.025f, 0.1f, 1.5f);
            }
            Vector3 castOrigin = target.transform.position + Vector3.up * vFocus;
            float currentDistance = maxDistance;
            Ray cast = new Ray(castOrigin, hOffset);
            RaycastHit hitinfo = new RaycastHit();
            if(Physics.Raycast(cast,out hitinfo, currentDistance, blockers))
            {
                currentDistance = hitinfo.distance - 0.2f;
            }
            Vector3 newPos = castOrigin + hOffset * currentDistance;

            this.transform.position = newPos;
            this.transform.LookAt(target.transform.position + Vector3.up * vOffset);
            if (debug)
            {
                Debug.DrawLine(cast.origin, castOrigin + cast.direction * currentDistance, Color.red);
                Debug.DrawLine(cast.origin + Vector3.up * 0.05f, newPos, Color.yellow);
            }
        }
    }

    public void SetTargetToLookAt(GameObject obj)
    {
        target = obj;
        tw = target.GetComponent<TwinstickInput>();
        target.GetComponent<Mover>().SetTranslationTransform(this.transform);
    }
}
