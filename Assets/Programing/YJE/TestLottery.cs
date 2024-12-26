using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class TestLottery : MonoBehaviour
{
    List<Lottery> lotteries = new List<Lottery>();
    private int total = 0;
    private void Awake()
    {
        Lottery gold = new Lottery();
        gold.Id = 500;
        gold.Probability = 3000;
        lotteries.Add(gold);

        Lottery binoBlood = new Lottery();
        binoBlood.Id = 501;
        binoBlood.Probability = 3500;
        lotteries.Add(binoBlood); 

        Lottery boneCrystal = new Lottery();
        boneCrystal.Id = 502;
        boneCrystal.Probability = 2500;
        lotteries.Add(boneCrystal); 
        
        Lottery dinoStone = new Lottery();
        dinoStone.Id = 503;
        dinoStone.Probability = 500;
        lotteries.Add(dinoStone); 
        
        Lottery stone = new Lottery();
        stone.Id = 504;
        stone.Probability = 500;
        lotteries.Add(stone);

        for(int i = 0; i < lotteries.Count; i++)
        {
            total += lotteries[i].Probability;
            Debug.Log(lotteries[i].Id);
            Debug.Log(lotteries[i].Probability);
        }
        Debug.Log(total);
    }

    public void SingleLotteryBtn()
    {

    }
}
