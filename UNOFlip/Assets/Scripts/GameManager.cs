using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public List<Player> players = new List<Player>();
    [SerializeField] Deck deck;

    [SerializeField] Transform playerHandTransform; //HOLDS PLAYER HAND
    [SerializeField] List<Transform> aiHandTransform = new List<Transform>(); //HOLDS AI HANDS
    [SerializeField] GameObject cardPrefab;
    [SerializeField] int numberOfAiPlayers = 3;
    [SerializeField] int startingHandSize = 7;
    int currentPlayer = 0;



    void Awake()
    {
        instance = this;

    }

    void Start()
    {
        //INITIALIZE DECK
        deck.InitializeDeck();

        //INITIALIZE PLAYERS
        InitializePlayers();

        //DEAL CARDS
        StartCoroutine(DealStartingCards());

        //START GAME
    }

    void InitializePlayers()
    {
        players.Clear();
        players.Add(new Player("Player",true));

        for (int i = 0; i < numberOfAiPlayers; i++)
        {
            players.Add(new AiPlayer("AI" + (i+1)));
        }
    }

    void DealCards()
    {
        for (int i = 0; i < startingHandSize; i++)
        {
            foreach (Player player in players)
            {
                player.DrawCard(deck.DrawCard());
            }
        }

        //DISPLAY CARDS
        StartCoroutine(DealStartingCards());

        //DISPLAY AI HAND  
    }

    IEnumerator DealStartingCards()
    {
        for (int i = 0; i < startingHandSize; i++)
        {
            foreach (Player player in players)
            {
                Card drawnCard = deck.DrawCard();
                player.DrawCard(drawnCard);

                //VISUALISE CARDS
                Transform hand = player.IsHuman ? playerHandTransform : aiHandTransform[players.IndexOf(player)-1];
                GameObject card = Instantiate(cardPrefab, hand, false);

                //DRAW CARDS
                card.GetComponent<CardDisplay>().SetCard(drawnCard); //ONLY FOR HUMAN

                //FOR AI PLAYERS
                if (!player.IsHuman)
                {
                    //SHOW BACK OF CARD
                }

                yield return new WaitForSeconds(0.1f);
            }
        }
        //START GAME
        Debug.Log("Game Started");
    }
}