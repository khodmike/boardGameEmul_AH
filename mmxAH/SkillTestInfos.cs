using System;

namespace mmxAH
{

	public class SkillTestInfos
	{ private SkillTestInfoEntry [] infos; 
		private short mpmodif;
		public SkillTestInfos ()
		{ infos= new SkillTestInfoEntry[SkillTest.skill_test_types_count];
			for (int i = 0; i < infos.Length; i++)
			{ infos [i] = new SkillTestInfoEntry ();
				 infos [i].modif=0;
				infos [i].ClueDiceRathio = 1;
				infos [i].isReroll = false;
				mpmodif = 0;
			}

		}

		public SkillTestInfoEntry  GetInfo(SkillTestType t)
		{
			return infos[(int)t];
		}

	 
		public string PrintInfo(SkillTestType t, SystemStrings strs)
		{ SkillTestInfoEntry info= infos[(int)t];
			string res="";
			if (info.modif != 0)
			{ if (info.modif > 0)
					res += "+";
			 res += info.modif.ToString ();

			}
			if (info.ClueDiceRathio != 1)
			{ if (res != "")
					res += ", ";
			  res += strs.GetNumberClueToken (1);
			  res += " = + " + info.ClueDiceRathio + "d6"; 



			}

			if (info.isReroll )
			{ if (res != "")
				res += ", ";
			  res += strs.GetString (SSType.Reroll);  

			}

			if (res != "")
				res = "\t" + strs.GetCharekteresticName (t) + " : " + res;

			if( t== SkillTestType.Speed && mpmodif != 0)
			{    
				res += "\t"+strs.GetString( SSType.MovementPoints)  ;
					if( mpmodif > 0)
						res+= "+";
				res+= mpmodif;



			}
			return res;

		}


		public void ChangeMpModif(short changevalue)
		{
			mpmodif+= changevalue;


		}
		public short GetMPModif()
		{
			return mpmodif; 
		}
	}

	public class SkillTestInfoEntry
	{ public short modif;
	  public byte ClueDiceRathio;
	  public bool isReroll;
      


	}
	
}
