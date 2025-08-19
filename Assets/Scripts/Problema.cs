[System.Serializable]
public class Problema
{
    public string titulo;
    public string descricao;
    public string solucao;
    public string imagem;

    public string id => imagem; // Atalho para manter consistência com outros scripts
}
