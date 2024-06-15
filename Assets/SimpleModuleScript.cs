using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using KModkit;
using Newtonsoft.Json;
using System.Linq;
using System.Text.RegularExpressions;
using Rnd = UnityEngine.Random;
using System;

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
	private bool moved = false;

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

		if (!_isSolved)
		{
			switch (buttonPosition)
			{
				case 0:
					if (!moved) break;
					if (orderInt == 0)
					{
						if (stageCur == 0)
						{
							orderInt++;
							Log("Button press done!");
							correct.Play();
							Invoke("TL", 0);
						}
						else
						{
							incorrect = true;
							Log("Solved module amount is not the required amount.");
						}
					}
					else
					{
						incorrect = true;
						Log("Wrong order");
					}
					break;
				case 1:
					if (!moved)
					{
						Invoke("MoveRest", 0);
						Log("Buttons revealed");
						correct.Play();
					}
					if (orderInt == 3)
					{
						Invoke("M", 0);
						module.HandlePass();
						_isSolved = true;
					}
					break;
				case 2:
                    if (!moved) break;
                    if (orderInt == 1)
					{
						if (StagesTotes < 4)
						{
							if (info.GetTime() < 601)
							{
								orderInt++;
								Log("Button press done!");
								correct.Play();
								Invoke("BL", 0);
							}
							else
							{
								incorrect = true;
								Log("Time is not the required amount.");
							}
						}
						else
						{
							if (stageCur == 4)
							{
								orderInt++;
								Log("Button press done!");
								correct.Play();
								Invoke("BL", 0);
							}
							else
							{
								incorrect = true;
								Log("Solved modules are not the required amount.");
							}
						}
					}
					else
					{
						incorrect = true;
						Log("Wrong order");
					}
					break;
				case 3:
                    if (!moved) break;
                    if (orderInt == 2)
					{
						if (StagesTotes < 7)
						{
							if (info.GetTime() < 601)
							{
								orderInt++;
								Log("Button press done!");
								correct.Play();
								Invoke("BR", 0);

							}
							else
							{
								incorrect = true;
								Log("Time is not the required amount.");
							}
						}
						else
						{
							if (stageCur == 7)
							{
								orderInt++;
								Log("Button press done!");
								correct.Play();
								Invoke("BR", 0);
							}
							else
							{
								incorrect = true;
								Log("Solved modules are not the required amount.");
							}
						}
					}
					else
					{
						incorrect = true;
						Log("Wrong order!");
					}
					break;
			}
			if (incorrect)
			{
				module.HandleStrike();
				incorrect = false;
			}
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

		if (a1.y <= b1.y - 0.0005f) Invoke("MoveRest", 0);
		else moved = true;
	}

	void TL()
	{
		Buttons[0].gameObject.GetComponent<KMSelectable>().Highlight.gameObject.SetActive(false);

		Vector3 a1 = movers[0].position;
		Vector3 b1 = returnTargets[0].position;
		movers[0].position = Vector3.Lerp (a1, b1, t);

        if (a1 != b1) Invoke("TL", 0);
	}

	void BL()
	{
        Buttons[2].gameObject.GetComponent<KMSelectable>().Highlight.gameObject.SetActive(false);

        Vector3 a2 = movers[1].position;
		Vector3 b2 = returnTargets[1].position;
		movers[1].position = Vector3.Lerp (a2, b2, t);

        if (a2 != b2) Invoke ("BL", 0);
    }

	void BR()
	{
        Buttons[3].gameObject.GetComponent<KMSelectable>().Highlight.gameObject.SetActive(false);

        Vector3 a3 = movers[2].position;
		Vector3 b3 = returnTargets[2].position;
		movers[2].position = Vector3.Lerp (a3, b3, t);

        if (a3 != b3) Invoke ("BR", 0);
    }

	void M()
	{
        Buttons[0].gameObject.GetComponent<KMSelectable>().Highlight.gameObject.SetActive(false);

        Vector3 a4 = movers[3].position;
		Vector3 b4 = returnTargets[3].position;
		movers[3].position = Vector3.Lerp (a4, b4, t);

		Vector3 a5 = cube.position;
		Vector3 b5 = cubeReturn.position;
		cube.position = Vector3.Lerp (a5, b5, t);

        if (a4 != b4) Invoke ("M", 0);
    }

	#pragma warning disable 414
	private readonly string TwitchHelpMessage = @"!{0} press M/TL/BL/BR [Presses button at specified position if activated]";
	#pragma warning restore 414
    IEnumerator ProcessTwitchCommand(string command)
	{
        Dictionary<string, int> position = new Dictionary<string, int>() { { "TL", 0 }, { "M", 1 }, { "BL", 2 }, { "BR", 3 } };
		string[] parameters = command.Split(' ');

		if (parameters[0] == "press")
		{
			if (parameters.Length < 2) yield return "sendtochaterror Too little parameters!";
			else if (parameters.Length > 2) yield return "sendtochaterror Too many parameters!";
			else
			{
				parameters[1] = parameters[1].ToUpper();

				if (!position.ContainsKey(parameters[1])) yield return "sendtochaterror Button position does not exist!";
				else if ((!moved && parameters[1] != "M") || !Buttons[position[parameters[1]]].gameObject.GetComponent<KMSelectable>().Highlight.gameObject.activeSelf) 
					yield return "sendtochaterror Button not activated!";
				else yield return null; Buttons[position[parameters[1]]].OnInteract();
			}
        }
		else yield return "sendtochaterror Invalid parameter!";
	}
	IEnumerator TwitchHandleForcedSolve()
	{
        yield return null;
        module.HandlePass();
		TL(); BL(); BR(); M();
	}
}
