using System;

namespace mmxAH
{
	public  class GlobalActhions
	{ private GameEngine en;
		Func rp;
		public GlobalActhions ( GameEngine eng)
		{en=eng;

		} 

		public void ReturnToArchem (short ow, Func retFunc , byte inv=40)
		{ rp=retFunc;
			if (inv == 40)
				inv = en.clock.GetCurPlayer (); 
			Investigator invest = en.ActiveInvistigators [inv]; 
			System.Collections.Generic.List< short> potLoc = new System.Collections.Generic.List<short> ();
			foreach (GatePrototype g in en.openGates)
				if (g.GetOW () == ow)
					potLoc.Add (g.GetArchemLoc());
			 
			if (potLoc.Count  == 0)
			{
				new EffLitas (en).Execute (rp, inv);
				return;
			}
			if (potLoc.Count == 1)
			{
				invest.SetLocathion (potLoc [0]);
				en.io.ServerWrite (invest.GetTitle(), 12, true, true);
				en.io.ServerWrite (" " + en.sysstr.GetString (SSType.ReturnToArchem) + " ");
				en.io.ServerWrite (en.locs [potLoc [0]].GetTitle()+"."+ Environment.NewLine, 12, false, true);
				invest.isExploredToken = true; 
				invest.isCanMove = false;
				rp ();
			
			}


		}

		public void Awekeen()
		{

		}

		public void SetupMythos()
		{ //ЗАГЛУШКА
			en.clock.EndTurn();

		}

		public void MonsterSurge( short startLoc)
		{
			en.curs.resolvingMythos.Step2 ();

		}


	}
}

