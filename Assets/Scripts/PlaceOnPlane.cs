using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;
using System.Collections.Generic;

public class PlaceOnPlane : MonoBehaviour
{
    [Header("AR Placement")]
    [SerializeField] private GameObject placedPrefab;
    [SerializeField] private Camera arCamera;

    [Header("UI Elements")]
    [SerializeField] private GameObject uiCanvas;
    [SerializeField] private GameObject grupoMontagem;
    [SerializeField] private GerenciarMontagem gerenciarMontagem;
    [SerializeField] private RectTransform painelTutorial;

    private ARRaycastManager raycastManager;
    private GameObject spawnedObject;
    private static List<ARRaycastHit> hits = new List<ARRaycastHit>();
    private Animator animator;
    private float painelTutorialY;

    void Awake()
    {
        raycastManager = GetComponent<ARRaycastManager>();

        if (arCamera == null)
            arCamera = Camera.main;

        if (uiCanvas != null)
            uiCanvas.SetActive(true);

        if (grupoMontagem != null)
            grupoMontagem.SetActive(false);

        if (painelTutorial != null)
        {
            painelTutorialY = painelTutorial.anchoredPosition.y;
            painelTutorial.anchoredPosition = new Vector2(0f, painelTutorialY); // Centralizado no início
        }
    }

    void Update()
    {
        Vector2 screenPosition;

#if UNITY_EDITOR
        if (!Mouse.current.leftButton.wasPressedThisFrame) return;
        if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject()) return;
        screenPosition = Mouse.current.position.ReadValue();
#else
        if (Touchscreen.current == null || Touchscreen.current.touches.Count == 0) return;
        var touch = Touchscreen.current.touches[0];
        if (!touch.press.wasPressedThisFrame) return;
        if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject(touch.touchId.ReadValue())) return;
        screenPosition = touch.position.ReadValue();
#endif

        if (raycastManager.Raycast(screenPosition, hits, TrackableType.PlaneWithinPolygon))
        {
            Pose hitPose = hits[0].pose;

            if (spawnedObject == null)
            {
                spawnedObject = Instantiate(placedPrefab, hitPose.position, hitPose.rotation);
                
                animator = spawnedObject.GetComponentInChildren<Animator>();
                if (animator == null)
                {
                    Debug.LogError("❌ CRÍTICO: Animator não encontrado no prefab instanciado!");
                    return; 
                }

                animator.Rebind();
                animator.Update(0f); 

                Debug.Log("✅ Prefab AR colocado e Animator reinicializado.");

                if (grupoMontagem != null)
                    grupoMontagem.SetActive(true);

                if (gerenciarMontagem != null && !gerenciarMontagem.IsRunning())
                    gerenciarMontagem.IniciarPassos(); // Inicia os passos diretamente aqui

                if (painelTutorial != null)
                {
                    painelTutorial.anchoredPosition = new Vector2(107.3553f, painelTutorialY); // Mover para lateral após AR colocado
                    Debug.Log($"📍 Painel Tutorial movido lateralmente para: {painelTutorial.anchoredPosition}");
                }

            }
            else
            {
                spawnedObject.transform.SetPositionAndRotation(hitPose.position, hitPose.rotation);
            }
        }
    }

    public void PlayAnimation(string animName)
    {
        if (animator == null)
        {
            Debug.LogWarning("⚠️ Animator não está disponível para tocar a animação.");
            return;
        }

        string origem = ControleDeCena.Instance?.origemDaCena ?? "montagem";
        string camadaAlvo = "Base Layer"; 

        if (origem != "montagem")
        {
            camadaAlvo = ProblemaSelecionadoAR.Instance?.passoAPasso?.layer ?? "Base Layer";
            if (camadaAlvo == "Base Layer")
            {
                Debug.LogWarning("⚠️ Origem é 'problema', mas o layer específico não foi encontrado. Usando 'Base Layer'.");
            }
        }

        int layerIndex = animator.GetLayerIndex(camadaAlvo);

        if (layerIndex == -1)
        {
            Debug.LogError($"❌ Layer '{camadaAlvo}' não existe no Animator Controller. Verifique o nome.");
            return;
        }

        for (int i = 0; i < animator.layerCount; i++)
        {
            animator.SetLayerWeight(i, (i == layerIndex) ? 1f : 0f);
        }

        Debug.Log($"▶️ Tentando tocar a animação '{animName}' no layer '{camadaAlvo}' (índice: {layerIndex}).");

        animator.Play(animName, layerIndex, 0f);

        animator.Update(Time.deltaTime);
    }

    public void NextAnimation() { }
    public void PrevAnimation() { }
}


