using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;

public class PainelDeslizante : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler
{
    private RectTransform rt;
    private float alturaFechado = 735f;
    private float alturaAberto;
    private bool aberto = false;

    public GameObject botaoFechar;
    public float duracaoAnimacao = 0.3f;

    private float deltaMinima = 50f; // Quanto precisa arrastar para subir/descer
    private float yInicialDrag;
    private bool arrastou = false;

    void Start()
    {
        rt = GetComponent<RectTransform>();
        alturaAberto = Screen.height;
        FecharInstantaneamente();
        if (botaoFechar != null)
            botaoFechar.SetActive(false);
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        yInicialDrag = eventData.position.y;
        arrastou = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        float deltaY = Mathf.Abs(eventData.position.y - yInicialDrag);
        if (deltaY > 5f) // Pequeno threshold para não considerar toques simples como arrasto
        {
            arrastou = true;
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (!arrastou)
        {
            Debug.Log("Clique simples detectado — não faz nada.");
            return;
        }

        float deltaY = eventData.position.y - yInicialDrag;

        if (deltaY > deltaMinima)
        {
            Abrir();
        }
        else if (deltaY < -deltaMinima)
        {
            Fechar();
        }
        else
        {
            // Se movimento for muito pequeno, volta ao estado anterior
            if (aberto)
                Abrir();
            else
                Fechar();
        }
    }

    public void Abrir()
    {
        StopAllCoroutines();
        StartCoroutine(AnimarAltura(alturaAberto));
        aberto = true;

        if (botaoFechar != null)
            botaoFechar.SetActive(true);
    }

    public void Fechar()
    {
        StopAllCoroutines();
        StartCoroutine(AnimarAltura(alturaFechado));
        aberto = false;

        if (botaoFechar != null)
            botaoFechar.SetActive(false);
    }

    public void FecharInstantaneamente()
    {
        StopAllCoroutines();
        rt.sizeDelta = new Vector2(rt.sizeDelta.x, alturaFechado);
        aberto = false;

        if (botaoFechar != null)
            botaoFechar.SetActive(false);
    }

    public void ResetarPainel()
    {
        FecharInstantaneamente();
    }

    public bool EstaAberto()
    {
        return aberto;
    }

    private IEnumerator AnimarAltura(float alturaFinal)
    {
        float tempo = 0f;
        float alturaInicial = rt.sizeDelta.y;

        while (tempo < duracaoAnimacao)
        {
            float altura = Mathf.Lerp(alturaInicial, alturaFinal, tempo / duracaoAnimacao);
            rt.sizeDelta = new Vector2(rt.sizeDelta.x, altura);
            tempo += Time.deltaTime;
            yield return null;
        }

        rt.sizeDelta = new Vector2(rt.sizeDelta.x, alturaFinal);
    }
}
