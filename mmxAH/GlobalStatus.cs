using System;

namespace mmxAH
{
	public class GlobalStatus
	{ GameEngine en;
		private  byte CurDoom, CurGate, CurMonsters, CurOut, CurTerror, CurSealed;
		private byte MaxDoom, MaxGate, MaxMonsters, MaxOut;
		private const byte Terror1=3, Terror2=6, Terror3=9, MaxTerror=10, MaxSealed=6;
		public GlobalStatus ( GameEngine eng)
		{ en=eng;
			Reset ();
		}

		public void Print()
		{ en.clock.PrintCurPhase(false); 
			en.io.ClientWrite (en.sysstr.GetString (SSType.DoomTrack), 12, true);
		  en.io.ClientWrite (" " + CurDoom + " / " + MaxDoom+ "."+ Environment.NewLine );  
			en.io.ClientWrite ( en.sysstr.GetString (SSType.OpenGates ), 12, true);
			en.io.ClientWrite (" " + CurGate + " / " + MaxGate+ "."+ Environment.NewLine ); 
			en.io.ClientWrite (en.sysstr.GetString (SSType.MonsterInArchem   ), 12, true);
			en.io.ClientWrite (" " + CurMonsters + " / " + MaxMonsters + "."+ Environment.NewLine );
			en.io.ClientWrite (en.sysstr.GetString (SSType.MonsterInOutscirts   ), 12, true);
			en.io.ClientWrite (" " + CurOut + " / " + MaxOut + "."+ Environment.NewLine );  
			en.io.ClientWrite (en.sysstr.GetString (SSType.TerrorTrack  ), 12, true);
			en.io.ClientWrite (" " + CurTerror + " / " + MaxTerror + "."+ Environment.NewLine ); 
			en.io.ClientWrite (Environment.NewLine+ en.sysstr.GetString (SSType.SealedLocathion   ), 12, true);
			en.io.ClientWrite (" " + CurSealed + " / " + MaxSealed + "."+ Environment.NewLine ); 

		}

		public void Init( byte pMaxGate, byte pMaxMonsters, byte pMaxOut)
		{  MaxGate=pMaxGate;
			MaxMonsters = pMaxMonsters;
			MaxOut = pMaxOut;

		}

		public void SetMaxDoom( byte pMaxDoom)
		{
			MaxDoom = pMaxDoom;
		}

		public void Reset()
		{ CurDoom=0;
			CurGate = 0; 
			CurMonsters = 0;
			CurOut = 0;
			CurTerror = 0;
			CurSealed = 0;
			MaxDoom = 0;
			MaxGate = 0;
			MaxMonsters = 0;
			MaxOut = 0;

			//заглушка
			MaxDoom = 10;



		}

		public bool DoomIncrise( byte count=1)
		{ CurDoom+=count;
			en.io.ServerWrite (en.sysstr.GetNumberDoomToken (count) + " " + en.sysstr.GetString (SSType.DoomInc));
			en.io.ServerWrite (" " + en.sysstr.GetString (SSType.DoomTrack), 12, true);
			en.io.ServerWrite (" " + CurDoom + " / " + MaxDoom+ "."+ Environment.NewLine );  

			if (CurDoom >= MaxDoom)
			{ en.ga.Awekeen ();
				return false;

			}
			return true;

		}


		public bool NewGate()
		{ CurGate ++;
			en.io.ServerWrite (" " + en.sysstr.GetString (SSType.OpenGates ), 12, true);
			en.io.ServerWrite (" " + CurGate + " / " + MaxGate+ "."+ Environment.NewLine );  

			if (CurGate >= MaxGate)
			{ en.ga.Awekeen ();
				return false;

			}
			return true;

		}


		public bool IsMonserToPlace(MonsterIndivid m)
		{ if (CurMonsters + 1 > MaxMonsters)
			{  

			}

		}



	}
}

