using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IItem 
{
   void Excute();
   Sprite GetSprite();
   void Disable();
}
