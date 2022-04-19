using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ClassSystem;

namespace DuloGames.UI
{/// <summary>
/// Singleton to handle Character selection. 
/// </summary>
    public class CharacterSelect : Singleton<CharacterSelect>
    {
        public List<CharacterClass> classes = new List<CharacterClass>();//Store references to the class SO's
        public List<int> playerClassIndices = new List<int>(); //Store the position in the 'classes' list for each player. ie Player 1 has index 0 == player 1 is rogue, player 2 has index 3 == player 2 is enchanter

        protected override void Awake()
        {
            base.Awake();
            UIToggleActiveTransition.OnIconSelected += UIToggleActiveTransition_OnIconSelected;
        }
        /// <summary>
        /// Called from the UIToggleActiveTransition component of the UI icons. Event contains the index of the icon which invoked the event ie(icon 2 was clicked, so we should set player 1 to bulwark)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void UIToggleActiveTransition_OnIconSelected(object sender, UIToggleActiveTransitionEventArgs e)
        {
            int index = e.index;//Each icon on the panel has an icon index value. The first 4 are for the rogue, next set is for the bulwark etc.
            if(index < 4)//player 1
            {
                playerClassIndices[0] = index;
            }
            else if(index > 3 && index < 8)//Player 2
            {
                index -= 4; //Set the index back in the range of 0-3
                playerClassIndices[1] = index;
            }
            else if (index > 7 && index < 12)//Player 3
            {
                index -= 8; //Set the index back in the range of 0-3
                playerClassIndices[2] = index;
            }
            else if (index > 11 && index < 16)//Player 4
            {
                index -= 12; //Set the index back in the range of 0-3
                playerClassIndices[3] = index;
            }
        }
        /// <summary>
        /// Return the class stored for one of the players
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public CharacterClass GetCharClassAtIndex(int index)
        {
            return classes[playerClassIndices[index]];
        }

    }
}

