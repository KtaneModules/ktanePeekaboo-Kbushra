using System.Collections.Generic;
using UnityEngine;
using KModkit;
using Newtonsoft.Json;
using System.Linq;
using System.Text.RegularExpressions;
using Rnd = UnityEngine.Random;

public class SimpleModuleScript : MonoBehaviour {

	public KMAudio audio;
	public KMBombInfo info;
	public KMBombModule module;
	public KMBossModule BossModule;
	public KMSelectable[] Buttons;
	static int ModuleIdCounter = 1;
	int ModuleId;

	public AudioSource correct;

	bool _isSolved = false;
	bool incorrect = false;

	public int orderInt = 0;

	private string[] ignoredModules;
	public int StagesTotes;
	public int stageCur;

	public Transform[] movers;
	public Transform[] targets;
	public Transform[] returnTargets;
	public Transform cube;
	public Transform cubeReturn;
	public float t;


	void Awake() 
	{
		ModuleId = ModuleIdCounter++;

		foreach (KMSelectable button in Buttons)
		{
			KMSelectable pressedButton = button;
			button.OnInteract += delegate () { buttons(pressedButton); return false; };
		}
	}

	void Start()
	{

		if (ignoredModules == null) 
		{
			ignoredModules = BossModule.GetIgnoredModules ("Peek-A-Boo", new string[] 
			{
					"14",
					"Cruel Purgatory",
					"Forget Enigma",
					"Forget Everything",
					"Forget It Not",
					"Forget Infinity",
					"Forget Me Later",
					"Forget Me Not",
					"Forget Perspective",
					"Forget Them All",
					"Forget This",
					"Forget Us Not",
					"Organization",
					"Purgatory",
					"Simon's Stages",
					"Souvenir",
					"Tallordered Keys",
					"The Time Keeper",
					"Timing is Everything",
					"The Troll",
					"Turn The Key",
					"Übermodule",
					"Ültimate Custom Night",
					"The Very Annoying Button",
					"Remember Simple",
					"Remembern't Simple",
					"Peek-A-Boo"
			});

			module.OnActivate += delegate () 
			{ 
				StagesTotes = info.GetSolvableModuleNames ().Where (a => !ignoredModules.Contains (a)).ToList ().Count;
				if (StagesTotes > 0) 
				{
					Log ("Yes Solvables");
				} 
				else 
				{
					Log ("No Solvables");
				}
			};
		};
		Invoke ("StageTotalSetup", 1);
	}

	void StageTotalSetup()
	{
		StagesTotes = info.GetSolvableModuleNames ().Where (a => !ignoredModules.Contains (a)).ToList ().Count;
		Debug.LogFormat ("Total stages are {0}", StagesTotes);
	}

	void Update()
	{
		stageCur = info.GetSolvedModuleNames ().Where (a => !ignoredModules.Contains (a)).ToList ().Count;
	}

	void buttons(KMSelectable pressedButton)
	{
		GetComponent<KMAudio>().PlayGameSoundAtTransformWithRef(KMSoundOverride.SoundEffect.ButtonPress, transform);
		int buttonPosition = new int();
		for(int i = 0; i < Buttons.Length; i++)
		{
			if (pressedButton == Buttons[i])
			{
				buttonPosition = i;
				break;
			}
		}
		switch (buttonPosition) 
		{
		case 0:
			if (orderInt == 0) 
			{
				if (stageCur == 0) 
				{
					orderInt++;
					Log ("Button press done!");
					correct.Play ();
					Invoke ("TL", 0);
				}
				else 
				{
					incorrect = true;
					Log ("Solved module amount is not the required amount.");
				}
			}
			else 
			{
				incorrect = true;
				Log ("Wrong order");
			}
			break;
		case 1:
			Invoke ("MoveRest", 0);
			Log ("Buttons revealed");
			correct.Play ();

			if (orderInt == 3) 
			{
				Invoke ("M", 0);
				module.HandlePass ();
			}
			break;
		case 2:
			if (orderInt == 1) 
			{
				if (StagesTotes < 4) 
				{
					if (info.GetTime() < 601)
					{
						orderInt++;
						Log ("Button press done!");
						correct.Play ();
						Invoke ("BL", 0);
					}
					else 
					{
						incorrect = true;
						Log ("Time is not the required amount.");
					}
				}
				else 
				{
					if (stageCur == 4) 
					{
						orderInt++;
						Log ("Button press done!");
						correct.Play ();
						Invoke ("BL", 0);
					}
					else
					{
						incorrect = true;
						Log ("Solved modules are not the required amount.");
					}
				}
			}
			else 
			{
				incorrect = true;
				Log ("Wrong order");
			}
			break;
		case 3:
			if (orderInt == 2) 
			{
				if (StagesTotes < 7) 
				{
					if (info.GetTime () < 601) 
					{
						orderInt++;
						Log ("Button press done!");
						correct.Play ();
						Invoke ("BR", 0);

					}
					else 
					{
						incorrect = true;
						Log ("Time is not the required amount.");
					}
				} 
				else
				{
					if (stageCur == 7) 
					{
						orderInt++;
						Log ("Button press done!");
						correct.Play ();
						Invoke ("BR", 0);
					}
					else 
					{
						incorrect = true;
						Log ("Solved modules are not the required amount.");
					}
				}
			}
			else 
			{
				incorrect = true;
				Log ("Wrong order!");
			}
			break;
		}
		if (incorrect) 
		{
			module.HandleStrike ();
			incorrect = false;
		}
		else
		{
			//nothing
		}
	}

	void Log(string message)
	{
		Debug.LogFormat("[Peek-A-Boo #{0}] {1}", ModuleId, message);
	}

	void MoveRest()
	{
		Vector3 a1 = movers[0].position;
		Vector3 b1 = targets[0].position;
		movers[0].position = Vector3.Lerp (a1, b1, t);

		Vector3 a2 = movers[1].position;
		Vector3 b2 = targets[1].position;
		movers[1].position = Vector3.Lerp (a2, b2, t);

		Vector3 a3 = movers[2].position;
		Vector3 b3 = targets[2].position;
		movers[2].position = Vector3.Lerp (a3, b3, t);

		Invoke ("MoveRest", 0);
	}

	void TL()
	{
		Vector3 a1 = movers[0].position;
		Vector3 b1 = returnTargets[0].position;
		movers[0].position = Vector3.Lerp (a1, b1, t);

		Invoke ("TL", 0);
	}

	void BL()
	{
		Vector3 a2 = movers[1].position;
		Vector3 b2 = returnTargets[1].position;
		movers[1].position = Vector3.Lerp (a2, b2, t);

		Invoke ("BL", 0);
	}

	void BR()
	{
		Vector3 a3 = movers[2].position;
		Vector3 b3 = returnTargets[2].position;
		movers[2].position = Vector3.Lerp (a3, b3, t);

		Invoke ("BR", 0);
	}

	void M()
	{
		Vector3 a4 = movers[3].position;
		Vector3 b4 = returnTargets[3].position;
		movers[3].position = Vector3.Lerp (a4, b4, t);

		Vector3 a5 = cube.position;
		Vector3 b5 = cubeReturn.position;
		cube.position = Vector3.Lerp (a5, b5, t);

		Invoke ("M", 0);
	}
}
