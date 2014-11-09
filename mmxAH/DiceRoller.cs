using System;
using System.Collections.Generic; 
namespace mmxAH
{

	public class DiceRoller
	{   private const byte dice=6;
		GameEngine en;

		public DiceRoller( GameEngine eng)
		{ 
			en = eng;


		}

		public  byte RollDiceWithTreshhold(byte num, byte tresh)
		{
			string str = "";
			str+= num.ToString()+ "d"+dice.ToString();
			if (num == 0)
			{
				en.io.ServerWrite (str);
				return 0;
			}
		str+= (" = ( ");
		 byte sucesses=0;
		 byte curRoll = 0;
		for (int i=0; i< num; i++)
			{ 	if (i != 0)
				{
					str+= (",");

				}

				curRoll = Roll();
			  str+= curRoll.ToString() ;
				if (curRoll >= tresh)
				{ str += "*";
					sucesses++;

				}

		  }

			str+=")";
			en.io.ServerWrite (str); 
			return sucesses;

	  }

		private byte Roll()
		{  	System.Threading.Thread.Sleep (5);
			Random r = new Random (Environment.TickCount);


			 return (byte) ((r.NextDouble()*dice)+1); 
			  
		}

		public byte RollOneDice()
		{   byte res = Roll ();
			en.io.ServerWrite("1d"+dice.ToString()+ " = "+ res.ToString()+ " . ");
			return res;

		}



	}
}
