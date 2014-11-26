using System;

namespace mmxAH
{
	public  class GlobalActhions
	{ private GameEngine en;
		Func rp;
		Investigator invest;
		public GlobalActhions ( GameEngine eng)
		{en=eng;

		} 

		public void ReturnToArchem (short ow, Func retFunc , byte inv=40)
		{ rp=retFunc;
			if (inv == 40)
				inv = en.clock.GetCurPlayer (); 
		    invest = en.ActiveInvistigators [inv]; 
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
			{ RetToArchem2 (potLoc [0]);
				return;

			
			}

			string promt = invest.GetTitle () + "  " + en.sysstr.GetString (SSType.MonsterMovePromt1) + "  " + invest.GetPronaun() +"  " + en.sysstr.GetString (SSType.MonsterMovePromt2);
			System.Collections.Generic.List<IOOption> opts = new System.Collections.Generic.List<IOOption> ();
			foreach (short l in potLoc)
				opts.Add (new IOOptionWithParam (en.locs [l].GetTitle (), RetToArchem2 , l));
			en.io.StartChoose (opts, promt, en.sysstr.GetString (SSType.ChooseActhionButton));


		}


		private void RetToArchem2( short loc)
		{  
			invest.SetLocathion (loc);
			en.io.ServerWrite (invest.GetTitle(), 12, true, true);
			en.io.ServerWrite (" " + en.sysstr.GetString (SSType.ReturnToArchem) + " ");
			en.io.ServerWrite (en.locs [loc].GetTitle()+"."+ Environment.NewLine, 12, false, true);
			invest.isExploredToken = true; 
			invest.isCanMove = false;
			invest.isMonsterImunity = true;
			rp ();

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

