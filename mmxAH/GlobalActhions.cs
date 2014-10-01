using System;

namespace mmxAH
{
	public  class GlobalActhions
	{ private GameEngine en;
		public GlobalActhions ( GameEngine eng)
		{en=eng;

		} 

		public void ReturnToArchem (short ow, byte inv=40)
		{ if (inv == 40)
				inv = en.clock.GetCurPlayer (); 

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


		}


	}
}

