using System;

namespace mmxAH
{
	public enum SSType
	{ ResumeGame=0,
	  NewGame,
      LoadGame,
      SaveGame,
      Preference,
      Exit,
      MainMenuButton,
      Title,
      GateTo,
	  DS,
	  MonsterEncounter,
	  ChooseActhioPromt,
	  ChooseActhionButton,
      MoveToPropos,
      MoveToFact,
	  OWArea,
	 MovementEndPromt,
     RemainMP,
	 MonsterEncounterVerb,
     NowHas,
	 CluesPromtBegin,
	 CluesPromtEnd,
	 ClueActhionButtonName,
	 Get,
     ButtonStatus,
     ButtonLog,
     ButtonInvest,
     ButtonAO,
	 ButtonMonsters,
	 ButtonOW,
	 ButtonArchemMap,
	 FirstPlayer,
	 Speed,
     Sneak,
	 Fight,
	 Will,
	 Lore,
	 Luck,
	 Evade,
	 Combat,
	 Horror,
	 SpellCast,
	 SkillCheck,
     Stamina,
     Sanity,
     Focus,
     Reroll,
     TotalSuccesses1,
     TotalSuccesses2,
     SCPass,
     SCFail,
     DiscardAction,
     DiscardFact,
     Remain,
	 EndSC,
	 Has,
		Turn,
	 Setup,
		Upkeep,
		Movement,
		ArcEncPhase,
		OwEncPhase,
		Mythos,
		TurnEndMessage,
		Gain,
		Lose, 
		isPlacedTo,
		SetYourSlider,
		SetupSliders,
		MovementPoints,
		DelayedStart,
  		DelaydEnd,
		Delayed, 
		ClosedFact,
		SealedFact,
		Awerness,
		HorrorModif,
		HorrorDemage,
		ExtraHorror,
		CombatModif,
		CombatDemage,
		ExtraCombat,
		SpecCombDemage,
		Tougness,
		MI,
		MR,
		PI,
		PR,
		MonsMoveTitle,
	    MonsMoveNormal,
		MonsMoveFast,
		MonsMovFly,
		MonsMoveSpec,
		MonsMovInmob,
		EvadeOrFight1,
		EvadeOrFight2,
		EvadeButton,
		FightButton,
		Ambush,
		Endless,
		SpecialCards,
		MonsterTrofies,
		GateTrofies,
		GateCant1,
		GateCant2,
		MythosStep1,
		MythosStep2,
		MythosStep3,
		MythosStep4,
		GateDrawn,
		GateOpenVerb,
		MonsterPlacedVerb,
		DoomInc,
		DoomDecrease,
		DoomTrack,
		OpenGates,
		TerorInc,
		TerrorTrack,
		MonsterInArchem,
		PlacedToOut,
		MonsterInOutscirts,
		SealedLocathion,
		OutscirtsIsClear,
		ClueAppear,
		ClueCouldNotAppear1,
		ClueCouldNotAppear2,
		PickupClueOnMythosPromt,
		Yes,
		No,
		MonsterDemageSpecial,
		Undead,
		Mask,
		Spawn,
		ToChecks,
		MonsterMovePromt,
		Confirm,
		Scy,
		Setup_Started,
		Setup_NumOfInv_Promt,
		Setup_NumOfInv_Fact,
		Investigitor,
		SetupRandomStep,
		SetupSlidersStep,
		SetupMythosStep








	}


	public class SystemStrings
	{ private int count= Enum.GetValues(typeof(SSType)).Length;   
	 private string[]  systemStringsArr;
		private  string Cluetoken1="", Cluetoken2="", Cluetoken3="", Doomtoken1="", Doomtoken2="", Doomtoken3="";
		public SystemStrings ()
		{
			systemStringsArr= new string[count];
		}

		public void FromTextFile (TextFileParser pars)
		{
			for (byte i=0; i<count; i++)
				systemStringsArr [i] = pars.GetCurString (); 

			 Cluetoken1= pars.GetCurString ();
				Cluetoken2=pars.GetCurString ();
			if (pars.isMultiName)
				Cluetoken3=pars.GetCurString ();

			Doomtoken1= pars.GetCurString ();
			Doomtoken2=pars.GetCurString ();
			if (pars.isMultiName)
				Doomtoken3=pars.GetCurString ();




		}

		public string GetString( SSType t)
		{ 
			return systemStringsArr[(int) t];
		

		}

		public string GetNumberClueToken( short num)
		{ string t, res=num.ToString()+ "  ";

			if (Cluetoken3 == "") //eng
			{ if (num < 2)
					return res + Cluetoken1;
				else
					return res + Cluetoken2; 

			} else //rus
			{
				if (num >= 11 && num <= 19)
					return res + Cluetoken3 + "  ";
				while (num > 9)
					num = (short)(num % 10);
				if (num == 1)
					t = Cluetoken1;
				else if (num != 0 && num <= 4)
					t = Cluetoken2;
				else
					t = Cluetoken3;
				return res + t + "  ";
			}
		}

		public string GetNumberDoomToken( short num)
		{ string t, res=num.ToString()+ "  ";

			if (Doomtoken3 == "") //eng
			{ if (num < 2)
				return res + Doomtoken1;
				else
					return res + Doomtoken2; 

			} else //rus
			{
				if (num >= 11 && num <= 19)
					return res + Doomtoken3 + "  ";
				while (num > 9)
					num = (short)(num % 10);
				if (num == 1)
					t = Doomtoken1;
				else if (num != 0 && num <= 4)
					t = Doomtoken2;
				else
					t = Doomtoken3;
				return res + t + "  ";
			}
		}




		public void ToBinary( System.IO.BinaryWriter wr)
		{   foreach( string str in systemStringsArr)
				wr.Write (str);
			wr.Write (Cluetoken1);
			wr.Write (Cluetoken2);
			wr.Write (Cluetoken3);
		}

		public void FromBinary(System.IO.BinaryReader  rd)
		{
			for (byte i=0; i<count; i++)
				systemStringsArr [i] = rd.ReadString  (); 
			  
			    Cluetoken1= rd.ReadString  ();
				Cluetoken2=rd.ReadString ();
				Cluetoken3=rd.ReadString ();




		}

		public string GetCharekteresticName(SkillTestType t)
		{ 	switch(t)
		{ case SkillTestType.Speed: return GetString (SSType.Speed); 
		  case SkillTestType.Sneak: return GetString (SSType.Sneak); 
		  case SkillTestType.Fight: return GetString (SSType.Fight); 
		  case SkillTestType.Will: return GetString (SSType.Will);
		  case SkillTestType.Lore: return GetString (SSType.Lore);
		   case SkillTestType.Luck: return GetString (SSType.Luck); 
		   case SkillTestType.Evade: return GetString (SSType.Evade);
			case SkillTestType.Combat: return GetString (SSType.Combat); 
			case SkillTestType.Horror: return GetString (SSType.Horror); 
			case SkillTestType.SpellCast : return GetString (SSType.SpellCast); 
			default: return "";
			}


		}

		public string GetMonsterMovementString( MonsterMovementType t)
		{ string str= GetString( SSType.MonsMoveTitle)+ " ";
			switch(t)
			{ case MonsterMovementType.Normal : return str+ GetString (SSType.MonsMoveNormal);
			  case MonsterMovementType.Fast : return str+ GetString (SSType.MonsMoveFast ); 
			case MonsterMovementType.Fly : return str+ GetString (SSType.MonsMovFly ); 
			case MonsterMovementType.Special : return str+ GetString (SSType.MonsMoveSpec ); 
			case MonsterMovementType.Inmobile:  return str+ GetString (SSType.MonsMovInmob ); 

				default: return "";
			}

		}
	}
}

