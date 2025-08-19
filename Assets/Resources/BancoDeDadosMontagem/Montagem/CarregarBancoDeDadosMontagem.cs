using UnityEngine;

public class CarregarBancoDeDadosMontagem : MonoBehaviour
{
    public static DadosMontagem Dados { get; private set; }

    public static void Carregar(string idioma)
    {
        TextAsset arquivo = Resources.Load<TextAsset>($"BancoDeDadosMontagem/Montagem/banco_montagem_{idioma}");

        if (arquivo != null)
        {
            Dados = JsonUtility.FromJson<DadosMontagem>(arquivo.text);
        }
        else
        {
            Debug.LogError($"Arquivo banco_montagem_{idioma}.json não encontrado!");
            Dados = new DadosMontagem(); // evita null
        }
    }

    [System.Serializable]
    public class DadosMontagem
    {
        public PassoMontagem[] passos;
        public string proximo;
        public string finalizar;
        public string popupFinalTitulo;
        public string popupFinalBotaoMenu;
        public string popupFinalBotaoRecomecar;
        public string popupInicialTexto;
        public string popupInicialTexto1;
        public string popupInicialTexto2;
        public string popupInicialBotao;
        public string sair;
        public string textoDe;
        public string tutorialInicial;  // ✅ Adicione isso aqui
    }


    [System.Serializable]
    public class PassoMontagem
    {
        public string tutorial;
        public string numero;
    }
}
