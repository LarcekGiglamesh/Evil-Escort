using UnityEngine;
using System.Collections;

public class Parallaxing : MonoBehaviour
{

	// Beinhaltet Saemtliche zu Bewegende Hintergruende
	public Transform[] backgrounds;
	// Bestimmt die Staerke der Parallax Bewegung
	private float[] parallaxScales;
	// Smoothing
	public float smoothing = 1f;
	// Main Camera Transformation
	private Transform cam;
	// Letzte Kamera Position fuer den Schwank zur neuen
	private Vector3 previousCamPos;

	// Richtungsvorgabe
	private bool changeParallaxMovingDirection = true;

	// 
	private float factor = 5.0f;

	// Noch vor dem Start
	void Awake()
	{
		// Lese die Main Kamera
		cam = Camera.main.transform;
	}

	// Beim Start (nach der Awake)
	void Start()
	{
		// Richtungsvorgabe anpassen
		int direction = (changeParallaxMovingDirection ? 1 : -1);
		// lese letzte Kamera Position aus gemerkter Kamera
		previousCamPos = cam.position;
		// bestimme die Anzahl an Hintergruenden
		int backgroundCount = backgrounds.Length;
		// Bereite ein Feld fuer alle Hintergruende vor
		parallaxScales = new float[backgroundCount];
		// Iteriere ueber alle Hintergruende
		for (int i = 0; i < backgroundCount; i++)
		{
			// Staerke der Bewegung aus der Z Position ermitteln
			parallaxScales[i] = backgrounds[i].position.z * direction * factor;
		}
	}

	// Fuer jeden Frame
	void Update()
	{
		// bestimme die Anzahl an Hintegruenden
		int backgroundCount = backgrounds.Length;
		// Bewegung und ZielPosition in X vorbereiten
		float parallax, backgroundTargetPosX;
		// Ziel Position als Vektor
		Vector3 backgroundTargetPos;
		// Fuer jeden Hintergrund
		for (int i = 0; i < backgroundCount; i++)
		{
			// Bewegung bestimmen aus letzter Kamera Position zu aktueller ueber die Skala
			parallax = (previousCamPos.x - cam.position.x) * parallaxScales[i];
			// Ziel Position um Bewegung verschieben
			backgroundTargetPosX = backgrounds[i].position.x + parallax;
			// Ziel Position als Vektor
			backgroundTargetPos = new Vector3(backgroundTargetPosX, backgrounds[i].position.y, backgrounds[i].position.z);
			// Bewegung des Hintergrundes zur Zielposition ueber deltaTime
			backgrounds[i].position = Vector3.Lerp(backgrounds[i].position, backgroundTargetPos, smoothing * Time.deltaTime);
		}
		// Vergangene Kamera ist jetzige Kamera
		previousCamPos = cam.position;

	}
}
