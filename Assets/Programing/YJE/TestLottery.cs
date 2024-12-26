using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestLottery : MonoBehaviour
{
    List<Lottery> lotteries = new List<Lottery>();

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
            Debug.Log(lotteries[i].Id);
            Debug.Log(lotteries[i].Probability);
        }  
    }

    public void SingleLotteryBtn()
    {
        
    }
}
