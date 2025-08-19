using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Linq;

public class GerenciarMontagem : MonoBehaviour
{
    public TMP_Text textoTutorial;
    public TMP_Text textoNumero;
    public TMP_Text textoProximo;
    public TMP_Text tutorialInicial;
    public TMP_Text sair;

    

    [Header("Progresso Circular")]
    public Image preenchimentoProgresso;
    public TMP_Text textoNumeroProgresso;
    public TMP_Text textoTotalPassos;

    [Header("Referências")]
    public ControladorPopupFinal popupFinal;

    private string[] passosTutoriais;
    private string[] animacoes;
    private int passoAtual = -1;
    private bool passosIniciados = false;
    private PlaceOnPlane placeOnPlane;

    void Start()
    {
        CarregarPassosDoProblemaOuMontagem();

        if (passosTutoriais == null || passosTutoriais.Length == 0)
        {
            Debug.LogError("Passos não carregados corretamente.");
            return;
        }

        placeOnPlane = FindObjectOfType<PlaceOnPlane>();

        if (textoTotalPassos != null)
        {
            string deTraduzido = CarregarBancoDeDadosMontagem.Dados?.textoDe ?? "de";
            textoTotalPassos.text = $"{deTraduzido} {passosTutoriais.Length}";
        }

        if (preenchimentoProgresso != null)
            preenchimentoProgresso.fillAmount = 0f;

        var botaoSair = GameObject.Find("Sair")?.GetComponentInChildren<TMP_Text>();
        if (botaoSair != null && CarregarBancoDeDadosMontagem.Dados != null)
            botaoSair.text = CarregarBancoDeDadosMontagem.Dados.sair;
    }

    private void CarregarPassosDoProblemaOuMontagem()
    {
        string origem = ControleDeCena.Instance?.origemDaCena ?? "montagem";

        if (origem == "montagem")
        {
            var dados = CarregarBancoDeDadosMontagem.Dados;
            if (dados != null && dados.passos != null && dados.passos.Length > 0)
            {
                passosTutoriais = dados.passos.Select(p => p.tutorial).ToArray();
                animacoes = dados.passos.Select(p => $"animacao_{p.numero}").ToArray();
                Debug.Log("✅ Passos carregados do banco padrão de montagem.");
            }
        }
        else
        {
            string id = ProblemaSelecionadoAR.Instance?.idProblema;
            if (!string.IsNullOrEmpty(id))
            {
                TextAsset json = Resources.Load<TextAsset>($"BancoDeDadosProblemas/{id}");
                if (json != null)
                {
                    var dados = JsonUtility.FromJson<PassoAPasso>(json.text);
                    if (dados != null && dados.etapas != null && dados.etapas.Length > 0)
                    {
                        passosTutoriais = dados.etapas.Select(e => e.tutorial).ToArray();
                        animacoes = dados.etapas.Select(e => e.animacao).ToArray();
                        ProblemaSelecionadoAR.Instance.passoAPasso = dados;
                        Debug.Log($"✅ PassoAPasso carregado para problema '{id}' com layer '{dados.layer}'");
                    }
                }
                else
                {
                    Debug.LogError($"❌ JSON para problema {id} não encontrado!");
                }
            }
        }
    }

    public void IniciarPassos()
    {
        passoAtual = 0;
        passosIniciados = true;
        AtualizarPasso();
    }

    public void AvancarPasso()
    {
        if (!passosIniciados) return;
        if (passoAtual >= passosTutoriais.Length - 1)
        {
            popupFinal?.MostrarPopupFinal();
            return;
        }
        passoAtual++;
        AtualizarPasso();
    }

    public void VoltarPasso()
    {
        if (!passosIniciados || passoAtual <= 0) return;
        passoAtual--;
        AtualizarPasso();
    }

    public void RepetirPasso()
    {
        if (!passosIniciados) return;
        AtualizarPasso();

        if (placeOnPlane != null && passoAtual < animacoes.Length)
        {
            placeOnPlane.PlayAnimation(animacoes[passoAtual]);
        }
    }

    private void AtualizarPasso()
    {
        if (passoAtual < 0 || passoAtual >= passosTutoriais.Length) return;

        textoTutorial.text = passosTutoriais[passoAtual];
        textoNumero.text = (passoAtual + 1).ToString();
        textoNumeroProgresso.text = (passoAtual + 1).ToString();
        textoTotalPassos.text = $"{CarregarBancoDeDadosMontagem.Dados?.textoDe ?? "de"} {passosTutoriais.Length}";

        preenchimentoProgresso.fillAmount = (float)(passoAtual + 1) / passosTutoriais.Length;

        bool ultimo = passoAtual == passosTutoriais.Length - 1;
        textoProximo.text = ultimo
            ? CarregarBancoDeDadosMontagem.Dados?.finalizar ?? "Finalizar Montagem"
            : CarregarBancoDeDadosMontagem.Dados?.proximo ?? "Próximo Passo";
        textoProximo.color = ultimo ? new Color32(245, 71, 3, 255) : Color.white;
        Button botao = textoProximo.GetComponentInParent<Button>();
        if (botao != null)
            botao.image.color = ultimo ? Color.white : new Color32(245, 71, 3, 255);

        if (placeOnPlane != null && passoAtual < animacoes.Length)
        {
            placeOnPlane.PlayAnimation(animacoes[passoAtual]);
        }
    }

    public void ReiniciarMontagem()
    {
        passoAtual = 0;
        passosIniciados = true;
        AtualizarPasso();

        if (placeOnPlane != null && animacoes.Length > 0)
        {
            placeOnPlane.PlayAnimation(animacoes[0]);
        }
    }

    public bool IsRunning() => passosIniciados;
}
