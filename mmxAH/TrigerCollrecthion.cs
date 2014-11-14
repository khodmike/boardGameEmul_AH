using System;
using System.Collections.Generic;

namespace mmxAH
{
	public class TrigerCollrecthion
	{  private List< MandoratoryTriger> mandaratoryTrigers;
		private List< ChooseTriger> chooseTrigers;
		public TrigerCollrecthion ()
		{ mandaratoryTrigers= new List< MandoratoryTriger>();
			chooseTrigers= new List< ChooseTriger >();  
		}

		public void AddChooseTriger (ChooseTriger t)
		{ 
			chooseTrigers.Add (t);
         
		}

		
		public void AddMandarotoryTriger( MandoratoryTriger t)
		{ 
			mandaratoryTrigers .Add (t);

		}

		public void RemoveTriger (string Code)
		{
			foreach ( MandoratoryTriger t in mandaratoryTrigers)
			{ if( t.GetCodeName() == Code)
				{
					mandaratoryTrigers.Remove(t);
					return;
				}

			}

			foreach (ChooseTriger t in chooseTrigers)
			{ if( t.GetCodeName() == Code)
				{
					chooseTrigers.Remove(t);
					return;
				}

			}
		}

		public List<IOOption> GetChooseOpthions (TrigerEvent even, short CheckCode)
		{  List<IOOption> res= new List<IOOption>();
           foreach ( ChooseTriger  t in chooseTrigers)
				if( t.ev== even && t.isAvable(CheckCode) )
					res.Add (t.opt);
			return res;

		}

		public void ExecuteMandeoratory(TrigerEvent even)
		{ 
			for (int i = 0; i < mandaratoryTrigers.Count; i++)
			{
				MandoratoryTriger t = mandaratoryTrigers [i];
				if (t.ev == even)
					t.isExecuted = false;
				else
					t.isExecuted = true;
			}

			do
			{
			} while( ExecuteOne()); 



		}


		private bool ExecuteOne()
		{  for (int i = 0; i < mandaratoryTrigers.Count; i++)
		{
			MandoratoryTriger t = mandaratoryTrigers [i];
			if (t.isExecuted == false)
			{
					t.isExecuted= true;

				t.Execute ();
				return true;
			}
		}

			return false;


		}
	}





	public class MandoratoryTriger
	{ public TrigerEvent ev;
		public bool isExecuted;
		private Func ep;
		private string CodeName;
		public MandoratoryTriger (TrigerEvent e, Func entetyPoint, string pCodeName="")
		{ ev=e;
			ep=entetyPoint; 
			CodeName= pCodeName;
			isExecuted = false; 
		}

		public string GetCodeName ()
		{
			return CodeName;
		}

		public void Execute()
		{ ep ();

		}

	}


	public struct ChooseTriger
	{ public TrigerEvent ev;
		public IOOption opt;
		private string CodeName;
		private FuncBoolReturn cf;
		public ChooseTriger (TrigerEvent e, IOOption o, string pCodeName="", FuncBoolReturn  checkFunc= null)
		{ ev=e;
		 opt=o;
		 CodeName= pCodeName;
			cf = checkFunc; 
		}

			public string GetCodeName ()
		{
			return CodeName;
		}

		public bool  isAvable( short checkCode)
		{ if (cf == null)
				return true;
			else 
			return cf (checkCode);
		}


	}

	public enum TrigerEvent
	{ Upkeep=1,
      MoveAH,
	  MoveOW,
      SCRerolls,
		BeforeGateClosing

	}
}

