﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Com.Mygame;

public class FirstController : MonoBehaviour, SceneController, UserAction {

	UserGUI userGUI;

	public CoastController fromCoast;
	public CoastController toCoast;
	public BoatController boat;
	private MyCharacterController[] characters;
	public Judger judger;
	private FirstSceneActionManager actionManager;

	void Awake() {
		Director director = Director.getInstance ();
		director.currentSceneController = this;
		userGUI = gameObject.AddComponent <UserGUI>() as UserGUI;
		characters = new MyCharacterController[6];
		judger = new Judger();
		loadResources ();
	}

	void Start() {
		actionManager = GetComponent<FirstSceneActionManager>();
	}

	public void loadResources() {
		//GameObject water = Instantiate (Resources.Load ("Perfabs/Water", typeof(GameObject)), water_pos, Quaternion.identity, null) as GameObject;
		//water.name = "water";

		fromCoast = new CoastController ("from");
		toCoast = new CoastController ("to");
		boat = new BoatController ();

		loadCharacter ();
	}

	private void loadCharacter() {
		for (int i = 0; i < 3; i++) {
			MyCharacterController cha = new MyCharacterController ("priest");
			cha.setName("priest" + i);
			cha.setPosition (fromCoast.getEmptyPosition ());
			cha.getOnCoast (fromCoast);
			fromCoast.getOnCoast (cha);

			characters [i] = cha;
		}

		for (int i = 0; i < 3; i++) {
			MyCharacterController cha = new MyCharacterController ("devil");
			cha.setName("devil" + i);
			cha.setPosition (fromCoast.getEmptyPosition ());
			cha.getOnCoast (fromCoast);
			fromCoast.getOnCoast (cha);

			characters [i+3] = cha;
		}
	}


	public void moveBoat() {
		if (boat.isEmpty ())
			return;
		/*	old way to move boat
		boat.Move ();
		*/
		actionManager.moveBoat(boat);
		boat.move();
		userGUI.status = judger.check_game_over(fromCoast,toCoast,boat);
	}

	public void characterIsClicked(MyCharacterController characterCtrl) {
		if (characterCtrl.isOnBoat ()) {
			CoastController whichCoast;
			if (boat.get_to_or_from () == -1) { // to->-1; from->1
				whichCoast = toCoast;
			} else {
				whichCoast = fromCoast;
			}

			boat.GetOffBoat (characterCtrl.getName());
			//characterCtrl.moveToPosition (whichCoast.getEmptyPosition ());
			actionManager.moveCharacter(characterCtrl, whichCoast.getEmptyPosition ());
			characterCtrl.getOnCoast (whichCoast);
			whichCoast.getOnCoast (characterCtrl);

		} else {									// character on coast
			CoastController whichCoast = characterCtrl.getCoastController ();

			if (boat.getEmptyIndex () == -1) {		// boat is full
				return;
			}

			if (whichCoast.get_to_or_from () != boat.get_to_or_from ())	// boat is not on the side of character
				return;

			whichCoast.getOffCoast(characterCtrl.getName());
			//characterCtrl.moveToPosition (boat.getEmptyPosition());
			actionManager.moveCharacter(characterCtrl, boat.getEmptyPosition());
			characterCtrl.getOnBoat (boat);
			boat.GetOnBoat (characterCtrl);
		}
		userGUI.status = judger.check_game_over(fromCoast,toCoast,boat);
	}
/* 
	int check_game_over() {	// 0->not finish, 1->lose, 2->win
		int from_priest = 0;
		int from_devil = 0;
		int to_priest = 0;
		int to_devil = 0;

		int[] fromCount = fromCoast.getCharacterNum ();
		from_priest += fromCount[0];
		from_devil += fromCount[1];

		int[] toCount = toCoast.getCharacterNum ();
		to_priest += toCount[0];
		to_devil += toCount[1];

		if (to_priest + to_devil == 6)		// win
			return 2;

		int[] boatCount = boat.getCharacterNum ();
		if (boat.get_to_or_from () == -1) {	// boat at toCoast
			to_priest += boatCount[0];
			to_devil += boatCount[1];
		} else {	// boat at fromCoast
			from_priest += boatCount[0];
			from_devil += boatCount[1];
		}
		if (from_priest < from_devil && from_priest > 0) {		// lose
			return 1;
		}
		if (to_priest < to_devil && to_priest > 0) {
			return 1;
		}
		return 0;			// not finish
	}*/

	public void restart() {
		boat.reset ();
		fromCoast.reset ();
		toCoast.reset ();
		for (int i = 0; i < characters.Length; i++) {
			characters [i].reset ();
		}
	}
}



