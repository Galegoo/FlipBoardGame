using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;



[System.Serializable]
public class Save
{
    public string _namee;
    public int _triess;
    public float _timee;
    public int _acertoss;

    public int[] posicoes; //cartas
    public int[] pais; //cards

    public List<int> ViradasCartas;
    public List<int> AcertadasCartas;


}

[System.Serializable]
public class jogador
{
    public string _name;
    public int _tries;
    public float _time;
    public int _score;
}

public class Gerenciador : MonoBehaviour
{
    public GameObject[] cartas;//posicoes
    public GameObject[] _FaceCarta;//face das cartas
    public GameObject[] cards; //array com todos os game objects Pais das cartas

    public Carta PrimeiraCarta; // primeira carta selecionada na jogada
    public Carta SegundaCarta;  // segunda carta selecionada na jogada
    public Carta TerceiraCarta; // terceira carta selecionada na jogada
    public List<GameObject> cartasViradas;//Cartas viradads nesta jogada
    public List<GameObject> cartasAcertadas;//Cartas Acertadas das jogadas anteriores

    public bool _naovira = false;//bool para não permitir eterna validação com varios cliques 

    public int acertos;
    public int jogocomeçou = 0;

    public int _tentativas;
    public float _timeElapsed;


    public Text _tentativasTexto;
    public Text _TimeElapsedTexto;

    public Text _ScoreText;
    public GameObject WinPopUp;// Referencia do Game Object do Menu de Vitoria
    public GameObject TelaInicial;
    public int score;

    public GameObject _leaderBoard; // Referencia do Game Object Leaderboard
    public Text[] _leaderBoardTexts; // array com os textos do ranking

    public jogador[] positionRank;// array de informações do jogador no ranking
    public jogador _jogadorAtual; // variavel que possui as informações do jogador atual

    int jaentrou;//validador para o ranking

    public string playername;
    public InputField nameInput;
    public Text nameText;

    void Start()
    {
        jaentrou = 0;
        Time.timeScale = 1;
        acertos = 0;
        cartas = GameObject.FindGameObjectsWithTag("Carta");
        cards = GameObject.FindGameObjectsWithTag("Cards");
        MisturaArray();
        SetRank();
    }
    

    private void Update()
    {
        _TimeElapsedTexto.text = "Time Elapsed: " + Mathf.Round(_timeElapsed);
        _tentativasTexto.text = "Tries: " + _tentativas;

        if (Input.GetMouseButtonDown(0))
        {
            if (!_naovira)
                Validacao();
        }

        if (acertos < 8)
        {
            if(jogocomeçou > 0)
                _timeElapsed += Time.deltaTime;
        }
        else if (acertos >= 8)
        {
            StartCoroutine("EsperaParaGanhar");
        }
    }

    void MisturaArray() // mistura o array de posições
    {
        for (int i = 0; i < cartas.Length; i++)
        {
            int rnd = UnityEngine.Random.Range(0, cartas.Length);
            GameObject tempGO = cartas[rnd];
            cartas[rnd] = cartas[i];
            cartas[i] = tempGO;
        }

        for (int index = 0; index < _FaceCarta.Length; index++) // muda o Sprite dos objetos no array de Faces das Cartas
        {
            cartas[index].transform.parent.GetComponent<Carta>().tipoo = _FaceCarta[index].GetComponent<FrenteCarta>()._tipo;
            cartas[index].GetComponent<SpriteRenderer>().sprite = _FaceCarta[index].GetComponent<SpriteRenderer>().sprite;
        }
    }

    public void Validacao()// Adiciona as cartas viradas a lista de cartas viradas e verifica se 3 cartas ja foram viradas
    {
        cartasViradas.Clear();
        for (int i = 0; i < cartas.Length; i++)
        {

            if (cartas[i].transform.parent.GetComponent<Carta>()._estado == 1)
            {
                cartasViradas.Add(cartas[i]);
            }
        }
        if (cartasViradas.Count >= 3)
        {
            _tentativas++;
            checarCartas();
        }
    }

    public void checarCartas() // verifica se as 3 cartas viradas são do mesmo tipo
    {
        PrimeiraCarta = cartasViradas[0].transform.parent.GetComponent<Carta>();
        SegundaCarta = cartasViradas[1].transform.parent.GetComponent<Carta>();
        TerceiraCarta = cartasViradas[2].transform.parent.GetComponent<Carta>();

        if (PrimeiraCarta.tipoo == SegundaCarta.tipoo && PrimeiraCarta.tipoo == TerceiraCarta.tipoo)
        {
            acertos++;
            cartasAcertadas.AddRange(cartasViradas);

            PrimeiraCarta._estado = 2;
            SegundaCarta._estado = 2;
            TerceiraCarta._estado = 2;

            cartasViradas.Clear();
        }
        else
        {
            StartCoroutine("EsperaParaVirar");
        }
    }

    IEnumerator EsperaParaVirar() // Ienumerator para ter um delay na virada das cartas
    {
        _naovira = true;
        Color normal = new Color(1f, 1f, 1f, 1f);

        yield return new WaitForSeconds(0.5f);
        PrimeiraCarta._Costas.color = normal;
        PrimeiraCarta._estado = 0;

        SegundaCarta._Costas.color = normal;
        SegundaCarta._estado = 0;

        TerceiraCarta._Costas.color = normal;
        TerceiraCarta._estado = 0;

        _naovira = false;
        cartasViradas.Clear();
    }


    IEnumerator EsperaParaGanhar()  // Ienumerator para ter um delay na vitoria
    {
        yield return new WaitForSeconds(1);

        for (int i = 0; i < cards.Length; i++)
        {
            cards[i].SetActive(false);
        }
        score = (_tentativas * 5) + (int)_timeElapsed;     //(Number of Moves x 5) + Total elapsed time in seconds.
        WinPopUp.SetActive(true);
        _ScoreText.text = "Your Score was: " + score;
        if (jaentrou <= 0)
        {
            jaentrou = 0;
            VerifRank();
        }
    }

    public void JogarNovamente() // Carrega a cena principal
    {
        SceneManager.LoadScene("Flip Card Game");
    }

    public void Jogar() 
    {
        if (nameInput.text != "")
        {
            playername = nameInput.text;
            nameText.text = playername;
            TelaInicial.SetActive(false);
            jogocomeçou++;
            LoadGame();
        }
        else
            Debug.Log("Insira um nome");
    }

    public void LeaderBoard() // Metodo que mostra o ranking, desativa todas as cartas e para o tempo
    {
        for (int i = 0; i < cards.Length; i++)
        {
            cards[i].SetActive(false);
        }
        _leaderBoard.SetActive(true);
        DistribuicaoRanking();
        Time.timeScale = 0;
    }

    public void FlipBoardGame() // Metodo que mostra o Jogo, reativa todas as cartas, desativa o ranking e volta o tempo ao normal
    {
        for (int i = 0; i < cards.Length; i++)
        {
            cards[i].SetActive(true);
        }
        _leaderBoard.SetActive(false);
        Time.timeScale = 1;
    }

    void VerifRank() // Verifica se o jogador fez uma pontuação boa o suficiente para entrar no ranking e se sim, em que posição
    {

        _jogadorAtual._time = _timeElapsed;
        _jogadorAtual._tries = _tentativas;
        _jogadorAtual._score = score;
        _jogadorAtual._name = playername;

        if (_jogadorAtual._score <= positionRank[0]._score && jaentrou <= 0)
        {
            positionRank[5] = positionRank[4];
            positionRank[4] = positionRank[3];
            positionRank[3] = positionRank[2];
            positionRank[2] = positionRank[1];
            positionRank[1] = positionRank[0];
            positionRank[0] = _jogadorAtual;
            jaentrou++;
            Debug.Log("ENTROU POS 1");
        }
        else if (_jogadorAtual._score <= positionRank[1]._score && jaentrou <= 0)
        {
            positionRank[5] = positionRank[4];
            positionRank[4] = positionRank[3];
            positionRank[3] = positionRank[2];
            positionRank[2] = positionRank[1];
            positionRank[1] = _jogadorAtual;
            jaentrou++;
            Debug.Log("ENTROU POS 2");
        }
        else if (_jogadorAtual._score <= positionRank[2]._score && jaentrou <= 0)
        {
            positionRank[5] = positionRank[4];
            positionRank[4] = positionRank[3];
            positionRank[3] = positionRank[2];
            positionRank[2] = _jogadorAtual;
            jaentrou++;
            Debug.Log("ENTROU POS 3");
        }
        else if (_jogadorAtual._score <= positionRank[3]._score && jaentrou <= 0)
        {
            positionRank[5] = positionRank[4];
            positionRank[4] = positionRank[3];
            positionRank[3] = _jogadorAtual;
            jaentrou++;
            Debug.Log("ENTROU POS 4");
        }
        else if (_jogadorAtual._score <= positionRank[4]._score && jaentrou <= 0)
        {
            positionRank[5] = positionRank[4];
            positionRank[4] = _jogadorAtual;
            jaentrou++;
            Debug.Log("ENTROU POS 5");
        }
        else if (_jogadorAtual._score <= positionRank[5]._score && jaentrou <= 0)
        {
            positionRank[5] = _jogadorAtual;
            jaentrou++;
            Debug.Log("ENTROU POS 6");
        }
        PlayerPrefs.SetInt("TriesPos1", positionRank[0]._tries); //Salvando as informações do ranking
        PlayerPrefs.SetFloat("TimePos1", positionRank[0]._time);
        PlayerPrefs.SetInt("ScorePos1", positionRank[0]._score);
        PlayerPrefs.SetString("NamePos1", positionRank[0]._name);

        PlayerPrefs.SetInt("TriesPos2", positionRank[1]._tries);
        PlayerPrefs.SetFloat("TimePos2", positionRank[1]._time);
        PlayerPrefs.SetInt("ScorePos2", positionRank[1]._score);
        PlayerPrefs.SetString("NamePos2", positionRank[1]._name);

        PlayerPrefs.SetInt("TriesPos3", positionRank[2]._tries);
        PlayerPrefs.SetFloat("TimePos3", positionRank[2]._time);
        PlayerPrefs.SetInt("ScorePos3", positionRank[2]._score);
        PlayerPrefs.SetString("NamePos3", positionRank[2]._name);

        PlayerPrefs.SetInt("TriesPos4", positionRank[3]._tries);
        PlayerPrefs.SetFloat("TimePos4", positionRank[3]._time);
        PlayerPrefs.SetInt("ScorePos4", positionRank[3]._score);
        PlayerPrefs.SetString("NamePos4", positionRank[3]._name);

        PlayerPrefs.SetInt("TriesPos5", positionRank[4]._tries);
        PlayerPrefs.SetFloat("TimePos5", positionRank[4]._time);
        PlayerPrefs.SetInt("ScorePos5", positionRank[4]._score);
        PlayerPrefs.SetString("NamePos5", positionRank[4]._name);

        PlayerPrefs.SetInt("TriesPos6", positionRank[5]._tries);
        PlayerPrefs.SetFloat("TimePos6", positionRank[5]._time);
        PlayerPrefs.SetInt("ScorePos6", positionRank[5]._score);
        PlayerPrefs.SetString("NamePos6", positionRank[5]._name);

        SetRank();
    }
    void DistribuicaoRanking() // Relaciona os Textos do Ranking com as Variaveis do script
    {
        _leaderBoardTexts[0].text = "" + positionRank[0]._tries;
        _leaderBoardTexts[1].text = "" + positionRank[0]._time;
        _leaderBoardTexts[2].text = "" + positionRank[0]._score;

        _leaderBoardTexts[3].text = "" + positionRank[1]._tries;
        _leaderBoardTexts[4].text = "" + positionRank[1]._time;
        _leaderBoardTexts[5].text = "" + positionRank[1]._score;

        _leaderBoardTexts[6].text = "" + positionRank[2]._tries;
        _leaderBoardTexts[7].text = "" + positionRank[2]._time;
        _leaderBoardTexts[8].text = "" + positionRank[2]._score;

        _leaderBoardTexts[9].text = "" + positionRank[3]._tries;
        _leaderBoardTexts[10].text = "" + positionRank[3]._time;
        _leaderBoardTexts[11].text = "" + positionRank[3]._score;

        _leaderBoardTexts[12].text = "" + positionRank[4]._tries;
        _leaderBoardTexts[13].text = "" + positionRank[4]._time;
        _leaderBoardTexts[14].text = "" + positionRank[4]._score;

        _leaderBoardTexts[15].text = "" + positionRank[5]._tries;
        _leaderBoardTexts[16].text = "" + positionRank[5]._time;
        _leaderBoardTexts[17].text = "" + positionRank[5]._score;

        _leaderBoardTexts[18].text = "" + positionRank[0]._name;
        _leaderBoardTexts[19].text = "" + positionRank[1]._name;
        _leaderBoardTexts[20].text = "" + positionRank[2]._name;
        _leaderBoardTexts[21].text = "" + positionRank[3]._name;
        _leaderBoardTexts[22].text = "" + positionRank[4]._name;
        _leaderBoardTexts[23].text = "" + positionRank[5]._name;

    }

    public void SetRank() // Busca as informações do ranking
    {
        if (PlayerPrefs.GetInt("TriesPos1") != 0)
            positionRank[0]._tries = PlayerPrefs.GetInt("TriesPos1");
        if (PlayerPrefs.GetFloat("TimePos1") != 0)
            positionRank[0]._time = PlayerPrefs.GetFloat("TimePos1");
        if (PlayerPrefs.GetInt("ScorePos1") != 0)
            positionRank[0]._score = PlayerPrefs.GetInt("ScorePos1");
        if (PlayerPrefs.HasKey("NamePos1"))
            positionRank[0]._name = PlayerPrefs.GetString("NamePos1");

        if (PlayerPrefs.GetInt("TriesPos2") != 0)
            positionRank[1]._tries = PlayerPrefs.GetInt("TriesPos2");
        if (PlayerPrefs.GetFloat("TimePos2") != 0)
            positionRank[1]._time = PlayerPrefs.GetFloat("TimePos2");
        if (PlayerPrefs.GetInt("ScorePos2") != 0)
            positionRank[1]._score = PlayerPrefs.GetInt("ScorePos2");
        if (PlayerPrefs.HasKey("NamePos2"))
            positionRank[1]._name = PlayerPrefs.GetString("NamePos2");

        if (PlayerPrefs.GetInt("TriesPos3") != 0)
            positionRank[2]._tries = PlayerPrefs.GetInt("TriesPos3");
        if (PlayerPrefs.GetFloat("TimePos3") != 0)
            positionRank[2]._time = PlayerPrefs.GetFloat("TimePos3");
        if (PlayerPrefs.GetInt("ScorePos3") != 0)
            positionRank[2]._score = PlayerPrefs.GetInt("ScorePos3");
        if (PlayerPrefs.HasKey("NamePos3"))
            positionRank[2]._name = PlayerPrefs.GetString("NamePos3");

        if (PlayerPrefs.GetInt("TriesPos4") != 0)
            positionRank[3]._tries = PlayerPrefs.GetInt("TriesPos4");
        if (PlayerPrefs.GetFloat("TimePos4") != 0)
            positionRank[3]._time = PlayerPrefs.GetFloat("TimePos4");
        if (PlayerPrefs.GetInt("ScorePos4") != 0)
            positionRank[3]._score = PlayerPrefs.GetInt("ScorePos4");
        if (PlayerPrefs.HasKey("NamePos4"))
            positionRank[3]._name = PlayerPrefs.GetString("NamePos4");

        if (PlayerPrefs.GetInt("TriesPos5") != 0)
            positionRank[4]._tries = PlayerPrefs.GetInt("TriesPos5");
        if (PlayerPrefs.GetFloat("TimePos5") != 0)
            positionRank[4]._time = PlayerPrefs.GetFloat("TimePos5");
        if (PlayerPrefs.GetInt("ScorePos5") != 0)
            positionRank[4]._score = PlayerPrefs.GetInt("ScorePos5");
        if (PlayerPrefs.HasKey("NamePos5"))
            positionRank[4]._name = PlayerPrefs.GetString("NamePos5");

        if (PlayerPrefs.GetInt("TriesPos6") != 0)
            positionRank[5]._tries = PlayerPrefs.GetInt("TriesPos6");
        if (PlayerPrefs.GetFloat("TimePos6") != 0)
            positionRank[5]._time = PlayerPrefs.GetFloat("TimePos6");
        if (PlayerPrefs.GetInt("ScorePos6") != 0)
            positionRank[5]._score = PlayerPrefs.GetInt("ScorePos6");
        if (PlayerPrefs.HasKey("NamePos6"))
            positionRank[5]._name = PlayerPrefs.GetString("NamePos6");
    }

    private Save CriaSave()
    {

        Save save = new Save();

        save._namee = playername;
        save._triess = _tentativas;
        save._timee = _timeElapsed;
        save._acertoss = acertos;

        save.posicoes = new int [cartas.Length];
        save.ViradasCartas = new List<int>();
        save.AcertadasCartas = new List<int>();


        for (int index = 0; index < cartas.Length; index++)
        {
            save.posicoes[index] = cartas[index].transform.parent.GetComponent<Carta>().tipoo; //diz onde estava cada carta
        }

        if (cartasViradas.Count > 0)
        {
            for (int index = 0; index < cartas.Length; index++)
            {
                if (cartas[index].transform.parent.GetComponent<Carta>()._estado == 1)
                    save.ViradasCartas.Add(index);                //recebe como int a posição das cartas viradas
            }
        }

        if (cartasAcertadas.Count > 0)
        {
            for (int index = 0; index < cartas.Length; index++)
            {
                if (cartas[index].transform.parent.GetComponent<Carta>()._estado == 2)
                    save.AcertadasCartas.Add(index);                //recebe como int a posição das cartas acertadas
            }
        }
        return save;
    }

    public void SaveGame()
    {

        Save save = CriaSave();

        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Create(Application.persistentDataPath + playername +".save");
        bf.Serialize(file, save);
        file.Close();
    }

    public void LoadGame()
    {
        if (File.Exists(Application.persistentDataPath + playername + ".save"))
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(Application.persistentDataPath + playername + ".save", FileMode.Open);
            Save save = (Save)bf.Deserialize(file);
            file.Close();

            playername = save._namee;
            _tentativas = save._triess;
            _timeElapsed = save._timee;
            acertos = save._acertoss;

            cartasViradas.Clear();
            cartasAcertadas.Clear();

            for (int index = 0; index < cartas.Length; index++)
            {
                cartas[index].transform.parent.GetComponent<Carta>().tipoo = save.posicoes[index];
            }
            for (int index = 0; index < _FaceCarta.Length; index++) // muda o Sprite dos objetos 
            {
                if(cartas[index].transform.parent.GetComponent<Carta>().tipoo == _FaceCarta[index].GetComponent<FrenteCarta>()._tipo)
                    cartas[index].GetComponent<SpriteRenderer>().sprite = _FaceCarta[index].GetComponent<SpriteRenderer>().sprite;
            }

            if (save.ViradasCartas.Count > 0)
            {
                foreach(int i in save.ViradasCartas)
                {
                    cartas[i].transform.parent.GetComponent<Carta>()._estado = 1;
                }
            }

            if (save.AcertadasCartas.Count > 0)
            {
                foreach (int i in save.AcertadasCartas)
                {
                    cartas[i].transform.parent.GetComponent<Carta>()._estado = 2;
                }
            }
        }
    }
}

  
    

    


