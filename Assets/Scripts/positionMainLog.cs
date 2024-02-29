using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class positionMainLog : MonoBehaviour
{
    public bool dessin = false;
    public Camera camera;
    public GameObject mainDroite;
    public LineRenderer sort2d;
    public Material materialDessin;

    private float largeur = 0.01f;

    private GameObject tipMainDroite;
    private LineRenderer dessinEnCours;
    private int index = 0;
    private int timer = 0;

    void Start(){
        tipMainDroite = mainDroite.transform.GetChild(0).GetChild(0).GetChild(0).GetChild(0).GetChild(0).gameObject;
    }

    void Update()
    {
        if (dessin)
        {
            dessiner();
        } 
        else if (dessinEnCours != null)
        {
            timer++;
            if (timer > 29) //Ce if me permet d'éviter de supprimer le dessin trop tôt si jamais la reconnaissance n'est pas fluide.
            {
                timer = 0;
                dessinVers2d();
                dessinEnCours = null;
            } else {
                dessiner();
            }
        }
    }

    private void dessiner()
    {
        if (dessinEnCours == null)
        {
            index = 0;
            dessinEnCours = new GameObject().AddComponent<LineRenderer>();
            dessinEnCours.material = materialDessin;
            dessinEnCours.startColor = dessinEnCours.endColor = Color.red;
            dessinEnCours.startWidth = dessinEnCours.endWidth = largeur;
            dessinEnCours.positionCount = 1;
            dessinEnCours.SetPosition(0, tipMainDroite.transform.position);
        } 
        else
        {
            var positionActuelle = dessinEnCours.GetPosition(index);
            if (Vector3.Distance(positionActuelle, tipMainDroite.transform.position) > 0.001f)
            {
                index++;
                dessinEnCours.positionCount = index + 1;
                dessinEnCours.SetPosition(index, tipMainDroite.transform.position);
            }
        }
    }

    public void printIsFalse()
    {
        dessin = false;
    }

    public void printIsTrue()
    {
        dessin = true;
    }

    public void dessinVers2d(){
        Matrix4x4 matriceProjection = camera.projectionMatrix;
        Matrix4x4 matriceVue = camera.worldToCameraMatrix;
        Matrix4x4 matriceProjectionCombine = matriceProjection * matriceVue;

        sort2d.positionCount = dessinEnCours.positionCount;

        Vector2 difference = -(new Vector2(
            ((matriceProjectionCombine.MultiplyPoint3x4(dessinEnCours.GetPosition(0)).x / matriceProjectionCombine.MultiplyPoint3x4(dessinEnCours.GetPosition(0)).z) + 1) / 2 * Screen.width,
            ((matriceProjectionCombine.MultiplyPoint3x4(dessinEnCours.GetPosition(0)).y / matriceProjectionCombine.MultiplyPoint3x4(dessinEnCours.GetPosition(0)).z) +1) / 2 * Screen.height
        ));

        for (int i = 0; i < dessinEnCours.positionCount; i++)
        {
            Vector3 point = dessinEnCours.GetPosition(i);
            Vector3 pointProjete = matriceProjectionCombine.MultiplyPoint3x4(point);
            Vector2 point2d = new Vector2(
                ((pointProjete.x / pointProjete.z) + 1) / 2 * Screen.width,
                ((pointProjete.y / pointProjete.z) + 1) / 2 * Screen.height
            );
            sort2d.SetPosition(i, (point2d+difference)/100);
            Debug.Log(point2d);
            Destroy(dessinEnCours.gameObject);
        }
    }
}
