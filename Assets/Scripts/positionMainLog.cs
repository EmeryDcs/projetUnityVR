using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class positionMainLog : MonoBehaviour
{
    public bool dessin = false;
    public GameObject mainDroite;

    public Material materialDessin;
    private float largeur = 0.01f;

    private LineRenderer dessinEnCours;
    private int index = 0;
    private int i = 0;

    void Update()
    {
        if (dessin)
        {
            // positionLog();
            dessiner();
        } 
        else if (dessinEnCours != null)
        {
            i = 0;
            dessinEnCours = null;
        }
    }

    private void dessiner()
    {
        Debug.Log(i++);
        if (dessinEnCours == null)
        {
            index = 0;
            dessinEnCours = new GameObject().AddComponent<LineRenderer>();
            dessinEnCours.material = materialDessin;
            dessinEnCours.startColor = dessinEnCours.endColor = Color.red;
            dessinEnCours.startWidth = dessinEnCours.endWidth = largeur;
            dessinEnCours.positionCount = 1;
            dessinEnCours.SetPosition(0, mainDroite.transform.position);
        } 
        else
        {
            var positionActuelle = dessinEnCours.GetPosition(index);
            if (Vector3.Distance(positionActuelle, mainDroite.transform.position) > 0.001f)
            {
                index++;
                dessinEnCours.positionCount = index + 1;
                dessinEnCours.SetPosition(index, mainDroite.transform.position);
            }
        }
    }

    private void positionLog()
    {
        Debug.Log("Position : " + mainDroite.transform.position);
    }

    public void printIsFalse()
    {
        dessin = false;
    }

    public void printIsTrue()
    {
        dessin = true;
    }
}
