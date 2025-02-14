using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit; // Zorg ervoor dat je deze using hebt!

public class InteractableHighlight : MonoBehaviour
{
    public Material highlightMaterial;
    private Material[] originalMaterials;
    private Renderer objectRenderer;
    private bool isPlayerNearby = false;
    private bool isHeld = false;

    // Drempel voor nabijheid
    private float pickupDistance = 1.25f; // Stel de gewenste afstand in (bijvoorbeeld 2f)

    // Namen van de handcontrollers (pas deze aan als jouw setup anders heet)
    private string leftHandName = "Left Controller";
    private string rightHandName = "Right Controller";

    // Zorg ervoor dat je een XRGrabInteractable component hebt toegevoegd aan dit object
    private XRGrabInteractable grabInteractable;

    void Start()
    {
        objectRenderer = GetComponent<Renderer>();
        // Sla alle originele materialen op
        originalMaterials = objectRenderer.materials;

        // Zorg ervoor dat je de XRGrabInteractable ook hebt
        grabInteractable = GetComponent<XRGrabInteractable>();
        if (grabInteractable == null)
        {
            Debug.LogWarning("XRGrabInteractable niet gevonden op dit object!");
        }

        // Zet de interactie standaard uit
        grabInteractable.enabled = false;
    }

    void Update()
    {
        // Als het object vastgehouden wordt, zet dan de originele materialen terug
        if (isHeld)
        {
            objectRenderer.materials = originalMaterials;
            return;
        }

        // Zoek naar de VR-handcontrollers
        GameObject leftHand = GameObject.Find(leftHandName);
        GameObject rightHand = GameObject.Find(rightHandName);

        // Controleer of beide controllers bestaan
        if (leftHand == null || rightHand == null)
        {
            Debug.LogWarning("VR-handcontrollers niet gevonden! Controleer de namen in de Hierarchy.");
            return;
        }

        // Bereken afstand tot beide handen
        float distanceToLeftHand = Vector3.Distance(leftHand.transform.position, transform.position);
        float distanceToRightHand = Vector3.Distance(rightHand.transform.position, transform.position);

        // Als een van de handen dichtbij genoeg is
        if (distanceToLeftHand < pickupDistance || distanceToRightHand < pickupDistance)
        {
            if (!isPlayerNearby)
            {
                // Maak een nieuwe array met dezelfde lengte als het aantal materialen
                Material[] highlightMaterials = new Material[originalMaterials.Length];

                // Vul de array met het highlight-materiaal
                for (int i = 0; i < highlightMaterials.Length; i++)
                {
                    highlightMaterials[i] = highlightMaterial;
                }

                // Zet alle materialen naar het highlight-materiaal
                objectRenderer.materials = highlightMaterials;
                isPlayerNearby = true;

                // Zet de interactie aan als de speler dichtbij is
                grabInteractable.enabled = true; // Zet interactie aan
            }
        }
        else
        {
            if (isPlayerNearby)
            {
                // Zet alle materialen terug naar de originele
                objectRenderer.materials = originalMaterials;
                isPlayerNearby = false;

                // Zet de interactie uit als de speler te ver weg is
                grabInteractable.enabled = false; // Zet interactie uit
            }
        }
    }

    // Deze methode kan je aanroepen wanneer je het object oppakt
    public void OnPickup()
    {
        isHeld = true;
        objectRenderer.materials = originalMaterials; // Zorg ervoor dat de materialen resetten bij het oppakken
    }

    // Deze methode kan je aanroepen wanneer je het object loslaat
    public void OnDrop()
    {
        isHeld = false;

        // Controleer opnieuw of het object dichtbij is om de highlight opnieuw in te schakelen
        GameObject leftHand = GameObject.Find(leftHandName);
        GameObject rightHand = GameObject.Find(rightHandName);

        // Bereken de afstand tot de handen
        float distanceToLeftHand = Vector3.Distance(leftHand.transform.position, transform.position);
        float distanceToRightHand = Vector3.Distance(rightHand.transform.position, transform.position);

        // Als het object nog dichtbij is, zet de highlight aan
        if (distanceToLeftHand < pickupDistance || distanceToRightHand < pickupDistance)
        {
            Material[] highlightMaterials = new Material[originalMaterials.Length];
            for (int i = 0; i < highlightMaterials.Length; i++)
            {
                highlightMaterials[i] = highlightMaterial;
            }
            objectRenderer.materials = highlightMaterials;
            isPlayerNearby = true;
        }
    }
}
