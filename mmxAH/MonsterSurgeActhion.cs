using System;

namespace mmxAH
{
	public class MonsterSurgeActhion
	{ private  GameEngine en;
		short surgeLoc;
		byte MonstersInEachGate=0, MonstersToOutscirts=0;
		bool isExtraMonsterToSurgeGate=false;
		public MonsterSurgeActhion ( GameEngine eng, short pSurgheLoc )
		{ en=eng;
			surgeLoc = pSurgheLoc;
		}


		public void Execute()
		{   byte ToPlace=  Math.Max( (byte) en.openGates.Count, en.GetPlayersNumber()); 
			byte CouldBePlace = en.status.MonstersCouldBePlacedBefreLim ();
			if (ToPlace > CouldBePlace)
				MonstersToOutscirts = (byte) (ToPlace - CouldBePlace);
			else
				MonstersToOutscirts = 0;
			ToPlace -= MonstersToOutscirts;
			MonstersInEachGate =  (byte) (ToPlace / en.openGates.Count);
			byte md = (byte) (  ToPlace % en.openGates.Count);
			if (md > 0)
			{
				isExtraMonsterToSurgeGate = true;
				md--;
			}

			if (md != 0)
				PlayerChoose (md);
			else
				PlaceMonsters ();




		}


		private void  PlayerChoose(  byte remainMonsters)
		{

		}

		private void  PlaceMonsters()
		{ //ЗАГЛУШКА
			en.io.PrintToLog(" MonsterSurge "+ MonstersInEachGate + "  " + MonstersToOutscirts + " " + isExtraMonsterToSurgeGate   ,14); 
			if (isExtraMonsterToSurgeGate)
			if (! ((ArchemUnstableLoc)en.locs [surgeLoc]).MonseterPlaced ())
				return;

			ArchemUnstableLoc l;
			foreach (GatePrototype g in en.openGates)
			{
				l = (ArchemUnstableLoc)en.locs [g.GetArchemLoc ()];
				for (int i=0; i<MonstersInEachGate; i++) 
					if (! l.MonseterPlaced ())
						return;
			}

			for (int i=0; i< MonstersToOutscirts; i++)
				if (! en.status.PlacedToOutscirts())
					return;

			en.curs.resolvingMythos.Step2 ();  


		}

	}
}

