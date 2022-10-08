using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : MonoBehaviour
{
    public struct CharacterAttributes
    {
        public int LEVEL;
        public int EXPERIENCE;      // 0 to 1 level exp requirement is 500exp, by each level, it increases by
                                    // baseExp(500) + ((lastExpRequirement/39.58708f)*30) + (baseExp(500) * (i/1))

        public float DEFAULT_SPEED;
        public float RUNNING_MULTIPLIER;
        public float JUMPING_STRENGTH;

        public int DEFENSE;         // Each defense point increases damage reduction against all enemies by 0.30% (3x against lower level enemies) (90% Maximum)
        public int AGILITY;         // Each agility point increases movement speed, jumping strength (by 0.65%) and decreases dodge cooldown (by 1.35%).
        public int INTELLIGENCE;    // Each intelligence point increases your mana by 1 point, increases mana regeneration speed by 1.60% and increases magic damage by 0.50%.


        //Buffs

        public float EXP_BOOST; // MAX 65%
        
    }
    public CharacterAttributes attributes;

    public Rigidbody2D RB2D;

    void Start()
    {
        //Set attributes
        attributes.LEVEL = 0;
        attributes.EXPERIENCE = 0;
        attributes.DEFAULT_SPEED = 1.05f;
        attributes.RUNNING_MULTIPLIER = 2.54f;
        attributes.JUMPING_STRENGTH = 5f;
        attributes.DEFENSE = 0;
        attributes.INTELLIGENCE = 0;
        attributes.AGILITY = 0;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
