using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Carta : MonoBehaviour
{
    public int _estado;
    public int tipoo;

    public SpriteRenderer _Frente;
    public SpriteRenderer _Costas;

    Gerenciador _Gerencia;

    // Start is called before the first frame update
    void Start()
    {
        _estado = 0;
        _Costas = GetComponent<SpriteRenderer>();
        _Frente = GetComponentInChildren<SpriteRenderer>();
        _Gerencia = GameObject.Find("Gerenciador").GetComponent<Gerenciador>();
    }

    // Update is called once per frame
    void Update()
    {
        if(_estado == 1 || _estado == 2)
        {
            _Costas.color = new Color(1f, 1f, 1f, 0f); //turn the alpha for the back card to 0
        }
        
    }

    public void OnMouseDown()
    {
        if (_estado == 0 && _Gerencia._naovira == false)
        {
            _estado = 1;
            _Costas.color = new Color(1f, 1f, 1f, 0f); //turn the alpha for the back card to 0
        }
    }
}
