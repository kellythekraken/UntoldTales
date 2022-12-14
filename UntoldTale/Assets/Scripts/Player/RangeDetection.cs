using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class RangeDetection : MonoBehaviour
{
    [SerializeField] List<Befriendable> befriendList;
    [SerializeField] Light2D areaLight;
    [SerializeField] TailMovement tailScript;
    LineRenderer tailrender;
    public float befriendSpeed = 1f;    //could be modified as you become better at it
    public float exhaustSpeed = .1f;      //will DOUBLE if you befriending multiple at once
    public int exhaustRate = 0;         //how many you're socializing at once
    public float rechargeSpeed = .1f;
    public float socialBattery = 1;   //reduce when you're befriending blobs
    public Gradient exhaustColor;
    private Gradient healthyColor;

    GradientColorKey[] healthyColorKeys,exhaustColorKeys;
    void OnEnable() =>befriendList = new List<Befriendable>();
    void Start()
    {
        tailrender = tailScript.GetComponent<LineRenderer>();
        healthyColor = tailrender.colorGradient;
        healthyColorKeys = healthyColor.colorKeys;
        exhaustColorKeys = exhaustColor.colorKeys;
    }
    void FixedUpdate()
    {
        UpdateExhaustionParam();
        if(exhaustRate>0 && socialBattery>0f) //socializing, reduce social battery
        {
            socialBattery -= exhaustSpeed * exhaustRate * Time.deltaTime;
            ChangeExhaustionColor();
            for(int i = 0; i<exhaustRate;i ++) 
            {
                var friend = befriendList[i];
                if (friend.friendliness>=100)
                {
                    friend.befriended = true;
                    exhaustRate --;
                    befriendList.Remove(friend);
                }
                else if(friend.friendliness<100) 
                {
                    friend.friendliness += befriendSpeed * Time.deltaTime;
                    friend.StartBefriending();
                }
            }
            if(socialBattery <.05f) tailScript.CurlUp();
        }
        else if(exhaustRate == 0 && socialBattery<1) //alone time. increase social battery
        {
            socialBattery += rechargeSpeed * Time.deltaTime;
            ChangeExhaustionColor();
        }
    }

    void UpdateExhaustionParam()
    {
        AudioManager.Instance.SetGlobalParam("Energy",socialBattery);
    }
    private void ChangeExhaustionColor()
    {
        areaLight.intensity = socialBattery;
        tailrender.colorGradient = GradientLerp(exhaustColor,healthyColor,socialBattery,false,false);
    }
    void OnTriggerEnter2D(Collider2D collider)
    {
        if(collider.CompareTag("FriendToBe")) 
        {
            exhaustRate ++;
            befriendList.Add(collider.GetComponentInParent<Befriendable>());
        }
    }

    void OnTriggerExit2D(Collider2D collider)
    {
        if(collider.CompareTag("FriendToBe")) 
        {
            exhaustRate --;
            befriendList.Remove(collider.GetComponentInParent<Befriendable>());
        }
    }

    private Gradient GradientLerp(Gradient a, Gradient b, float t, bool noAlpha, bool noColor) 
    {
        //list of all the unique key times
        var keysTimes = new List<float>();

        if (!noColor) {
            for (int i = 0; i < a.colorKeys.Length; i++) {
                float k = a.colorKeys[i].time;
                if (!keysTimes.Contains(k))
                    keysTimes.Add(k);
            }

            for (int i = 0; i < b.colorKeys.Length; i++) {
                float k = b.colorKeys[i].time;
                if (!keysTimes.Contains(k))
                    keysTimes.Add(k);
            }
        }

        if (!noAlpha) {
            for (int i = 0; i < a.alphaKeys.Length; i++) {
                float k = a.alphaKeys[i].time;
                if (!keysTimes.Contains(k))
                    keysTimes.Add(k);
            }

            for (int i = 0; i < b.alphaKeys.Length; i++) {
                float k = b.alphaKeys[i].time;
                if (!keysTimes.Contains(k))
                    keysTimes.Add(k);
            }
        }

        GradientColorKey[] clrs = new GradientColorKey[keysTimes.Count];
        GradientAlphaKey[] alphas = new GradientAlphaKey[keysTimes.Count];

        //Pick colors of both gradients at key times and lerp them
        for (int i = 0; i < keysTimes.Count; i++) {
            float key = keysTimes[i];
            var clr = Color.Lerp(a.Evaluate(key), b.Evaluate(key), t);
            clrs[i] = new GradientColorKey(clr, key);
            alphas[i] = new GradientAlphaKey(clr.a, key);
        }

        var g = new Gradient();
        g.SetKeys(clrs, alphas);

        return g;
    }
}
