using System.Collections;
using System.Collections.Generic;
using M4u;
using UnityEngine;

public class HeroTestContext : M4uContextMonoBehaviour {
    
    
    public M4uProperty<Sprite> hero = new M4uProperty<Sprite> ();

    public Sprite Hero {
        get {
            return hero.Value;
        }
        set {
            hero.Value = value;
        }
    }

    
   
    public M4uProperty<Texture> hero_mask = new M4uProperty<Texture> ();

    public Texture Hero_mask {
        get {
            return hero_mask.Value;
        }
        set {
            hero_mask.Value = value;
        }
    }

   

}
