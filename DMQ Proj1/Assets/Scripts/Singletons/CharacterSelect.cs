using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ClassSystem;
using UnityEngine.UI;
namespace DuloGames.UI
{/// <summary>
/// Singleton to handle Character selection. 
/// </summary>
    public class CharacterSelect : Singleton<CharacterSelect>
    {
        public List<CharacterClass> classes = new List<CharacterClass>();//Store references to the class SO's
        protected override void Awake()
        {
            base.Awake();
            UIToggleActiveTransition.OnIconSelected += UIToggleActiveTransition_OnIconSelected;
            PlayerDataManager.OnPlayerActivated += PlayerDataManager_OnPlayerActivated;
        }

        private void PlayerDataManager_OnPlayerActivated(object sender, PlayerDataManager.PlayerDataSessionEventArgs e)
        {
            e.Data.Info._CurrentClassPreset = classes[0]; //Initialize class to rogue
        }

        /// <summary>
        /// Called from the UIToggleActiveTransition component of the UI icons. Event contains the index of the icon which invoked the event ie(icon 2 was clicked, so we should set player 1 to bulwark)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void UIToggleActiveTransition_OnIconSelected(object sender, UIToggleActiveTransitionEventArgs e)
        {
            if(PlayerDataManager.InstanceExists)
            {
                int index = e.index;//Each icon on the panel has an icon index value. The first 4 are for the rogue, next set is for the bulwark etc.
                if (index < 4)//player 1
                {
                    Debug.Log("Player 1 selected icon " + index);
                    PlayerDataManager.Instance.UpdateCharacterClassPreset(0, classes[index]);
                }
                else if (index > 3 && index < 8)//Player 2
                {
                    index -= 4; //Set the index back in the range of 0-3
                    Debug.Log("Player 2 selected icon " + index);
                    PlayerDataManager.Instance.UpdateCharacterClassPreset(1, classes[index]);
                }
                else if (index > 7 && index < 12)//Player 3
                {
                    index -= 8; //Set the index back in the range of 0-3
                    PlayerDataManager.Instance.UpdateCharacterClassPreset(2, classes[index]);
                }
                else if (index > 11 && index < 16)//Player 4
                {
                    index -= 12; //Set the index back in the range of 0-3
                    PlayerDataManager.Instance.UpdateCharacterClassPreset(3, classes[index]);
                }
            }
        }
    }
}

