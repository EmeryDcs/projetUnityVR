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
    public LineRenderer [] figures;

    // private Vector2 [] points;
    // private Vector2 [][] figuresVector2;
    private float largeur = 0.01f;
    private GameObject tipMainDroite;
    private LineRenderer dessinEnCours;
    private int index = 0;
    private int timer = 0;

    void Start(){
        //Je récupère le bout du doigt pour plus d'immersion
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
                dessinComparaison();
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

    public void dessinComparaison(){
        sort2d.positionCount = dessinEnCours.positionCount;

        Vector2 difference = -conversion3dvers2d(dessinEnCours.GetPosition(0));

        for (int i = 0; i < dessinEnCours.positionCount; i++)
        {
            Vector2 point2d = conversion3dvers2d(dessinEnCours.GetPosition(i));
            sort2d.SetPosition(i, (point2d+difference));

            Destroy(dessinEnCours.gameObject);
        }

        sort2d = reducTaille(sort2d);
        estUneFigure(sort2d);
    }

    public LineRenderer reducTaille(LineRenderer figure)
    {
        float tailleMaxVecteur = 0;
        for (int i=0; i<figure.positionCount; i++)
        {
            for (int j=0; j<figure.positionCount; j++)
            {
                float tailleVecteur = (figure.GetPosition(i) - figure.GetPosition(j)).magnitude;
                if (tailleVecteur > tailleMaxVecteur)
                {
                    tailleMaxVecteur = tailleVecteur;
                }
            }
        }

        for (int i=0; i<figure.positionCount; i++)
        {
            figure.SetPosition(i, figure.GetPosition(i)/(tailleMaxVecteur/2));
        }

        return figure;
    }

    public void estUneFigure(LineRenderer sortCast)
    {
        float comparaison = 10;
        float tmpComparaison = 10000;
        int indiceFigure = 0;

        for (int i = 0; i < figures.Length; i++)
        {
            if (sortCast.positionCount < figures[i].positionCount)
            {
                comparaison = comparerFigures(sortCast, figures[i]);
            } 
            else if (sortCast.positionCount > figures[i].positionCount) 
            {

                comparaison = comparerFigures(figures[i], sortCast);
            } else {
                comparaison = comparerFigures(sortCast, figures[i]);
            }
            if (comparaison<tmpComparaison)
            {
                tmpComparaison = comparaison;
                indiceFigure = i;
            }
        }

        if (tmpComparaison < 0.2){
            Debug.Log("C'est la figure : "+figures[indiceFigure].gameObject.name);
            Debug.Log("La comparaison est de : "+tmpComparaison);
        } else {
            Debug.Log("Ce n'est pas une figure");
            Debug.Log("La comparaison est de : "+tmpComparaison);
        }
    }

    public float comparerFigures(LineRenderer petiteFigure, LineRenderer grandeFigure){
        int nombrePointsPetiteFigure = petiteFigure.positionCount;
        int nombrePointsGrandeFigure = grandeFigure.positionCount;
        float margeErreur = 0;
        float distance = 0;
        float distanceTampon = 100;
        //Dans ces boucles, on compare chaque point de la petite figure à chaque point de la grande figure, afin de savoir si la petite figure est bien une partie de la grande figure.
        for (int i = 0; i < nombrePointsPetiteFigure; i++)
        {
            distance = 0;
            distanceTampon = 100;
            for (int j = 0; j < nombrePointsGrandeFigure ; j++)
            {
                distance = (petiteFigure.GetPosition(i) - grandeFigure.GetPosition(j)).magnitude;
                if (distance < distanceTampon){
                    distanceTampon = distance;
                }
            }
            margeErreur += distanceTampon;
        }
        return margeErreur/nombrePointsPetiteFigure;
    }

    public Vector2 conversion3dvers2d(Vector3 vecteur){
        Matrix4x4 matriceProjection = camera.projectionMatrix;
        Matrix4x4 matriceVue = camera.worldToCameraMatrix;
        Matrix4x4 matriceProjectionCombine = matriceProjection * matriceVue;

        Vector3 point = vecteur;
        Vector3 pointProjete = matriceProjectionCombine.MultiplyPoint3x4(point);
        Vector2 point2d = new Vector2(
            ((pointProjete.x / pointProjete.z) + 1) / 2 * Screen.width,
            ((pointProjete.y / pointProjete.z) + 1) / 2 * Screen.height
        );
        return point2d;
    }
}
