using System;
using System.Collections.Generic; 
namespace mmxAH
{
	public class SkillTest
	{ public  const byte skill_test_types_count=10;
		private short totalDice;
		private byte successes;
		private byte ClueDice;
		private FuncWithParam RP;
		private byte needSuc;
		private Investigator  inv;
		private SkillTestType typ;
		private GameEngine en;
		public SkillTest ( GameEngine eng, SkillTestType type, short modif,   FuncWithParam returnPoint,  byte needSuccess= 1, bool isSecondType=false, SkillTestType type2= SkillTestType.Speed, byte modif2=0)
		{  en=eng;
			typ = type;
			inv = en.ActiveInvistigators [en.clock.GetCurPlayer ()];
			SkillTestInfoEntry  info1, info2, infog1, infog2;
			info1 = inv.STInfo.GetInfo(type);
			infog1 = en.GlobalModifs.GetInfo (type);
			totalDice=(short)(modif+ modif2+ inv.GetCharValue(type)+info1.CharModif +infog1.CharModif+ info1.SCmodif+ infog1.SCmodif); 
			ClueDice = (byte)(Math.Max (info1.ClueDiceRathio, infog1.ClueDiceRathio));  
		
			if (isSecondType)
			{ info2 = inv.STInfo.GetInfo(type2);
				infog2 = en.GlobalModifs.GetInfo (type2);
				totalDice += (short)(inv.GetCharValue (type2) + info2.CharModif + info2.SCmodif+ infog2.CharModif + infog2.SCmodif);
				ClueDice = (byte)(Math.Max (ClueDice, Math.Max(info1.ClueDiceRathio, infog1.ClueDiceRathio)));  

			}

			if (totalDice < 0)
				totalDice = 0; 
			 
			string str=en.sysstr.GetString (SSType.SkillCheck) + " {"+en.sysstr.GetCharekteresticName (type);
			if (modif >= 0)
				str += "+";
			str+= modif+ " } ";
			en.io.ServerWrite (str, 12, true);

			needSuc = needSuccess;  
			RP = returnPoint;
		

			successes = new DiceRoller (en).RollDiceWithTreshhold ((byte)totalDice, inv.GetSTTresh() );
			AfterRoll (); 


		}


		private void AfterRoll()
		{ en.io.ServerWrite (en.sysstr.GetString (SSType.TotalSuccesses1) + " " + successes + " " + en.sysstr.GetString (SSType.TotalSuccesses2) + Environment.NewLine);  

			if (successes >= needSuc || needSuc == 255)
				Finish ();
			else
				CluesAndRerolls ();

		}
		private void Finish()
		{
			if (successes >= needSuc)
				en.io.ServerWrite (en.sysstr.GetString (SSType.SkillCheck) + " " + en.sysstr.GetString (SSType.SCPass)+ ". ", 12, true); 
			else
				if( needSuc != 255) // не таблица по успехам
					en.io.ServerWrite (en.sysstr.GetString (SSType.SkillCheck) + " " + en.sysstr.GetString (SSType.SCFail)+ ". ", 12, true); 

			RP( successes); 
          
		}


		private void CluesAndRerolls()
		{ List<IOOption> opts= new List<IOOption>();
			foreach (IOOption o in inv.myTrigers.GetChooseOpthions(TrigerEvent.SCRerolls, (short) typ))
				opts.Add (o);
			if ( inv.GetCluesValue () > 0)
			{
				string str = en.sysstr.GetString (SSType.DiscardAction) +" "+ en.sysstr.GetNumberClueToken (1);
				str += " :  + " + ClueDice + "d6";
				opts.Add( new IOOpthionWithoutParam(str, AddDiceForClue));
			}

			if (opts.Count == 0)
				Finish ();
			else
			{ opts.Add( new IOOpthionWithoutParam(en.sysstr.GetString(SSType.EndSC),Finish));
				string str =  inv.GetTitle () + " " + en.sysstr.GetString (SSType.Has);
				str += " " + en.sysstr.GetNumberClueToken ( inv.GetCluesValue ());
				str += Environment.NewLine + en.sysstr.GetString (SSType.ChooseActhioPromt);  
				en.io.StartChoose (opts,str , en.sysstr.GetString (SSType.ChooseActhionButton));  
			}
          

		}

	    private void AddDiceForClue()
		{ inv.RemoveClues (1);
			totalDice++;
			successes += new DiceRoller (en).RollDiceWithTreshhold (ClueDice , inv.GetSTTresh() );   
			AfterRoll ();
		}


	}




	public enum SkillTestType
	{ Speed=0,
      Sneak,
      Fight,
      Will,
      Lore,
      Luck,
      Evade,
      Combat,
      Horror,
      SpellCast

	}


}

